using APIBackend.API.Controllers;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace APIBackend.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock; // Mock para simular o IUserService
        private readonly UsersController _controller; // Controller que vamos testar

        // Construtor: prepara o ambiente antes de cada teste
        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>(); // Cria o mock do IUserService
            _controller = new UsersController(_userServiceMock.Object); // Passa o mock para a controller
        }

        [Fact] // Marca que esse método é um teste
        public async Task CreateUser_ValidData_ReturnsCreatedAtAction()
        {
            // Arrange (Preparar o cenário): Preparamos os dados para o teste
            var userDto = new UserDTO
            {
                Email = "r",
                Password = "Perola09",
                FirstName = "RogerS",
                LastName = "soares",
                Address = "Rua A, 120",
                Complement = "Jardim B",
                ZipCode = "15015015",
                City = "São José do rio preto",
                Description = "string",
                Role = "Admin"
            };

            // Preparamos o que o serviço deverá retornar após a criação do usuário
            var userDtoCriado = new UserDTO
            {
                Email = userDto.Email,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Address = userDto.Address,
                Complement = userDto.Complement,
                ZipCode = userDto.ZipCode,
                City = userDto.City,
                Description = userDto.Description,
                Role = userDto.Role
            };

            // Configuramos o mock: Quando o método AddUserAsync for chamado, ele retorna o userDtoCriado
            _userServiceMock.Setup(x => x.AddUserAsync(userDto))
                .ReturnsAsync(userDtoCriado); // Retorna o userDtoCriado quando o método for chamado

            // Act (Executar a ação): Chama o método CreateUser da controller
            var resultado = await _controller.CreateUser(userDto);

            // Verificamos se o resultado é do tipo CreatedResult (resposta de sucesso)
            var createdResult = Assert.IsType<CreatedResult>(resultado);

            // Verificamos se o status HTTP é 201 (Created), indicando que o recurso foi criado
            Assert.Equal(201, createdResult.StatusCode);

            // Verificamos se o valor retornado é o mesmo UserDTO enviado para a criação
            Assert.Equivalent(userDto, createdResult.Value);
        }


        [Fact]
        public async Task CreateUser_InvalidEmail_ReturnsBadRequest()
        {
            var userDto = new UserDTO
            {
                Email = "r",
                Password = "Perola09",
                FirstName = "RogerS",
                LastName = "soares",
                Address = "Rua A, 120",
                Complement = "Jardim B",
                ZipCode = "15015015",
                City = "São José do rio preto",
                Description = "string",
                Role = "Admin"
            };

            var resultado = await _controller.CreateUser(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetUserByName_ReturnsOkResult()
        {
            var userDto = new UserDTO
            {
                Email = "rogeio@rogrio.com",
                Password = "Perol@09",
                FirstName = "Roger",
                LastName = "soares",
                Address = "Rua A, 120",
                Complement = "Jardim B",
                ZipCode = "15015015",
                City = "São José do rio preto",
                Description = "string",
                Role = "Admin"
            };

            var userDtoCriado = new UserDTO
            {
                Email = userDto.Email,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Address = userDto.Address,
                Complement = userDto.Complement,
                ZipCode = userDto.ZipCode,
                City = userDto.City,
                Description = userDto.Description,
                Role = userDto.Role
            };

            _userServiceMock.Setup(x => x.GetUserByNameAsync(userDto.FirstName))
                .ReturnsAsync(new List<object> { userDtoCriado });

            var resultadoFindName = await _controller.GetUserByName(userDto.FirstName);

            Assert.IsType<OkObjectResult>(resultadoFindName);

            var findResult = Assert.IsType<OkObjectResult>(resultadoFindName);

            Assert.Equal(200, findResult.StatusCode);

            // Converter findResult.Value para List<object> e acessar o primeiro item
            var resultList = Assert.IsType<List<object>>(findResult.Value);
            var firstItem = Assert.IsType<UserDTO>(resultList[0]);
            
            Assert.Equal(userDtoCriado.FirstName, firstItem.FirstName);

            //Assert.Equivalent(userDto, findResult.Value);
        }
    }
}
