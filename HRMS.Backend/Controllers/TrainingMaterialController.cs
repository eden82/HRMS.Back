using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingMaterialController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TrainingMaterialController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/training-materials
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] CreateTrainingMaterialDto dto)
        {
            // Validate Program
            var program = await _context.TrainingPrograms
                .FirstOrDefaultAsync(p => p.Id == dto.ProgramId);

            if (program == null)
                return NotFound("Training program not found.");

            string? filePath = null;
            string? fileName = null;

            // Handle file upload
            if (dto.ContractFile != null)
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads", "TrainingMaterials");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                fileName = $"{Guid.NewGuid()}_{dto.ContractFile.FileName}";
                filePath = Path.Combine(uploadsFolder, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await dto.ContractFile.CopyToAsync(fileStream);
            }

            // Create TrainingMaterial entry
            var material = new TrainingMaterial
            {
                ProgramId = dto.ProgramId,
                DocumentOrVideoLink = dto.DocumentOrVideoLink,
                ContractFileName = fileName,
                ContractFilePath = filePath
            };

            _context.TrainingMaterials.Add(material);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Training material uploaded successfully.",
                material.Id,
                material.DocumentOrVideoLink,
                material.ContractFileName
            });
        }

        // GET all materials for a program
        [HttpGet("program/{programId}")]
        public async Task<IActionResult> GetByProgram(Guid programId)
        {
            var materials = await _context.TrainingMaterials
                .Where(m => m.ProgramId == programId)
                .Select(m => new
                {
                    m.Id,
                    m.DocumentOrVideoLink,
                    m.ContractFileName,
                    m.ContractFilePath,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(materials);
        }
    }
}
