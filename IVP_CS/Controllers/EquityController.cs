using CS_Console.Model;
using CS_Console.EquityRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

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
            try
            {
                List<EditEquityModel> result = _security.GetSecurityData();
                return Ok(result);
            }
            catch(DbException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"DB ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error Retrieving Equities: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostData(IFormFile file)
        {
            try
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
                    await file.CopyToAsync(fileStream);
                }

                await _security.ImportDataFromCsv(filePath);
                return Ok();
            }
            catch (DbException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"DB ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error Uploading Equities: {ex.Message}");
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> PutData([FromBody] EditEquityModel esm)
        {
            try
            {
                await _security.UpdateSecurityData(esm);
                return Ok();
            }
            catch (DbException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"DB ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error Updating Equities: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData([FromRoute] int id)
        {
            try
            {
                await _security?.DeleteSecurityData(id);
                return Ok();
            }
            catch (DbException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"DB ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error Retrieving Equities: {ex.Message}");
            }
        }
    }
}
