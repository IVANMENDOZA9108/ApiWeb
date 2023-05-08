using System.ComponentModel.DataAnnotations;

namespace Crud.Models
{
    public class UsuarioModels
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(500)]
        public string Apellido { get; set; }
        [Required]
        public int Cedula { get; set; }
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }


    public class UsuarioActivo
    {

        public int id { get; set; }
        public string cedula { get; set; }
        public string nombreCompleto { get; set; }
        public bool estado { get; set; }
        public string alias { get; set; }
        public DateTime fechaCreacion { get; set; }

    }



}
