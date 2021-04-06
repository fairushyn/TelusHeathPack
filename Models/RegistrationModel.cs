using System.ComponentModel.DataAnnotations;

namespace TelusHeathPack.Models
{
    public class RegistrationModel
    {
        [Required]
        public string Alias { get; set; }
    }
}