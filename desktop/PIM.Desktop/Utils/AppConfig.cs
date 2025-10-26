using System.Configuration;

namespace PIM.Desktop.Utils
{
    /// <summary>
    /// Gerencia as configurações da aplicação
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// URL base da API
        /// </summary>
        public static string ApiBaseUrl
        {
            get
            {
                // Tenta ler do app.config, se não existir usa a URL de desenvolvimento local
                string url = ConfigurationManager.AppSettings["ApiBaseUrl"];
                return string.IsNullOrEmpty(url)
                    ? "https://localhost:7095"
                    : url;
            }
        }

        /// <summary>
        /// Timeout padrão para requisições HTTP (em segundos)
        /// </summary>
        public static int RequestTimeout
        {
            get
            {
                string timeout = ConfigurationManager.AppSettings["RequestTimeout"];
                return string.IsNullOrEmpty(timeout) ? 30 : int.Parse(timeout);
            }
        }
    }
}
