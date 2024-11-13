using CS_Console.Model;
using CS_Console.EquityRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IVP_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquityController : ControllerBase
    {
        IEquity _security;
        public EquityController(IEquity security)
        {
            _security = security;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            List<EditEquityModel> result = _security.GetSecurityData();
            return Ok(result);
        }

        [HttpPost("upload")]
        public IActionResult PostData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Set the directory where the file will be saved
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            // Ensure the directory exists
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }       

            _security.ImportDataFromCsv(filePath);
            return Ok();
        }

        [HttpPut("edit")]
        public IActionResult PutData([FromBody] EditEquityModel esm)
        {
            _security.UpdateSecurityData(esm);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteData([FromRoute] int id)
        {
            _security?.DeleteSecurityData(id);
            return Ok();
        }
    }
}
