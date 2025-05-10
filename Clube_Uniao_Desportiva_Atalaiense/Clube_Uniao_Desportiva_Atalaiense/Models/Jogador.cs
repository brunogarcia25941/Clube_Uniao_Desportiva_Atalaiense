using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Jogador
    {
        public int JogadorId { get; set; }

        [Required]
        public string Nome { get; set; }

        public int EquipaId { get; set; }

        [ForeignKey(nameof(EquipaId))]
        public Equipa Equipa { get; set; }

        public ICollection<JogadorJogo> JogadoresJogos { get; set; }
    }
}
