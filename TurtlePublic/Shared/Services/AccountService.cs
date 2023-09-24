using Generated;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TurtlePublic.Extensions;

namespace TurtlePublic.Services;

public class AccountService : Account.IBackendService
{
    private readonly ILogger<AccountService> logger;
    private readonly Account.Repository repo;
    private readonly IEmailSender emailSender;
    private readonly ILinkPathGenerator linkPathGen;
    private readonly IJwtAuthService jwtAuth;
    public AccountService(ILogger<AccountService> logger, Account.Repository repo, IEmailSender emailSender, ILinkPathGenerator linkPathGen, IJwtAuthService jwtAuth)
    {
        this.logger = logger;
        this.repo = repo;
        this.emailSender = emailSender;
        this.linkPathGen = linkPathGen;
        this.jwtAuth = jwtAuth;
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

    public async Task<Account.WithSession> AttemptLogin(string email, string password)
    {
        var verify = await repo.dbo__Account_GetWithPassword(email);
        if (verify is null)
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
            var hash = CalculateHash(verify.PasswordScheme, password, verify.PasswordSalt);
            if (hash == verify.PasswordHash)
            {
                logger.LogInformation("User '{0}' has provided valid login credentials", email);
            } else
            {
                logger.LogWarning("User '{0}' attempted log in with invalid password", email);
                return null;
            }

            var account = await repo.dbo__Account_GetWithRoles(verify.Id);
            jwtAuth.GenerateToken(account, out var result);
            return result;
        } catch (Exception ex)
        {
            logger.LogError(ex, "User '{0}' encountered an error during login", email);
            return null;
        }
    }

    public Task LogOut()
    {
        // With JWT, this is a purely API-related task
        return Task.CompletedTask;
    }

    public async Task BeginReset(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ValidationException("Email cannot be blank");
        }
        try
        {
            _ = new MailAddress(email);
        } catch (FormatException)
        {
            throw new ValidationException("Provide a valid email address");
        }

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

            var concatName = $"{account.FirstName} {account.LastName}".Trim();
            var hexKey = Convert.ToHexString(keyBytes);
            var link = linkPathGen.GenerateActionPath("reset", HttpUtility.UrlEncode(hexKey));

            await emailSender.SendEmailAsync(account.Email,
                "Account password reset",
                $"""
                <html>
                    <p>Hello {concatName},</p>
                    <p>Click <a href="{link}">this link</a> to set an account password.</p>
                    <br />
                    <p>Can't click the link? Try this one:</p>
                    <p><a href="{link}">{link}</a></p>
                    <br />
                    <p>&mdash;</p>
                    <p>Thanks,</p>
                    <p>Turtles</p>
                </html>
                """);
        }
    }

    public async Task<Account> GetResetDetails(string resetToken)
    {
        var key = Convert.ToBase64String(Convert.FromHexString(resetToken));
        var keyHash = CalculateHash(nameof(SaltedPbkdf2), key, "");
        return await repo.dbo__Account_GetByResetToken(keyHash);
    }

    public async Task ResetPassword(string resetToken, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ValidationException("Password cannot be blank");
        }
        if (password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters");
        }

        var key = Convert.ToBase64String(Convert.FromHexString(resetToken));
        var keyHash = CalculateHash(nameof(SaltedPbkdf2), key, "");
        var salt = CreateSalt();
        var passwordHash = CalculateHash(CURRENT_PASSWORD_SCHEME, password, salt);

        var account = await repo.dbo__Account_ResetPassword(keyHash, CURRENT_PASSWORD_SCHEME, passwordHash, salt);
        if (account is null)
        {
            logger.LogWarning("User attempted to reset password with invalid token");
        } else
        {
            logger.LogInformation("User '{0}' successfully reset password", account.Email);
        }
    }
}
