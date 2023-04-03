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
                        tripEvent.EvenimentId = Guid.NewGuid();

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
                        socialEvent.EvenimentId = Guid.NewGuid(); ;

                        _context.Event.Add(socialEvent);
                    }
                    else if (eveniment.TipEveniment == "Activitate")
                    {
                        var activityEvent = new Event();
                        activityEvent.TipEveniment = "Activitate";
                        activityEvent.Data_Inceput = eveniment.Data_Inceput.ToLocalTime();
                        activityEvent.Data_Sfarsit = eveniment.Data_Sfarsit.ToLocalTime();
                        activityEvent.Denumire = eveniment.Denumire;
                        activityEvent.Descriere = eveniment.Descriere;
                        activityEvent.Ora = eveniment.Ora;
                        activityEvent.Judet = eveniment.Judet;
                        activityEvent.Locatia = eveniment.Locatia;
                        activityEvent.Poster = eveniment.Poster;
                        activityEvent.EvenimentId = Guid.NewGuid();

                        _context.Event.Add(activityEvent);
                    }
                    eveniment.EvenimentId = Guid.NewGuid();
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

        [HttpPost]
        [Route("AddRegister")]
        public async Task<Object> AddParticipare(Participation register)
        {
            try
            {
                string userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value).ToString();
                bool registeredBefore = _context.Participation.Any(x => (x.ParticipantGuid == register.ParticipantGuid) && x.EvenimentGuid == register.EvenimentGuid);
                if (registeredBefore == false)
                {
                    var participant = _context.Profile.FirstOrDefault(x => x.ProfileId == register.ParticipantGuid.ToString());
                    var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == register.EvenimentGuid);

                    if (participant == null)
                    {
                        return BadRequest("User-ul selectat nu se afla in sistem.");
                    }
                    var accountParticipant = _context.Users.FirstOrDefault(x => x.Id == participant.UserId.ToString());

                    if (userId != accountParticipant.Id)
                    {
                        return BadRequest("Nu puteti inscrie alti participanti.");
                    }

                    if (participant == null)
                    {
                        return BadRequest("Participantul trebuie sa isi completeze datele pentru a va putea inscrie.");
                    }

                    register.Telefon = participant.Telefon;
                    register.Email = accountParticipant.Email;
                    register.ParticipantNume = participant.Nume + " " + participant.Prenume;
                    register.DataInscriere = DateTime.Now;
                    register.Status = "Inscris";
                    register.ParticipareId = Guid.NewGuid();
                    _context.Participation.Add(register);

                    if (eveniment.TipEveniment == "Excursie")
                    {
                        var tripRegister = new Participation();
                        // tripRegister.EvenimentId = eveniment.Trial_GUID;
                        tripRegister.ParticipantGuid = register.ParticipantGuid;
                        tripRegister.Telefon = register.Telefon;
                        tripRegister.Email = register.Email;
                        tripRegister.ParticipantNume = register.ParticipantNume;
                        tripRegister.DataInscriere = register.DataInscriere;
                        tripRegister.Status = register.Status;
                        tripRegister.ParticipareId = Guid.NewGuid();

                        _context.Participation.Add(tripRegister);
                    }else if(eveniment.TipEveniment == "Caz Social")
                    {
                        var socialRegister = new Participation();
                        // tripRegister.EvenimentId = eveniment.Trial_GUID;
                        socialRegister.ParticipantGuid = register.ParticipantGuid;
                        socialRegister.Telefon = register.Telefon;
                        socialRegister.Email = register.Email;
                        socialRegister.ParticipantNume = register.ParticipantNume;
                        socialRegister.DataInscriere = register.DataInscriere;
                        socialRegister.Status = register.Status;
                        socialRegister.ParticipareId = Guid.NewGuid();
                    }
                    else if (eveniment.TipEveniment == "Activitate")
                    {
                        var activityRegister = new Participation();
                        // tripRegister.EvenimentId = eveniment.Trial_GUID;
                        activityRegister.ParticipantGuid = register.ParticipantGuid;
                        activityRegister.Telefon = register.Telefon;
                        activityRegister.Email = register.Email;
                        activityRegister.ParticipantNume = register.ParticipantNume;
                        activityRegister.DataInscriere = register.DataInscriere;
                        activityRegister.Status = register.Status;
                        activityRegister.ParticipareId = Guid.NewGuid();
                    }
                }
                else
                {
                    return BadRequest("Participantul este deja inscris la eveniment.");
                }

                await _context.SaveChangesAsync();
                return Ok(new { succeeded = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "AddRegister", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetEventParticipants/{id}")]
        public object GetEventParticipants(string id)
        {
            try
            {
                var registers = _context.Participation.Where(x => x.EvenimentGuid == new Guid(id))
                                                    .Select(a => a)
                                                    .OrderBy(a => a.Status)
                                                    .ToList();
                return (registers.Count == 0) ? null : Ok(registers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetEventParticipants", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPozeParticipanti/{participareId}")]
        public object GetPozeParticipanti(string participareId)
        {
            try
            {
                Guid GuidParticipare = new Guid(participareId);
                var participare = _context.Participation.FirstOrDefault(x => x.ParticipareId == GuidParticipare);

                if (participare != null)
                {
                    var participant = _context.Profile.FirstOrDefault(x => x.ProfileId == participare.ParticipantGuid.ToString());

                    return Ok(new
                    {
                        poza_CI_Participant = participant.Poza_CI
                    });
                }
                else
                {
                    return BadRequest("Nu am putut identifica participarea.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "UtileController", method = "GetPozeParticipanti", message = ex.Message });
            }
        }

    }
}
