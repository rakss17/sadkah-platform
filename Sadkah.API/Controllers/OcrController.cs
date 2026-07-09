using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Sadkah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcrController : ControllerBase
    {
        private readonly IOcrService _ocrService;
        public OcrController(IOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [HttpPost("extract-receipt-text")]
        [EnableRateLimiting("api")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ExtractReceiptText(IFormFile file)
        {
            try
            {
                var text = await _ocrService.ExtractTextAsync(file);
                var amount = ReceiptParser.ExtractAmount(text);
                var referenceNumber = ReceiptParser.ExtractReferenceNumber(text);

                return Ok(ApiResponse<OcrResultDto>.SuccessResponse("Text extracted successfully.", new OcrResultDto { Text = text, Amount = amount, ReferenceNumber = referenceNumber }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while extracting text."));
            }
        }
    }
}
