using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Equipa
    {
        public int EquipaId { get; set; }

        [Required]
        public string Nome { get; set; }

        public ICollection<Jogador> Jogadores { get; set; }

        public ICollection<Jogo> Jogos { get; set; }

        public ICollection<Favorito> Favoritos { get; set; }
    }
}
