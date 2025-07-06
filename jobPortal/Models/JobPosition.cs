using JobPortal.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace JobPortal.Models
{
    public class JobPosition
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Range(1, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
        public decimal Salary { get; set; }

        [BindNever]
        [ValidateNever]
        public ICollection<Applicant> Applicants { get; set; }
    }

}