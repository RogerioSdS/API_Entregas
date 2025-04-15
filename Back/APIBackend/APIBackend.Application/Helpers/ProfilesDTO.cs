using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Helpers
{
    public class ProfilesDTO : AutoMapper.Profile
    {
       
        public ProfilesDTO()
        {
            /// <summary>
            /// Mapeia a entidade <"Evento"/> para o objeto <"EventoDTO"/> e vice-versa.
            /// </summary>
            /// <remarks>
            /// Essa mapeamento é realizado usando a biblioteca AutoMapper.
            /// </remarks>
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserUpdateFromUserDTO, User>().ReverseMap();
            CreateMap<UserUpdateFromAdminDTO, User>().ReverseMap();
            CreateMap<LoginDTO, User>().ReverseMap();
        }

        /// <summary>
        /// O AutoMapper é uma biblioteca de mapeamento de objetos que permite mapear
        /// automaticamente objetos de uma classe para outra, transformando os dados de uma
        /// classe em uma outra de acordo com as regras de mapeamento definidas.
        /// 
        /// Ele é muito útil quando temos classes com propriedades similares ou iguais,
        /// mas com nomes diferentes. Com o AutoMapper, podemos definir regras de
        /// mapeamento de forma simples, evitando código repetitivo e propenso a erros.
        /// </summary>
    }
}