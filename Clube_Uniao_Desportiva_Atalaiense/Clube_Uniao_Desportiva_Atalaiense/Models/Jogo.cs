using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Jogo
    {
        public int JogoId { get; set; }

        public DateTime DataHora { get; set; }

        public int EquipaId { get; set; }

        [ForeignKey(nameof(EquipaId))]
        public Equipa Equipa { get; set; }

        public ICollection<JogadorJogo> JogadoresJogos { get; set; }
    }
}
