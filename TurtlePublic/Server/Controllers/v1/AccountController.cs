using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class AccountController : ControllerBase
{
    private readonly Account.IService accounts;
    public AccountController(Account.IService accounts)
    {
        this.accounts = accounts;
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
    /// Validates user credentials, producing an error message if appropriate.
    /// </summary>
    /// <param name="email">Unique account email address.</param>
    /// <param name="password">Plaintext password entered during login.</param>
    [HttpPost("AttemptLogin")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(typeof(Account))]
    public async Task<IActionResult> AttemptLogin(
        string email,
        string password)
    {
        try
        {
            var account = await accounts.AttemptLogin(email, password);
            return account is null
                ? Unauthorized()
                : Ok(account);
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
    /// Issues a password reset token and emails the user a link (failures are
    /// silently ignored).
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
    /// Sets a new password in the database for a pending password reset.
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