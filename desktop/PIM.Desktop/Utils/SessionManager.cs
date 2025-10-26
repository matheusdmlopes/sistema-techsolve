using System;

namespace PIM.Desktop.Utils
{
    /// <summary>
    /// Gerencia a sessão do usuário na aplicação
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// Nome completo do usuário logado
        /// </summary>
        public static string UsuarioNome { get; set; }

        /// <summary>
        /// Email do usuário logado
        /// </summary>
        public static string UsuarioEmail { get; set; }

        /// <summary>
        /// Token JWT de autenticação
        /// </summary>
        public static string Token { get; set; }

        /// <summary>
        /// ID do usuário logado
        /// </summary>
        public static int? UsuarioId { get; set; }

        /// <summary>
        /// Perfil de acesso do usuário (Admin, Técnico, Usuário)
        /// </summary>
        public static string PerfilAcesso { get; set; }

        /// <summary>
        /// ID do perfil de acesso
        /// </summary>
        public static int? IdPerfil { get; set; }

        /// <summary>
        /// Nome do setor do usuário
        /// </summary>
        public static string Setor { get; set; }

        /// <summary>
        /// Verifica se há um usuário autenticado
        /// </summary>
        public static bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(Token); }
        }

        /// <summary>
        /// Verifica se o usuário logado é administrador
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                return IsAuthenticated &&
                       !string.IsNullOrEmpty(PerfilAcesso) &&
                       PerfilAcesso.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Limpa todos os dados da sessão (logout)
        /// </summary>
        public static void Clear()
        {
            UsuarioNome = null;
            UsuarioEmail = null;
            Token = null;
            UsuarioId = null;
            PerfilAcesso = null;
            IdPerfil = null;
            Setor = null;
        }
    }
}
