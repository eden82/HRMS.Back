using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HRMS.Backend.DTOs
{
    public class CreateTrainingMaterialDto
    {
        [Required]
        public Guid ProgramId { get; set; }

        [MaxLength(1000)]
        public string? DocumentOrVideoLink { get; set; }

        public IFormFile? ContractFile { get; set; } // uploaded file
    }
}
