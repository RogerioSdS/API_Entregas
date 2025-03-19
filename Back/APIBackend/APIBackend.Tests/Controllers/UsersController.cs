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
        private readonly Mock<IUserService> _userServiceMock; // Objeto falso que simula o IUserService
        private readonly UsersController _controller; // A controller que vamos testar

        // Construtor: executa antes de cada teste
        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>(); // Cria o mock do IUserService
            _controller = new UsersController(_userServiceMock.Object); // Passa o mock para a controller
        }

        [Fact] // Marca que isso é um teste
        public async Task CreateUser_ValidData_ReturnsCreatedAtAction()
        {
            // Arrange (Preparar o cenário)
            // Criamos um UserDTO com dados fictícios que seriam enviados para a controller
            var userDto = new UserDTO
            {
                FirstName = "Maria", // Nome do usuário
                LastName = "Silva", // Sobrenome
                Email = "maria.silva@example.com", // Email válido
                Address = "Rua Teste, 123", // Endereço
                ZipCode = 12345, // CEP
                City = "São Paulo" // Cidade
            };

            // Criamos um objeto User que representa o resultado do serviço
            var userCriado = new User
            {
                Id = 1, // ID que o usuário terá após ser criado
                UserName = userDto.Email, // O IdentityUser usa Email como UserName por padrão
                FirstName = userDto.FirstName, // Nome
                LastName = userDto.LastName, // Sobrenome
                Email = userDto.Email, // Email
                Address = userDto.Address, // Endereço
                ZipCode = userDto.ZipCode, // CEP
                City = userDto.City // Cidade
            };

            // Configuramos o mock para dizer: "Quando AddUserAsync for chamado, retorne userCriado"
            _userServiceMock.Setup(x => x.AddUserAsync(userDto, "User", "senha123", true))
                .ReturnsAsync(userCriado); // Simula o serviço retornando o usuário criado

            // Act (Executar a ação)
            // Chamamos o método CreateUser da controller com os dados preparados
            var resultado = await _controller.CreateUser(userDto, "User", "senha123");

            // Assert (Verificar o resultado)
            // Verificamos se o resultado é do tipo CreatedAtActionResult (resposta de sucesso ao criar algo)
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);

            // Verificamos se o status HTTP é 201 (Created)
            Assert.Equal(201, createdResult.StatusCode);

            // Verificamos se o valor retornado é o mesmo UserDTO que enviamos
            Assert.Equal(userDto, createdResult.Value);

            // Verificamos se o nome da ação retornada é "model" (como no CreatedAtAction da controller)
            Assert.Equal("model", createdResult.ActionName);
        }

        [Fact] // Outro teste
        public async Task CreateUser_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange (Preparar o cenário)
            // Criamos um UserDTO com dados mínimos para testar o caso de falha
            var userDto = new UserDTO
            {
                FirstName = "João", // Nome
                LastName = "Souza", // Sobrenome
                Email = "joao.souza@example.com", // Email
                Address = "Rua Fail, 456" // Endereço (obrigatório no DTO)
            };

            // Configuramos o mock para dizer: "Quando AddUserAsync for chamado, retorne null" (simula erro)
            _userServiceMock.Setup(x => x.AddUserAsync(userDto, "User", "senha123", true))
                .ReturnsAsync((User)null);

            // Act (Executar a ação)
            // Chamamos o método CreateUser com os dados
            var resultado = await _controller.CreateUser(userDto, "User", "senha123");

            // Assert (Verificar o resultado)
            // Verificamos se o resultado é do tipo BadRequestObjectResult (resposta de erro)
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);

            // Verificamos se o status HTTP é 400 (Bad Request)
            Assert.Equal(400, badRequestResult.StatusCode);

            // Verificamos se a mensagem de erro é a esperada
            Assert.Equal("Erro ao criar usuário.João Souza", badRequestResult.Value);
        }
    }
}