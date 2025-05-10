using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class Utilizador
    {
        [Key]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Favorito> Favoritos { get; set; }
    }
}
