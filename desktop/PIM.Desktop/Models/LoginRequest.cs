namespace PIM.Desktop.Models
{
    /// <summary>
    /// Modelo de requisição para login
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
