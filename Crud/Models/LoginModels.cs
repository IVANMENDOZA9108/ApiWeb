using System.ComponentModel.DataAnnotations;

namespace Crud.Models
{
    public class LoginModels
    {
        [Required]
        public int Cedula { get; set; }
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; }
     
    }

    public class TokenViewModel
    {
        public string Token { get; set; }

    }
}
