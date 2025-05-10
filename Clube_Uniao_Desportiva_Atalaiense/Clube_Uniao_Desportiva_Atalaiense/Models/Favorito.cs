using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Favorito
    {
        [Key, Column(Order = 0)]
        public string UtilizadorUsername { get; set; }

        [Key, Column(Order = 1)]
        public int EquipaId { get; set; }

        [ForeignKey(nameof(UtilizadorUsername))]
        public Utilizador Utilizador { get; set; }

        [ForeignKey(nameof(EquipaId))]
        public Equipa Equipa { get; set; }
    }
}
