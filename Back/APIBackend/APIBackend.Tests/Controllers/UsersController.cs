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
                Email =  "rogeio@rogrio.com",
                Password = "Perol@09",
                FirstName = "RogerS",
                LastName = "soares",
                Address = "Rua A, 120",
                Complement = "Jardim B",
                ZipCode = "15015015",
                City = "São José do rio preto",
                Description = "string",
                Role = "Admin",
                SignInAfterCreation = true,
                CreditLimit = 0,
                IsAdmin = true,
                AccessAllowed = true,
                CreditCardNumber = "string",
                FatureDay = 0
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
                Role = userDto.Role,
                SignInAfterCreation = userDto.SignInAfterCreation,
                CreditLimit = userDto.CreditLimit,
                IsAdmin = userDto.IsAdmin,
                AccessAllowed = userDto.AccessAllowed,
                CreditCardNumber = userDto.CreditCardNumber,
                FatureDay = userDto.FatureDay
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

        [Fact] // Outro teste para o caso de erro
        public async Task CreateUser_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange (Preparar o cenário): Preparamos dados mínimos para testar o caso de falha
            var userDto = new UserDTO
            {
                Password = "senha123",
                Role = "User",
                SignInAfterCreation = true,
                FirstName = "Maria", // Nome do usuário
                LastName = "Silva", // Sobrenome
                Email = "maria.silva@example.com", // Email válido
                Address = "Rua Teste, 123", // Endereço (obrigatório)
                ZipCode = "12345", // CEP
                City = "São Paulo" // Cidade
            };

            // Configuramos o mock: Quando AddUserAsync for chamado, ele retornará null (simulando um erro)
            _userServiceMock.Setup(x => x.AddUserAsync(userDto))
                .ReturnsAsync(null as UserDTO); // Retorna null, indicando falha na criação do usuário

            // Act (Executar a ação): Chama o método CreateUser da controller com os dados preparados
            var resultado = await _controller.CreateUser(userDto);

            // Assert (Verificar o resultado): Verificamos se o erro foi tratado corretamente

            // Verificamos se o resultado é do tipo BadRequestObjectResult (resposta de erro)
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);

            // Verificamos se o status HTTP é 400 (Bad Request), que indica falha na solicitação
            Assert.Equal(400, badRequestResult.StatusCode);

            // Verificamos se a mensagem de erro é a esperada
            Assert.Equal("Erro ao criar usuário.Maria Silva", badRequestResult.Value);
        }

        [Fact] // Teste para o método GetUserByName
        public async Task GetUserByName_ReturnsOkResult()
        {
            // Arrange (Preparar o cenário): Preparamos os dados para o teste
            var userDto = new UserDTO
            {
                Email =  "rogeio@rogrio.com",
                Password = "Perol@09",
                FirstName = "RogerS",
                LastName = "soares",
                Address = "Rua A, 120",
                Complement = "Jardim B",
                ZipCode = "15015015",
                City = "São José do rio preto",
                Description = "string",
                Role = "Admin",
                SignInAfterCreation = true,
                CreditLimit = 0,
                IsAdmin = true,
                AccessAllowed = true,
                CreditCardNumber = "string",
                FatureDay = 0
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
                Role = userDto.Role,
                SignInAfterCreation = userDto.SignInAfterCreation,
                CreditLimit = userDto.CreditLimit,
                IsAdmin = userDto.IsAdmin,
                AccessAllowed = userDto.AccessAllowed,
                CreditCardNumber = userDto.CreditCardNumber,
                FatureDay = userDto.FatureDay
            };

            _userServiceMock.Setup(x => x.GetUserByNameAsync(userDto.FirstName))
                .ReturnsAsync(new List<object> { userDtoCriado });

            var resultadoFindName = await _controller.GetUserByName(userDto.FirstName);

            // Assert (Verificar o resultado): Verificamos se o resultado foi retornado corretamente
            Assert.IsType<OkObjectResult>(resultadoFindName);

            var findResult = Assert.IsType<OkObjectResult>(resultadoFindName);
            
            // Verificamos se o status HTTP é 201 (Created), indicando que o recurso foi criado
            Assert.Equal(200, findResult.StatusCode);

            // Verificamos se o valor retornado é o mesmo UserDTO enviado para a criação
            Assert.Equivalent(userDto, findResult.Value);
        }
    }
}
