using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI_VDT.Context;
using WebAPI_VDT.Models;

namespace WebAPI_VDT.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EvenimentController : Controller
    {
        private readonly ApplicationContext _context;
        private UserManager<ApplicationUser> _userManager;

        public EvenimentController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetEvents")]
        public object GetEvents()
        {
            try
            {
                var events = _context.Event.Select(row => row).ToList();
                List<object> exportEvents = new List<object>();

                foreach (Event eventData in events)
                {
                    exportEvents.Add(new { Value = eventData.EvenimentId, Tip = eventData.TipEveniment, Name = eventData.Denumire, Location = eventData.Locatia });
                }
                return Ok(exportEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetEvents", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetParticipationById/{participationId}")]
        public object GetParticipationById(string participationId)
        {
            try
            {
                Guid guidParticipare = new Guid(participationId);
                var participare = _context.Participation.FirstOrDefault(x => x.ParticipareId == guidParticipare);
                if (participare != null)
                {
                    return Ok(participare);
                }
                return BadRequest("Participarea nu a fost identificata.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetParticipationById", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetEvent/{id}")]
        public object GetEvent(string id)
        {
            try
            {
                var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == new Guid(id));
                return (eveniment != null) ? Ok(eveniment) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetEvent", message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UploadEvent")]
        public async Task<Object> UploadEvent(Event eveniment)
        {
            try
            {
                var eventDB = _context.Event.FirstOrDefault(x => x.EvenimentId == eveniment.EvenimentId);
                if (eventDB == null)
                {
                    if (eveniment.TipEveniment == "Excursie")
                    {
                        var tripEvent = new Event();
                        tripEvent.TipEveniment = "Excursie";
                        tripEvent.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                        tripEvent.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                        tripEvent.Denumire = eveniment.Denumire;
                        tripEvent.Descriere = eveniment.Descriere;
                        tripEvent.Ora = eveniment.Ora;
                        tripEvent.Judet = eveniment.Judet;
                        tripEvent.Locatia = eveniment.Locatia;
                        tripEvent.Poster = eveniment.Poster;
                        tripEvent.EvenimentId = eveniment.EvenimentId;

                        _context.Event.Add(tripEvent);
                    }
                    else if (eveniment.TipEveniment == "Caz social")
                    {
                        var socialEvent = new Event();
                        socialEvent.TipEveniment = "Caz social";
                        socialEvent.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                        socialEvent.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                        socialEvent.Denumire = eveniment.Denumire;
                        socialEvent.Descriere = eveniment.Descriere;
                        socialEvent.Ora = eveniment.Ora;
                        socialEvent.Judet = eveniment.Judet;
                        socialEvent.Locatia = eveniment.Locatia;
                        socialEvent.Poster = eveniment.Poster;
                        socialEvent.EvenimentId = eveniment.EvenimentId;

                        _context.Event.Add(socialEvent);
                    }
                    else if (eveniment.TipEveniment == "Activitate")
                    {
                        var activityEvent = new Event();
                        activityEvent.TipEveniment = "Caz social";
                        activityEvent.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                        activityEvent.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                        activityEvent.Denumire = eveniment.Denumire;
                        activityEvent.Descriere = eveniment.Descriere;
                        activityEvent.Ora = eveniment.Ora;
                        activityEvent.Judet = eveniment.Judet;
                        activityEvent.Locatia = eveniment.Locatia;
                        activityEvent.Poster = eveniment.Poster;
                        activityEvent.EvenimentId = eveniment.EvenimentId;

                        _context.Event.Add(activityEvent);
                    }
                    eveniment.EvenimentId = eveniment.EvenimentId;
                    eveniment.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                    eveniment.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                    _context.Event.Add(eveniment);
                }
                else
                {
                    eveniment.ID = eventDB.ID;
                    if (eveniment.Data_Inceput != eventDB.Data_Inceput ||
                        eveniment.Data_Sfarsit != eventDB.Data_Sfarsit)
                    {
                        eveniment.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                        eveniment.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                    }

                    _context.Entry(eventDB).CurrentValues.SetValues(eveniment);
                }

                await _context.SaveChangesAsync();
                eveniment.ID = 0;
                eveniment.EvenimentId = null;

                return Ok(new { succeeded = true, eveniment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "UploadEvent", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetInscriere/{eventid}")]
        public async Task<IActionResult> GetInscriere(string eventid)
        {
            try
            {
                var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == new Guid(eventid));
                if (eveniment != null)
                {
                    Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                    var userProfile = _context.Profile.FirstOrDefault(x => x.UserId == userId.ToString());

                    var participare = _context.Participation.FirstOrDefault(x => (x.ParticipantGuid.ToString() == userProfile.ProfileId) && x.EvenimentGuid == new Guid(eventid));

                    return Ok(participare);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetInscriere", message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("DeleteParticipation/{id}")]
        public async Task<IActionResult> DeleteParticipation(string id)
        {
            try
            {
                var participare = _context.Participation.FirstOrDefault(x => x.ParticipareId == new Guid(id));

                if (participare == null)
                {
                    return NotFound();
                }
                _context.Participation.Remove(participare);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "DeleteParticipation", message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            try
            {
                var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == new Guid(id));

                if (eveniment == null)
                {
                    return NotFound();
                }
                _context.Event.Remove(eveniment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "DeleteEvent", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetParticipants")]
        public object GetParticipants()
        {
            try
            {
                var profiles = _context.Profile.Select(row => row).ToList();
                List<Profile> result = new List<Profile>();

                foreach (Profile profile in profiles)
                {
                    result.Add(new Profile { Nume = profile.Nume, Prenume = profile.Prenume, ProfileId = profile.ProfileId });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetParticipants", message = ex.Message });
            }
        }
    }
}
