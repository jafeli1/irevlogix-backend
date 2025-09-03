using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using irevlogix_backend.Services;
using irevlogix_backend.DTOs;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowFrontend")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IEmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponse>> SendContactEmail([FromBody] ContactRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new ContactResponse 
                    { 
                        Success = false, 
                        Message = string.Join("; ", errors) 
                    });
                }

                var toEmail = !string.IsNullOrEmpty(request.Recipient) ? request.Recipient : "support@irevlogix.com";
                
                var emailSent = await _emailService.SendContactEmailAsync(
                    request.Name, 
                    request.Email, 
                    request.Company, 
                    request.Message, 
                    toEmail
                );

                if (emailSent)
                {
                    return Ok(new ContactResponse { Success = true, Message = "Message sent successfully" });
                }
                else
                {
                    return StatusCode(500, new ContactResponse { Success = false, Message = "Failed to send message" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission");
                return StatusCode(500, new ContactResponse { Success = false, Message = "Internal server error" });
            }
        }
    }
}
