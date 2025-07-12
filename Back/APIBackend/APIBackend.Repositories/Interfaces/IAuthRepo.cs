using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces
{
    public interface IAuthRepo
    {
        /// <summary>
        /// Salva um novo token de refresh no banco de dados.
        /// </summary>
        /// <param name="token">O objeto RefreshToken a ser salvo.</param>
        public Task SaveTokenAsync(RefreshToken token);

        /// <summary>
        /// Obtém um token de refresh ativo e não revogado pelo id do usuário.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>O token de refresh correspondente ou null caso não exista.</returns>
        public Task<RefreshToken?> GetValideTokenByIdAsync(int id);

        /// <summary>
        /// Obtém um token de refresh pelo id.
        /// </summary>
        /// <param name="id">Id do token de refresh.</param>
        /// <returns>O token de refresh correspondente ou null caso não exista.</returns>
        public Task<RefreshToken?> GetTokenByIdAsync(int id);

        /// <summary>
        /// Obtém um token de refresh ativo e não revogado pelo valor do token.
        /// </summary>
        /// <param name="refreshToken">O valor do token de refresh.</param>
        /// <returns>O token de refresh correspondente ou null caso não exista.</returns>
        public Task<RefreshToken?> GetRefreshTokenByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Retorna todos os tokens de refresh associados a um usuário específico.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>Lista de tokens de refresh ou null se não houver tokens.</returns>
        public Task<List<RefreshToken>?> GetAllTokenByIdAsync(int id);

        /// <summary>
        /// Obtém o usuário associado a um token de refresh específico.
        /// </summary>
        /// <param name="refreshToken">O valor do token de refresh.</param>
        /// <returns>O usuário correspondente ou null se não encontrado.</returns>
        public Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoga (inativa) um token de refresh pelo id.
        /// </summary>
        /// <param name="id">Id do token a ser revogado.</param>
        public Task RevokeTokenAsync(int id);

        /// <summary>
        /// Atualiza uma lista de tokens de refresh no banco.
        /// </summary>
        /// <param name="tokens">Lista de tokens para atualização.</param>
        public Task UpdateTokenAsync(List<RefreshToken> tokens);

        /// <summary>
        /// Remove tokens antigos da base de dados.
        /// </summary>
        /// <param name="listTokens">Lista de tokens a serem removidos.</param>
        public Task RemoveOldTokensAsync(List<RefreshToken> listTokens);

        /// <summary>
        /// Deleta um token de refresh específico.
        /// </summary>
        /// <param name="token">Token a ser deletado.</param>
        public Task DeleteTokenAsync(RefreshToken token);

        /// <summary>
        /// Cria um token para confirmação de e-mail para o usuário com o e-mail informado.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Token de confirmação de e-mail.</returns>
        public Task<string> CreateEmailConfirmationTokenAsync(string email);

        /// <summary>
        /// Confirma o e-mail do usuário usando o token de confirmação.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="token">Token de confirmação recebido.</param>
        /// <returns>True se a confirmação for bem-sucedida, false caso contrário.</returns>
        public Task<bool> ConfirmEmailAsync(string email, string token);

        /// <summary>
        /// Cria um token para redefinição de senha para o usuário com o e-mail informado.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Token para redefinição de senha.</returns>
        public Task<string> CreateResetPasswordTokenAsync(string email);

        /// <summary>
        /// Redefine a senha do usuário utilizando o token de redefinição.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="token">Token de redefinição de senha.</param>
        /// <param name="newPassword">Nova senha para o usuário.</param>
        /// <returns>True se a redefinição for bem-sucedida, false caso contrário.</returns>
        /// 
        public Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        /// <summary>
        /// Obtém um token de refresh pelo valor do token.
        /// </summary>
        /// <param name="refreshToken">O valor do token de refresh.</param>
        /// <returns>O token de refresh correspondente ou null caso não exista.</returns>
        public Task<RefreshToken?> GetTokenByRefreshTokenAsync(string refreshToken);
    }
}
