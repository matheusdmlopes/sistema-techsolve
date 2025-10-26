using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PIM.Desktop.Utils;

namespace PIM.Desktop.Services
{
    /// <summary>
    /// Serviço para comunicação com a API TechSolve
    /// </summary>
    public class ApiService
    {
        private static readonly HttpClient _httpClient;
        private static readonly string _baseUrl;

        static ApiService()
        {
            _baseUrl = AppConfig.ApiBaseUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(AppConfig.RequestTimeout)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Define o token de autenticação para as próximas requisições
        /// </summary>
        /// <param name="token">Token JWT</param>
        public static void SetAuthToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Realiza uma requisição GET
        /// </summary>
        /// <typeparam name="T">Tipo do objeto retornado</typeparam>
        /// <param name="endpoint">Endpoint da API (ex: "/api/usuarios")</param>
        /// <returns>Objeto deserializado</returns>
        public static async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }
                else
                {
                    throw new HttpRequestException(
                        $"Erro na requisição: {response.StatusCode} - {content}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao comunicar com a API: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A requisição excedeu o tempo limite. Verifique sua conexão.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza uma requisição POST
        /// </summary>
        /// <typeparam name="TRequest">Tipo do objeto de requisição</typeparam>
        /// <typeparam name="TResponse">Tipo do objeto de resposta</typeparam>
        /// <param name="endpoint">Endpoint da API</param>
        /// <param name="data">Dados a serem enviados</param>
        /// <returns>Objeto de resposta deserializado</returns>
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent);
                }
                else
                {
                    throw new HttpRequestException(
                        $"Erro na requisição: {response.StatusCode} - {responseContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao comunicar com a API: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A requisição excedeu o tempo limite. Verifique sua conexão.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza uma requisição PUT
        /// </summary>
        /// <typeparam name="TRequest">Tipo do objeto de requisição</typeparam>
        /// <typeparam name="TResponse">Tipo do objeto de resposta</typeparam>
        /// <param name="endpoint">Endpoint da API</param>
        /// <param name="data">Dados a serem enviados</param>
        /// <returns>Objeto de resposta deserializado</returns>
        public static async Task<TResponse> PutAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent);
                }
                else
                {
                    throw new HttpRequestException(
                        $"Erro na requisição: {response.StatusCode} - {responseContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao comunicar com a API: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A requisição excedeu o tempo limite. Verifique sua conexão.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza uma requisição DELETE
        /// </summary>
        /// <param name="endpoint">Endpoint da API</param>
        /// <returns>True se deletado com sucesso</returns>
        public static async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao comunicar com a API: {ex.Message}", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A requisição excedeu o tempo limite. Verifique sua conexão.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a API está acessível
        /// </summary>
        /// <returns>True se a API responder</returns>
        public static async Task<bool> CheckApiConnectionAsync()
        {
            try
            {
                await _httpClient.GetAsync("/");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
