using APIBackend.API.Controllers;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace APIBackend.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<UserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        // Teste para POST /api/user
        [Fact]
        public async Task CreateUser_ValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var userDto = new UserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "123 Street",
                ZipCode = 12345,
                City = "Test City"
            };
            var user = new User
            {
                Id = 1,
                UserName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Address = userDto.Address,
                ZipCode = userDto.ZipCode,
                City = userDto.City
            };
            _userServiceMock.Setup(x => x.AddUserAsync(It.IsAny<User>(), "User", "password123", true))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.CreateUser(userDto, "User", "password123");

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(user, createdResult.Value);
            Assert.Equal("user", createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateUser_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDTO { FirstName = "John", LastName = "Doe" };
            _userServiceMock.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), true))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.CreateUser(userDto, "User", "password123");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Erro ao criar usuário.John Doe", badRequestResult.Value);
        }

        // Teste para GET /api/user/getUserById/{id}
        [Fact]
        public async Task GetUserById_UserExists_ReturnsOk()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "John" };
            var roles = new List<string> { "User" };
            _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
            _userServiceMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<dynamic>(okResult.Value);
            Assert.Equal(user, returnValue.User);
            Assert.Equal(roles, returnValue.Roles);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetUserByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // Teste para DELETE /api/user/{id}
        [Fact]
        public async Task DeleteUser_UserExists_ReturnsNoContent()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "John" };
            _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
            _userServiceMock.Setup(x => x.DeleteUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetUserByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeleteUser(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Usuário não encontrado.", notFoundResult.Value);
        }
    }
}