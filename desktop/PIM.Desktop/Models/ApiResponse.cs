namespace PIM.Desktop.Models
{
    /// <summary>
    /// Resposta padrão da API para encapsular resultados
    /// </summary>
    /// <typeparam name="T">Tipo do dado retornado</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
