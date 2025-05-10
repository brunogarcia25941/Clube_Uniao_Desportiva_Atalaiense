using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class JogadorJogo
    {
        [Key, Column(Order = 0)]
        public int JogadorId { get; set; }

        [Key, Column(Order = 1)]
        public int JogoId { get; set; }

        public int Golos { get; set; }

        public int Assistencias { get; set; }

        [ForeignKey(nameof(JogadorId))]
        public Jogador Jogador { get; set; }

        [ForeignKey(nameof(JogoId))]
        public Jogo Jogo { get; set; }
    }
}
