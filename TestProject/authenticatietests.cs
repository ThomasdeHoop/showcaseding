using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AccountControllerTests
{
    [Fact]
    public async Task Register_ValidModel_ReturnsRedirectToActionResult()
    {
        // Arrange
        var accountController = GetAccountController();
        var model = new RegisterViewModel
        {
            Email = "test@example.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        // Act
        var result = await accountController.Register(model) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public async Task Register_InvalidModel_ReturnsBadRequestWithModelStateErrors()
    {
        // Arrange
        var accountController = GetAccountController();
        var model = new RegisterViewModel
        {
            Email = "invalid-email", // Invalid email format
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };
        accountController.ModelState.AddModelError("Email", "Invalid email format");

        // Act
        var result = await accountController.Register(model) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var errors = result.Value as SerializableError;
        Assert.NotNull(errors);
        Assert.True(errors.ContainsKey("Email"));
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsRedirectToActionResult()
    {
        // Arrange
        var accountController = GetAccountController();
        var model = new LoginViewModel
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        // Act
        var result = await accountController.Login(model, null) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequestWithModelStateErrors()
    {
        // Arrange
        var accountController = GetAccountController();
        var model = new LoginViewModel
        {
            Email = "test@example.com",
            Password = "InvalidPassword"
        };
        accountController.ModelState.AddModelError(string.Empty, "Invalid login attempt");

        // Act
        var result = await accountController.Login(model, null) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var errors = result.Value as SerializableError;
        Assert.NotNull(errors);
        Assert.True(errors.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task Logout_ReturnsRedirectToActionResult()
    {
        // Arrange
        var accountController = GetAccountController();

        // Act
        var result = await accountController.Logout() as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
    }

    private AccountController GetAccountController()
    {
        var userStoreMock = new Mock<IUserStore<object>>();
        var userManagerMock = new Mock<UserManager<object>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<object>>();
        var signInManagerMock = new Mock<SignInManager<object>>(userManagerMock.Object, httpContextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);
        var loggerMock = new Mock<ILogger<AccountController>>();

        userManagerMock.Setup(u => u.CreateAsync(It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        signInManagerMock.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        signInManagerMock.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        return new AccountController(userManagerMock.Object, signInManagerMock.Object, loggerMock.Object);
    }
}

public class RegisterViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class LoginViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class AccountController : ControllerBase
{
    private readonly UserManager<object> _userManager;
    private readonly SignInManager<object> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<object> userManager, SignInManager<object> signInManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new object(); // Create a user object here if needed
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
    }

    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return BadRequest(ModelState);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
