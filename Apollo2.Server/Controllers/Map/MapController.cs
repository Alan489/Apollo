using Apollo2.Server.Database;
using Apollo2.Shared.Sys.Authentication;
using Apollo2.Shared.Sys.User;
using Apollo2.Server.Sys.Obj;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Apollo2.Server.Services;
using Apollo2.Shared.Sys.Data.Map;

namespace Apollo2.Server.Controllers.Map
{
 [ApiController]
 //API/Map/Map
 [Route("API/Map/[controller]")]
 public class MapController : Controller
 {

  private POIDBContext _pdbc;
  private Authentication _auth;

  public MapController(POIDBContext pdbc, Authentication auth)
  {
   _auth = auth;
   _pdbc = pdbc;
  }

  // API/Map/Map/get/poi
  // Access Level 0
  [HttpPost("get/poi")]
  public async Task<IActionResult> getPOIs(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess, 0);

   if (ar.success == false)
    return Unauthorized();


   return new ObjectResult(await _pdbc.getPOIs());
  }

  // API/Map/Map/get/cat
  // Access Level 0
  [HttpPost("get/cat")]
  public async Task<IActionResult> getCats(UserSession sess)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess, 0);

   if (ar.success == false)
    return Unauthorized();


   return new ObjectResult(await _pdbc.getCats());
  }

  // API/Map/Map/delete/poi/{id}
  // Access Level 5
  [HttpPost("delete/poi/{id}")]
  public async Task<IActionResult> deletePOI(UserSession sess, int id)
  {
   AuthenticationResponse ar = await _auth.verifySession(sess, 5);

   if (ar.success == false)
    return Unauthorized();

   await _pdbc.deletePOI(id);

   return Ok();
  }

  // API/Map/Map/create/poi
  // Access Level 5
  [HttpPost("create/poi")]
  public async Task<IActionResult> createPOI(CreatePOIRequest cpr)
  {
   AuthenticationResponse ar = await _auth.verifySession(cpr.session, 5);

   if (ar.success == false)
    return Unauthorized();

   await _pdbc.createPOI(cpr.POI);

   return Ok();
  }



 }
}
