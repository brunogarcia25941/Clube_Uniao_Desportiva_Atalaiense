using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Favorito
    {
        [Key, Column(Order = 0)]
        public string UtilizadorId { get; set; }

        [Key, Column(Order = 1)]
        public int EquipaId { get; set; }

        [ForeignKey(nameof(UtilizadorId))]
        public IdentityUser Utilizador { get; set; }

        [ForeignKey(nameof(EquipaId))]
        public Equipa Equipa { get; set; }
    }
}
