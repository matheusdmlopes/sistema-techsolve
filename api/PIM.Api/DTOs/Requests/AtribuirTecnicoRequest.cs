namespace PIM.Api.DTOs.Requests
{
    public class AtribuirTecnicoRequest
    {
        /// <summary>
        /// ID do técnico. Se null, remove a atribuição do técnico
        /// </summary>
        public int? IdUsuarioTecnico { get; set; }
    }
}
