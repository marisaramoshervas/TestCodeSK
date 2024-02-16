using System.ComponentModel.DataAnnotations;

namespace TestExerciseSK.DTO
{
    public class UserDTO
    {
        [Key]
        public string userID { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? birthDate { get; set; }
    }
}
