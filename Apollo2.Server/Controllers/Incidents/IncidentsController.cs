using Apollo2.Server.Database;
using Apollo2.Shared.Sys.Authentication;
using Apollo2.Shared.Sys.User;
using Apollo2.Server.Sys.Obj;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Apollo2.Server.Services;
using Apollo2.Shared.Sys.Data.Incidents;

namespace Apollo2.Server.Controllers.Incidents
{
 [ApiController]
 [Route("API/Incidents/[controller]")]
 public class IncidentsController : Controller
 {

  private IncidentsDbContext _idbc;
  private Authentication _auth;

  public IncidentsController(IncidentsDbContext idbc, Authentication auth)
  {
   _auth = auth;
   _idbc = idbc;
  }

  // API/Incidents/Incidents/get/active
  [HttpPost("get/active")]
  public async Task<IActionResult> getIncidents(UserSession sess)
  {
   GetIncidentResponse gir = new GetIncidentResponse();
   ObjectResult resulting = new ObjectResult(gir);

   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
   {
    gir.success = false;
    gir.errorMessage = ar.errorMessage;
    resulting.StatusCode = (int)HttpStatusCode.Unauthorized;
    return resulting;
   }

   gir.success = true;
   gir.Incidents = await _idbc.getCurrentIncidents();


   return resulting;
  }

  // API/Incidents/Incidents/post/save
  [HttpPost("post/save")]
  public async Task<IActionResult> saveIncident(SaveIncidentRequest sir)
  {

   AuthenticationResponse ar = await _auth.verifySession(sir.session);

   if (ar.success == false)
   {
    return Unauthorized();
   }

   await _idbc.saveIncident(sir.incident);

   if (sir.incident.ts_complete != null && sir.incident.ts_complete > DateTime.Now.AddYears(-250) && !string.IsNullOrEmpty(sir.incident.disposition) && sir.incident.disposition != "Not Selected")
    await LogDBContext.incidentLog(sir.incident.incident_id, "CLOSED INCIDENT", sir.session.token.username);
   else
    await LogDBContext.incidentLog(sir.incident.incident_id, "UPDATED INCIDENT", sir.session.token.username);

   return Ok();
  }


  // API/Incidents/Incidents/post/new
  [HttpPost("post/new")]
  public async Task<IActionResult> newIncident(SaveIncidentRequest sir)
  {

   AuthenticationResponse ar = await _auth.verifySession(sir.session);

   if (ar.success == false)
   {
    return Unauthorized();
   }

   await _idbc.newIncident(sir.incident);

   await LogDBContext.incidentLog(sir.incident.incident_id, "OPENED INCIDENT", sir.session.token.username);

   ObjectResult or = new ObjectResult(sir.incident);
   or.StatusCode = 200;

   return or;
  }


  // API/Incidents/Incidents/get/inctypes
  [HttpPost("get/inctypes")]
  public async Task<IActionResult> getIncidentTypes(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   List<string>? types = await _idbc.getIncidentTypes();

   if (types == null)
    return BadRequest();



   ObjectResult resulting = new ObjectResult(types);
   resulting.StatusCode = (int)HttpStatusCode.OK;
   return resulting;
  }

  // API/Incidents/Incidents/get/dispotypes
  [HttpPost("get/dispotypes")]
  public async Task<IActionResult> getIncidentDispoTypes(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   List<string>? types = await _idbc.getIncidentDispoTypes();

   if (types == null)
    return BadRequest();



   ObjectResult resulting = new ObjectResult(types);
   resulting.StatusCode = (int)HttpStatusCode.OK;
   return resulting;
  }

  // API/Incidents/Incidents/get/incnotes/{inc}
  [HttpPost("get/incnotes/{inc}")]
  public async Task<IActionResult> getIncidentDispoTypes(UserSession sess, int inc)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   List<IncidentNote> notes = await LogDBContext.getIncNotes(inc);

   ObjectResult resulting = new ObjectResult(notes);
   resulting.StatusCode = (int)HttpStatusCode.OK;
   return resulting;
  }

  // API/Incidents/Incidents/post/incnotes/{inc}
  [HttpPost("post/incnotes/{inc}")]
  public async Task<IActionResult> postIncident(IncidentNotePost inp, int inc)
  {
   AuthenticationResponse ar = await _auth.verifySession(inp.sess);

   if (ar.success == false)
    return Unauthorized();

   await LogDBContext.incidentLog(inc, inp.message, inp.sess.token.username, inp.unit);

   return Ok();
  }

  // API/Incidents/Incidents/get/archive/byday/{date(MM.dd.yyyy)}
  [HttpPost("get/archive/byday/{date}")]
  public async Task<IActionResult> getArchiveByDay(UserSession sess, string date)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   string[] broke = date.Split('.');
   if (broke.Length != 3)
    return BadRequest();

   int[] ints = new int[broke.Length];

   for (int i = 0; i < ints.Length; i++)
    if (!int.TryParse(broke[i], out ints[i]))
     return BadRequest();

   ObjectResult obj = new ObjectResult(await _idbc.getIncidentsByDay(new DateTime(ints[2], ints[0], ints[1])));
   obj.StatusCode = 200;

   return obj;
  }

  //API/Incidents/Incidents/get/archive/byinc/{incidentNum}
  [HttpPost("get/archive/byinc/{incidentNum}")]
  public async Task<IActionResult> getArchiveByInc(UserSession sess, string incidentNum)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   ObjectResult obj = new ObjectResult(await _idbc.getIncidentsByInc(incidentNum));
   obj.StatusCode = 200;

   return obj;
  }

  //API/Incidents/Incidents/post/archive/reopen/{incidentID}
  //ACCESS LEVEL 5
  [HttpPost("post/archive/reopen/{inc}")]
  public async Task<IActionResult> reopenInc(UserSession sess, int inc)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess, 5);

   if (ar.success == false)
    return Unauthorized();

   Incident incObj = await _idbc.getIncidentByIncId(inc) ?? new Incident();

   await LogDBContext.incidentLog(inc, $"[INCIDENT REOPENED] BY {sess.Name}({sess.Username}) PREVIOUSLY DISPOSITIONED AS ({incObj.disposition})");

   await _idbc.reopenInc(inc);

   return Ok();
  }

 }
}
