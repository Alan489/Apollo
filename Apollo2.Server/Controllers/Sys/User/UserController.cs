using Apollo2.Server.Database;
using Apollo2.Shared.Sys.Authentication;
using Apollo2.Shared.Sys.User;
using Apollo2.Server.Sys.Obj;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Apollo2.Server.Services;

namespace Apollo2.Server.Controllers.Sys.User
{
 [ApiController]
 [Route("API/Sys/User/[controller]")]
 public class UserController : Controller
 {

  private UserDBContext _udbc;
  private Authentication _auth;

  public UserController(UserDBContext udbc, Authentication auth)
  {
   _udbc = udbc;
   _auth = auth;
  }

  // API/Sys/User/User/changePassword
  [HttpPost("changePassword/")]
  public async Task<IActionResult> changePassword(ChangePasswordRequest cpr)
  {

   UserLoginResponse ulr = new UserLoginResponse();

   Apollo2.Server.Sys.Obj.User? u = await _udbc.getUser(cpr.token.username);

   if (u == null)
   {
    ulr.session = null;
    ulr.successful = false;
    ulr.errorMessage = "Invalid username or password.";

    var resulting2 = new ObjectResult(ulr)
    {
     StatusCode = (int)HttpStatusCode.Unauthorized
    };

    return resulting2;
   }

   if (u.locked_out != 0)
   {
    ulr.session = null;
    ulr.successful = false;
    ulr.errorMessage = "Account locked. Please contact your system administrator.";

    var resulting2 = new ObjectResult(ulr)
    {
     StatusCode = (int)HttpStatusCode.Unauthorized
    };

    return resulting2;
   }

   ChangePasswordVerify validation = cpr.verify(u.password);
   if (!validation.Success)
   {

    ulr.successful = false;
    ulr.session = null;
    ulr.errorMessage = validation.errorMessage;

    var resulting1 = new ObjectResult(ulr)
    {
     StatusCode = (int)HttpStatusCode.Unauthorized
    };

    return resulting1;
   }

   await _udbc.setPassword(u.id, cpr.newHashword);

   ulr.successful = true;
   ulr.session = null;
   ulr.errorMessage = validation.errorMessage;

   var resulting = new ObjectResult(ulr)
   {
    StatusCode = (int)HttpStatusCode.OK
   };


   return resulting;
  }

  // API/Sys/User/User/get/umos
  // ACCESS LEVEL 8
  [HttpPost("get/umos")]
  public async Task<IActionResult> getUmos(UserSession us)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   var obre = new ObjectResult(await _udbc.getUmos());
   obre.StatusCode = (int)HttpStatusCode.OK;
   return obre;

  }

  // API/Sys/Users/Users/post/setpw/{myUser.id}/{hashword}
  // ACCESS LEVEL 8
  [HttpPost("post/setpw/{id}/{hashword}")]
  public async Task<IActionResult> setpw(UserSession us, int id, string hashword)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.setPassword(id, hashword, true);

   return Ok();

  }

  // API/Sys/Users/Users/post/lock/{myUser.id}/{myUser.locked_out}
  // ACCESS LEVEL 8
  [HttpPost("post/lock/{id}/{locked_out}")]
  public async Task<IActionResult> setlocked(UserSession us, int id, int locked_out)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.setLock(id, locked_out != 0);

   return Ok();

  }

  //API/Sys/Users/Users/post/expired/{myUser.id}/{myUser.change_password}
  // ACCESS LEVEL 8
  [HttpPost("post/expired/{id}/{expired}")]
  public async Task<IActionResult> setExpired(UserSession us, int id, int expired)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.setExpired(id, expired != 0);

   return Ok();

  }

  //API/Sys/Users/Users/create
  // ACCESS LEVEL 8
  [HttpPost("create")]
  public async Task<IActionResult> setExpired(CreateUser cu)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(cu.Session, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.createUser(cu.umo);

   return Ok();

  }

  //API/Sys/User/User/post/name/{id}/{name}
  // ACCESS LEVEL 8
  [HttpPost("post/name/{id}/{name}")]
  public async Task<IActionResult> setName(UserSession us, int id, string name)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.setUserName(id, name);

   return Ok();

  }

  //API/Sys/User/User/post/access/{id}/{access}
  // ACCESS LEVEL 8
  [HttpPost("post/access/{id}/{name}")]
  public async Task<IActionResult> setName(UserSession us, int id, int access)
  {
   AuthenticationResponse authResponse = await _auth.verifySession(us, 8);
   if (!authResponse.success)
    return Unauthorized();

   await _udbc.setUserAccess(id, access);

   return Ok();

  }


 }
}
