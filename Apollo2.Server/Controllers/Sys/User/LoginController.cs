using Apollo2.Server.Database;
using Apollo2.Shared.Sys.Authentication;
using Apollo2.Shared.Sys.User;
using Apollo2.Server.Sys.Obj;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Apollo2.Server.Controllers.Sys.User
{
 [ApiController]
 [Route("API/Sys/User/[controller]")]
 public class LoginController : Controller
 {

  private UserDBContext _udbc;

  public LoginController(UserDBContext udbc) 
  { 
   _udbc = udbc;
  }


  [HttpPost]
  public async Task<IActionResult> tryLogin(Token t)
  {

   UserLoginResponse ulr = new UserLoginResponse();

   Apollo2.Server.Sys.Obj.User? u = await _udbc.getUser(t.username);

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

   TokenValidationResponse validation = t.verify(u.password);
   if (validation.isValid)
   {
    ulr.session = new UserSession();
    ulr.session.token = t;
    ulr.session.AccessLevel = u.access_level;
    ulr.session.Id = u.id;
    ulr.session.Username = u.username;
    ulr.session.Name = u.name;
    ulr.session.sysName = Program.sysName;
    ulr.session.sign(Program.mySign);

    ulr.successful = true;

    if (u.change_password != 0)
     ulr.errorMessage = "CHANGEPASSWORD";

    var resulting1 = new ObjectResult(ulr)
    {
     StatusCode = (int)HttpStatusCode.OK
    };

    return resulting1;
   }


   ulr.successful = false;
   ulr.session = null;
   ulr.errorMessage = validation.errorMessage;

   var resulting = new ObjectResult(ulr)
   {
    StatusCode = (int)HttpStatusCode.Unauthorized
   };


   return resulting;
  }
 }
}
