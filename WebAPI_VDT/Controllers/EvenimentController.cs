using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
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
        [Authorize]
        [Route("PayTax/{id}")]
        public async Task<IActionResult> PayTax(string id)
        {
            try
            {
                var participantionDB = _context.Participation.FirstOrDefault(x => x.ParticipareId == new Guid(id));
                var result = participantionDB;
                if (participantionDB == null)
                {
                    return NotFound();
                }
                result.Taxa = "Achitata";
                _context.Entry(participantionDB).CurrentValues.SetValues(result);
                await _context.SaveChangesAsync();

                return Ok(string.Format("Taxa achitata pentru voluntarul: {0}",result.ParticipantNume));
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "PayTax", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetParticipants/{id}")]
        public object GetParticipants(string id)
        {
            try
            {
                var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == new Guid(id));
                if (eveniment != null)
                {

                    var participants = _context.Participation.Where(a => a.EvenimentGuid == eveniment.EvenimentId)
                                                             .Select(x => x)
                                                             .ToList();
                    List<object> result = new List<object>();

                    foreach (Participation participant in participants)
                    {
                        result.Add(new 
                        { Value = participant.ParticipantGuid, 
                          Nume = participant.ParticipantNume, 
                          Taxa = participant.Taxa, 
                          DataInscriere = participant.DataInscriere, 
                          Status = participant.Status, 
                          Telefon = participant.Telefon });
                    }
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetParticipants", message = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddRegister")]
        public async Task<Object> AddRegister(ParticipationRegisterPatter data)
        {
            try
            {
                Guid userId = new Guid(User.Claims.First(c => c.Type == "UserID").Value);
                Guid eventGUID = new Guid(data.Id);
                bool registeredBefore = _context.Participation.Any(x => (x.ParticipantGuid == userId) && x.EvenimentGuid == eventGUID);
                if (registeredBefore == false)
                {
                    var profile = _context.Profile.FirstOrDefault(x => x.UserId == userId.ToString());
                    if (profile != null)
                    {
                        var eveniment = _context.Event.FirstOrDefault(x => x.EvenimentId == eventGUID);
                        var userInfo = await _userManager.FindByIdAsync(userId.ToString());
                        Participation register = new Participation();

                        register.EvenimentGuid = eventGUID;
                        register.ParticipantGuid = new Guid(profile.ProfileId);
                        register.Telefon = profile.Telefon;
                        register.Email = userInfo.Email;
                        register.ParticipantNume = profile.Nume + " " + profile.Prenume;
                        register.DataInscriere = DateTime.Now;
                        register.Status = "Inscris";
                        register.Taxa = "Neachitata";
                        register.ParticipareId = Guid.NewGuid();
                        register.EvenimentId = eveniment.ID;
                        register.ParticipantId = profile.ID;

                        if (eveniment.TipEveniment == "Excursie")
                        {
                          
                        }
                        else if (eveniment.TipEveniment == "Caz Social")
                        {
                           
                        }
                        else if (eveniment.TipEveniment == "Activitate")
                        {
                          
                        }
                        _context.Participation.Add(register);
                    }
                    else
                    {
                        return BadRequest("Profilul nu se afla in baza de date");
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

        public class ParticipationRegisterPatter
        {
            public string Id { get; set; }
        }

        [HttpGet]
        [Authorize]
        [Route("GetParticipantsList/{eventId}")]
        public IActionResult GetParticipantsList(string eventId)
        {
            try
            {
                Guid eventGuid = new Guid(eventId);
                var participants = _context.Participation.Where(x => x.EvenimentGuid == eventGuid)
                                                        .Select(a => a)
                                                        .ToList();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Participanti");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Nume participant";
                    worksheet.Cell(currentRow, 2).Value = "Taxa";
                    worksheet.Cell(currentRow, 3).Value = "Data inscriere";
                    worksheet.Cell(currentRow, 4).Value = "Status";
                    worksheet.Cell(currentRow, 5).Value = "Telefon";
                    worksheet.Cell(currentRow, 6).Value = "Email";
                    foreach (var participare in participants)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = participare.ParticipantNume;
                        worksheet.Cell(currentRow, 2).Value = participare.Taxa;
                        worksheet.Cell(currentRow, 3).Value = participare.DataInscriere;
                        worksheet.Cell(currentRow, 4).Value = participare.Status;
                        worksheet.Cell(currentRow, 5).Value = participare.Telefon;
                        worksheet.Cell(currentRow, 6).Value = participare.Email;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Participanti.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { controller = "EvenimentController", method = "GetParticipantsList", message = ex.Message });
            }
        }

    }
}
