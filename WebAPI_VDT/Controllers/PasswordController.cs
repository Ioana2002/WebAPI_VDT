using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using WebAPI_VDT.Models;

namespace WebAPI_VDT.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PasswordController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public IConfiguration Configuration { get; }

        public PasswordController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            Configuration = configuration;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ApplicationUserModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    string token = model.Token;
                    token = token.Replace(" ", "+");
                    var updateResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    return (updateResult.Succeeded) ? Ok() : BadRequest("Token-ul a expirat, va rugam trimiteti din nou solicitarea pe email");
                }
                else
                {
                    return BadRequest("Nu s-a putut identifica user-ul pentru modificarea parolei");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ApplicationUserController", method = "FindUserByEmail", message = ex.Message });
            }
        }
    }
}
