using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace JobPortal.Models
{
    public class Applicant
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // ✅ Do NOT validate ResumeUrl (set in controller)
        [BindNever]
        [ValidateNever]
        public string ResumeUrl { get; set; }

        [Required]
        public int JobPositionId { get; set; }

        // ✅ Do NOT validate navigation properties
        [BindNever]
        [ValidateNever]
        public JobPosition JobPosition { get; set; }

        // ✅ Not mapped to database, used only during form POST
        [NotMapped]
        [Required(ErrorMessage = "Resume file is required.")]
        public IFormFile ResumeFile { get; set; }
    }
}
