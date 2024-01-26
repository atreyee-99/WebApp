using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationProject.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FileName { get; set; }

        [Required(ErrorMessage = "No image selected. Please select an image.")]
        public byte[] ImageData { get; set; }
        public string? UserId { get; set; }

        internal static IDisposable FromStream(MemoryStream ms)
        {
            throw new NotImplementedException();
        }
    }
}
