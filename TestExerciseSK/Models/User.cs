using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;

namespace TestExerciseSK.Models
{
    public class User
    {
        [Key]
        public string userID { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        //public DateOnly birthDate { get; set; }
        //public DateOnly retirementDate { get; set; }

        public DateTime birthDate { get; set; }

        public DateTime retirementDate { get; set; }
    }
}
