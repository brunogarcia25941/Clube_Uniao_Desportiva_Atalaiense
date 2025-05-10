using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Loja
    {
        public int LojaId { get; set; }

        [Required]
        public string NomeProduto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        public string Descricao { get; set; }

        public string ImagemUrl { get; set; }
    }
}
