using Asp.Versioning;
using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class AccountController : ControllerBase
{
    private readonly Account.IService accounts;
    private readonly IConfiguration config;
    public AccountController(Account.IService accounts, IConfiguration config)
    {
        this.accounts = accounts;
        this.config = config;
    }

    /*
    [HttpPost("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Account>))]
    public async Task<IActionResult> List(
        int page,
        int count)
    {
        try
        {
            var list = await accounts.List(page, count);
            return Ok(list);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("Get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(typeof(Account))]
    public async Task<IActionResult> Get(
        int id)
    {
        try
        {
            var account = await accounts.Get(id);
            return account is null
                ? NotFound()
                : Ok(account);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    */

    /// <summary>
    /// Validates user credentials, storing any resulting refresh token in a
    /// cookie, else producing a "401 Unauthorized."
    /// </summary>
    /// <param name="email">Unique account email address.</param>
    /// <param name="password">Plaintext password entered during login.</param>
    [HttpPost("AttemptLogin")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(typeof(Account.WithSession))]
    public async Task<IActionResult> AttemptLogin(
        string email,
        [DataType(DataType.Password)] string password)
    {
        try
        {
            var account = await accounts.AttemptLogin(email, password);
            if (account is null)
            {
                return Unauthorized();
            }

            var lifespan = config.GetValue<int>("Jwt:RefreshSeconds");
            Response.Cookies.Append("RefreshToken",
                account.RefreshToken,
                new()
                {
                    IsEssential = true,
                    Secure = true,
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddSeconds(lifespan)
                });
            account.RefreshToken = "Content omitted";

            return Ok(account);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Nullifies the session by destroying any refresh token cookie; discard
    /// the access token.
    /// </summary>
    [HttpPost("LogOut")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogOut()
    {
        try
        {
            if (Request.Cookies.ContainsKey("RefreshToken"))
            {
                Response.Cookies.Delete("RefreshToken");
            }

            await accounts.LogOut();
            return Ok();
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new set of access and refresh tokens, also updating the refresh
    /// cookie expiration.
    /// </summary>
    [HttpPost("Refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(typeof(Account.WithSession))]
    public async Task<IActionResult> Refresh(string accessToken, string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = Request.Cookies.LastOrDefault((it) => it.Key == "RefreshToken").Value;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized();
                }
            }

            try
            {
                var newTokens = await accounts.Refresh(accessToken, refreshToken);
                if (newTokens is null)
                {
                    // Propagates as "Unauthorized"
                    throw new ApplicationException("New tokens are blank");
                }

                var lifespan = config.GetValue<int>("Jwt:RefreshSeconds");
                Response.Cookies.Append("RefreshToken",
                    newTokens.RefreshToken,
                    new()
                    {
                        IsEssential = true,
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddSeconds(lifespan)
                    });
                newTokens.RefreshToken = "Content omitted";

                return Ok(newTokens);
            } catch (Exception)
            {
                return Unauthorized();
            }
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /*
    TODO
    [HttpPost("SignUp")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status4...)]
    public async Task<IActionResult> SignUp(
        string email,
        string firstName,
        string lastName,
        ...)
    {
        try
        {
            var list = await accounts.SignUp(...);
            return Ok(list);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    */

    /// <summary>
    /// Issues a password reset token and emails the user a link; failures and
    /// non-existent accounts are ignored for obfuscation.
    /// </summary>
    /// <param name="email">Email entered by user for reset.</param>
    [HttpPost("BeginReset")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BeginReset(
        string email)
    {
        try
        {
            await accounts.BeginReset(email);
            return Ok();
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Allows display of the user's name and info as they select an account
    /// password.
    /// </summary>
    /// <param name="resetToken">Hex-encoded key bytes for the reset token.</param>
    [HttpPost("GetResetDetails")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(typeof(Account))]
    public async Task<IActionResult> GetResetDetails(
        string resetToken)
    {
        try
        {
            var account = await accounts.GetResetDetails(resetToken);
            return account is null
                ? Unauthorized()
                : Ok(account);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Sets a new password in the database for a pending password reset; removes
    /// any stored token from the account.
    /// </summary>
    /// <param name="resetToken">Hex-encoded key bytes for the reset token.</param>
    /// <param name="password">Plaintext password to assign to the account.</param>
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword(
        string resetToken,
        string password)
    {
        try
        {
            await accounts.ResetPassword(resetToken, password);
            return Ok();
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}