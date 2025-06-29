using Apollo2.Server.Database;
using Apollo2.Shared.Sys.Authentication;
using Apollo2.Shared.Sys.User;
using Apollo2.Server.Sys.Obj;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Apollo2.Server.Services;
using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;

namespace Apollo2.Server.Controllers.Units
{
 [ApiController]
 [Route("API/Units/[controller]")]
 public class UnitsController : Controller
 {

  private IncidentsDbContext _idbc;
  private UnitDBContext _udbc;
  private Authentication _auth;

  public UnitsController(IncidentsDbContext idbc, UnitDBContext udbc, Authentication auth)
  {
   _auth = auth;
   _idbc = idbc;
   _udbc = udbc;
  }

  // API/Units/Units/get/list
  [HttpPost("get/list")]
  public async Task<IActionResult> getList(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   List<UnitGlance>? ug = await _udbc.getUnitList();

   if (ug == null)
    return BadRequest();

   ObjectResult or = new ObjectResult(ug);

   or.StatusCode = 200;

   return or;
  }


  // API/Units/Units/get/attachment/{inc}
  [HttpPost("get/attachment/{inc}")]
  public async Task<IActionResult> getAttachments(UserSession sess, int inc)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   List<UnitAttachment>? ug = await _udbc.getUnitAttachments(inc);

   if (ug == null)
    return BadRequest();

   ObjectResult or = new ObjectResult(ug);

   or.StatusCode = 200;

   return or;
  }

  // API/Units/Units/post/attachment/{inc}/{val}
  [HttpPost("post/attachment/{inc}/{val}")]
  public async Task<IActionResult> postAttachment(UserSession sess, int inc, string val)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   await _udbc.updateAttachment(inc, val);

   string stat = "";

   switch (val)
   {
    case "arrived":
     stat += "On Scene";
     break;

    case "transport":
     stat += "Transporting";
     break;

    case "transportdone":
     stat += "Arrived";
     break;

    case "cleared":
     stat += "In Service";
     break;
    default:
     break;
   }

   int incident = await _udbc.getIncidentFromAttachment(inc);
   string u = await _udbc.getUnitFromAttachment(inc);

   await LogDBContext.incidentLog(incident, stat, sess.token.username, u);

   return Ok();
  }

  // API/Units/Units/new/attachment/{inc}/{unit}
  [HttpPost("new/attachment/{inc}/{unit}")]
  public async Task<IActionResult> newAttachment(UserSession sess, int inc, string unit)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   await _udbc.attachUnit(inc, unit);

   int incident = await _udbc.getIncidentFromAttachment(inc);

   await LogDBContext.incidentLog(inc, "Dispatched", sess.token.username, unit);

   return Ok();
  }


  //Status:
  //0: OOS
  //1: BUSY
  //2: IS
  // API/Units/Units/post/status/{unit}/{status}
  [HttpPost("post/status/{unit}/{status}")]
  public async Task<IActionResult> setStatus(UserSession sess, int status, string unit)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();

   string stat = "";

   switch(status)
   {
    case 2:
     stat = "In Service";
     break;
    case 1:
     stat = "Busy";
     break;
    default:
     stat = "Out Of Service";
     break;
   }

   await _udbc.setStatus(unit, stat);


   return Ok();
  }

  // API/Units/Units/get/{unit}
  [HttpPost("get/{unit}")]
  public async Task<IActionResult> getUnit(UserSession sess, string unit)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();


   Unit? u = await _udbc.getUnit(unit);

   if (u == null)
    return NotFound();

   ObjectResult op = new ObjectResult(u);
   op.StatusCode = 200;

   return op;
  }

  // API/Units/Units/roles/
  [HttpPost("roles")]
  public async Task<IActionResult> getRoles(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess);

   if (ar.success == false)
    return Unauthorized();


   List<string>? u = await _udbc.getRoles();

   if (u == null)
    return NotFound();

   ObjectResult op = new ObjectResult(u);
   op.StatusCode = 200;

   return op;
  }

  // API/Units/Units/update/
  [HttpPost("update")]
  public async Task<IActionResult> update(UnitUpdateRequest uur)
  {
   AuthenticationResponse ar = await _auth.verifySession(uur.session);

   if (ar.success == false)
    return Unauthorized();

   if ((await _udbc.getUnit(uur.unit.unit)).status == "Attached to Incident")
    uur.unit.status = "Attached to Incident";

   await _udbc.updateUnit(uur.unit);

   return Ok();
  }

  // API/Units/Units/new/
  [HttpPost("new")]
  public async Task<IActionResult> newUnit(UnitUpdateRequest uur)
  {
   AuthenticationResponse ar = await _auth.verifySession(uur.session);

   if (ar.success == false)
    return Unauthorized();

   if ((await _udbc.getUnit(uur.unit.unit)) != null)
    return BadRequest();

   await _udbc.newUnit(uur.unit);

   return Ok();
  }

 }
}
