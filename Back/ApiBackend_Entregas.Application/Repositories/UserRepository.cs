using System.Threading.Tasks;
using ApiBackend_Entregas.Application.Repositories.Interfaces;
using ApiBackend_Entregas.Models;

namespace ApiBackend_Entregas.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private List<Users> _users = new List<Users>();

        public UserRepository()
        {
            _users.Add(new Users { Email = "exemplo@email.com", Password = "Juli@123" });
            _users.Add(new Users { Email = "rogerio@rogerio.com", Password = "Rogerio@123" });
            _users.Add(new Users { Email = "ana.silva@email.com", Password = "Ana@2024" });
            _users.Add(new Users { Email = "carlos.santos@email.com", Password = "Carlos@456" });
            _users.Add(new Users { Email = "mariana.oliveira@email.com", Password = "Mari@789" });
            _users.Add(new Users { Email = "lucas.fernandes@email.com", Password = "Lucas@321" });
            _users.Add(new Users { Email = "beatriz.mendes@email.com", Password = "Bea@654" });
            _users.Add(new Users { Email = "fernando.lima@email.com", Password = "Fern@987" });
            _users.Add(new Users { Email = "juliana.rocha@email.com", Password = "Juli@753" });
            _users.Add(new Users { Email = "daniel.pereira@email.com", Password = "Dani@159" });
            _users.Add(new Users { Email = "patricia.alves@email.com", Password = "Paty@852" });
            _users.Add(new Users { Email = "ricardo.martins@email.com", Password = "Rica@369" });
        }

        public Task<List<Users>> GetUsersAsync()
        {
            return Task.Run(() => _users);
        }

        public async Task<Users> GetUserAsync(Users model)
        {
            var user = await Task.Run(() => _users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password));

            if (user == null)
            {
                throw new Exception($"Usuário com e-mail {model.Email} e senha fornecida não encontrado.");
            }

            return user;
        }

        async Task<Users> IUserRepository.AddUserAsync(Users user)
        {
            if (user == null)
            {
                throw new Exception("Usuário nulo.");
            }
            //user.Id = Guid.NewGuid();
            await Task.Run(() => _users.Add(user));

            return user;

        }
    }
}
