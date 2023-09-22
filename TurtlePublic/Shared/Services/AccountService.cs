using Generated;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace TurtlePublic.Services;

public class AccountService : Account.IBackendService
{
    private readonly ILogger<AccountService> logger;
    private readonly Account.Repository repo;
    public AccountService(ILogger<AccountService> logger, Account.Repository repo)
    {
        this.logger = logger;
        this.repo = repo;
    }

    public async Task<Account> Get(int id)
    {
        return await repo.dbo__Account_GetById(id);
    }

    public async Task<List<Account>> List(int page, int count)
    {
        return await repo.dbo__Account_List(page, count);
    }

    public const string EXPIRED_PASSWORD = "This password has expired; please select a new one";
    public const string CURRENT_PASSWORD_SCHEME = nameof(SaltedPbkdf2);

    private string CreateSalt()
    {
        int keySize = 64;
        var salt = RandomNumberGenerator.GetBytes(keySize);
        return Convert.ToBase64String(salt);
    }

    private string SaltedPbkdf2(string password, string salt)
    {
        int keySize = 64, iterations = 350000;
        var hashAlgorithm = HashAlgorithmName.SHA512;
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            Convert.FromBase64String(salt),
            iterations,
            hashAlgorithm,
            keySize);
        return Convert.ToBase64String(hash);
    }

    private string CalculateHash(string scheme, string password, string salt)
    {
        return scheme switch
        {
            nameof(SaltedPbkdf2) => SaltedPbkdf2(password, salt),
            _ => throw new Exception($"Invalid password scheme '{scheme}' for account")
        };
    }

    public async Task<Account> AttemptLogin(string email, string password)
    {
        var account = await repo.dbo__Account_GetWithPassword(email);
        if (account is null)
        {
            return null;
        }

        //TODO
        /*
        if (DateTime.Today - account.PasswordSet > TimeSpan.FromDays(...))
        {
            throw new Exception(EXPIRED_PASSWORD);
        }
        */

        try
        {
            var hash = CalculateHash(account.PasswordScheme, password, account.PasswordSalt);
            if (hash == account.PasswordHash)
            {
                logger.LogInformation("User '{0}' has provided valid login credentials", email);
            } else
            {
                logger.LogWarning("User '{0}' attempted log in with invalid password", email);
                return null;
            }

            return await repo.dbo__Account_GetById(account.Id);
        } catch (Exception ex)
        {
            logger.LogError(ex, "User '{0}' encountered an error during login", email);
            return null;
        }
    }

    public async Task BeginReset(string email)
    {
        var keyBytes = RandomNumberGenerator.GetBytes(64);
        var key = Convert.ToBase64String(keyBytes);
        var keyHash = CalculateHash(nameof(SaltedPbkdf2), key, "");

        var account = await repo.dbo__Account_SetResetToken(email, keyHash);
        if (account is null)
        {
            logger.LogWarning("Ignored password reset for invalid email '{email}'", email);
        } else
        {
            logger.LogInformation("User '{0}' requested password reset", email);
            //TODO await SendResetEmail(...);
        }
    }

    public async Task<Account> GetResetDetails(string resetToken)
    {
        return await repo.dbo__Account_GetByResetToken(resetToken);
    }

    public async Task ResetPassword(string resetToken, string password)
    {
        var salt = CreateSalt();
        var passwordHash = CalculateHash(CURRENT_PASSWORD_SCHEME, password, salt);

        var account = await repo.dbo__Account_ResetPassword(resetToken, CURRENT_PASSWORD_SCHEME, passwordHash, salt);
        if (account is null)
        {
            logger.LogWarning("User attempted to reset password with invalid token");
        } else
        {
            logger.LogInformation("User '{0}' successfully reset password", account.Email);
        }
    }
}
