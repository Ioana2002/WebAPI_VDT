using WebAPI_VDT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_VDT.Context;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Exchange.WebServices.Data;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI_VDT.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly ApplicationContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        [Route("GetUserProfile")]
        public object GetUserProfile()
        {
            try
            {
                Guid UserId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                string userIdConverted = UserId.ToString();
                Profile profile = _context.Profile.FirstOrDefault(x => x.UserId == userIdConverted);

                if (profile != null)
                {
                    return Ok(new { profile });
                }
                else
                {
                    return Ok(new { });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "GetUserProfile", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetProfiles")]
        public object GetProfiles()
        {
            try
            {
                var profiles = _context.Profile.Select(row => row).ToList();
                List<object> exportProfiles = new List<object>();

                foreach (Profile profile in profiles)
                {
                    exportProfiles.Add(new
                    {
                        value = profile.ProfileId,
                        nume = profile.Nume,
                        prenume = profile.Prenume,
                        cnp = profile.CNP,
                        poza_buletin = profile.Poza_CI
                    });
                }
                return Ok(exportProfiles);

            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "GetProfiles", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetUserProfileById/{id}")]
        public object GetUserProfileById(string id)
        {
            try
            {
                Guid userGuid = new Guid(id);
                Profile profile = _context.Profile.FirstOrDefault(x => x.ProfileId == userGuid.ToString());

                if (profile != null)
                {
                    profile.ProfileId = null;
                    profile.UserId = null;
                    return Ok(new { profile });
                }
                else
                {
                    return BadRequest(String.Format("Nu am gasit user-ul cu id-ul: {0}", id));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "GetUserProfile", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetAccountInfo")]
        public async Task<object> GetAccountInfo()
        {
            try
            {
                Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
                return Ok(new
                {
                    username = user.UserName,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "GetAccountInfo", message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UploadProfile")]
        public async Task<IActionResult> UploadProfile([FromBody]Profile profile)
        {
            try
            {
                Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                Profile dbProfile = _context.Profile.FirstOrDefault(x => x.UserId == userId.ToString());
                if (dbProfile == null)
                {
                    profile.ProfileId = Guid.NewGuid().ToString();
                    profile.UserId = userId.ToString();
                    _context.Profile.Add(profile);

                    //var bodyBuilder = new BodyBuilder { HtmlBody = PopulateBody(profile.Nume, profile.Prenume, profile.CNP) };
                    //var body = bodyBuilder.ToMessageBody();
                }
                else
                {
                    profile.ProfileId = dbProfile.ProfileId;
                    profile.UserId = dbProfile.UserId;
                    profile.ID = dbProfile.ID;
                    _context.Entry(dbProfile).CurrentValues.SetValues(profile);
                }

                await _context.SaveChangesAsync();
                profile.UserId = null;
                profile.ProfileId = null;

                ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());

                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("User"))
                {
                    string role = "User";
                    await _userManager.AddToRoleAsync(user, role);
                }

                return Ok(profile);

            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "UploadProfile", message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture([FromBody]UploadProfilePicturePattern profilePictureData)
        {
            try
            {
                Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                ProfilePicture? dbProfilePicture = _context.ProfilePicture.FirstOrDefault(x => x.UserId == userId.ToString());
                if (dbProfilePicture == null)
                {
                    ProfilePicture cProfilePicture = new ProfilePicture();

                    cProfilePicture.ProfilePictureId = Guid.NewGuid().ToString();
                    cProfilePicture.UserId = userId.ToString();
                    cProfilePicture.Picture_URL = profilePictureData.Picture_URL;
                    _context.ProfilePicture.Add(cProfilePicture);
                }
                else
                {
                    ProfilePicture cProfilePicture = new ProfilePicture();

                    cProfilePicture.ProfilePictureId = dbProfilePicture.ProfilePictureId;
                    cProfilePicture.UserId = dbProfilePicture.UserId;
                    cProfilePicture.Picture_URL = profilePictureData.Picture_URL;

                    _context.Entry(dbProfilePicture).CurrentValues.SetValues(cProfilePicture);
                }

                await _context.SaveChangesAsync();

                return Ok(new { picture_url = profilePictureData.Picture_URL });

            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "UpdateProfilePicture", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetProfilePicture")]
        public object GetProfilePicture()
        {
            try
            {
                Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                ProfilePicture profilePicture = _context.ProfilePicture.FirstOrDefault(x => x.UserId == userId.ToString());
                if (profilePicture != null)
                {
                    return Ok(new { picture_url = profilePicture.Picture_URL });
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "ProfileController", method = "GetProfilePicture", message = ex.Message });
            }
        }

        public class UploadProfilePicturePattern
        {
            public string Picture_URL { get; set; }
        }

    }
}
