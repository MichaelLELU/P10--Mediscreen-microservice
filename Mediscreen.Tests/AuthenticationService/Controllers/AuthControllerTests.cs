using AuthenticationService.Api.Controllers;
using AuthenticationService.Api.Models;
using AuthenticationService.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Mediscreen.Tests.AuthenticationService.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        Mock<IUserStore<ApplicationUser>> userStoreMock = new();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object,
            Options.Create(new IdentityOptions()),
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            null!,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        _tokenServiceMock = new Mock<ITokenService>();

        _controller = new AuthController(
            _userManagerMock.Object,
            _tokenServiceMock.Object);
    }

    private static LoginRequest CreateLoginRequest()
    {
        return new LoginRequest
        {
            Email = "demo@mediscreen.com",
            Password = "Demo123!"
        };
    }

    private static ApplicationUser CreateUser()
    {
        return new ApplicationUser
        {
            Id = "user-test-123",
            Email = "demo@mediscreen.com",
            UserName = "demo@mediscreen.com"
        };
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnOk()
    {
        // Arrange
        LoginRequest request = CreateLoginRequest();
        ApplicationUser user = CreateUser();

        LoginResponse loginResponse = new()
        {
            Token = "jwt-token-test",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Email = user.Email!
        };

        _userManagerMock
            .Setup(manager =>
                manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager =>
                manager.CheckPasswordAsync(
                    user,
                    request.Password))
            .ReturnsAsync(true);

        _tokenServiceMock
            .Setup(service => service.CreateToken(user))
            .Returns(loginResponse);

        // Act
        ActionResult<LoginResponse> result =
            await _controller.Login(request);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        LoginResponse response =
            Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal("jwt-token-test", response.Token);
        Assert.Equal(user.Email, response.Email);

        _tokenServiceMock.Verify(
            service => service.CreateToken(user),
            Times.Once);
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldReturnUnauthorized()
    {
        // Arrange
        LoginRequest request = CreateLoginRequest();

        _userManagerMock
            .Setup(manager =>
                manager.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        ActionResult<LoginResponse> result =
            await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(
            result.Result);

        _userManagerMock.Verify(
            manager => manager.CheckPasswordAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenServiceMock.Verify(
            service => service.CreateToken(
                It.IsAny<ApplicationUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Login_WhenPasswordIsInvalid_ShouldReturnUnauthorized()
    {
        // Arrange
        LoginRequest request = CreateLoginRequest();
        ApplicationUser user = CreateUser();

        _userManagerMock
            .Setup(manager =>
                manager.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager =>
                manager.CheckPasswordAsync(
                    user,
                    request.Password))
            .ReturnsAsync(false);

        // Act
        ActionResult<LoginResponse> result =
            await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(
            result.Result);

        _tokenServiceMock.Verify(
            service => service.CreateToken(
                It.IsAny<ApplicationUser>()),
            Times.Never);
    }
}