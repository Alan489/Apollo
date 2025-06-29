using Apollo2.Server.Database;
using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.User;

namespace Apollo2.Server.Services
{
 public class Authentication
 {
  private UserDBContext _user {  get; set; }

  public Authentication(UserDBContext user)
  {
   _user = user;
  }

  public async Task<AuthenticationResponse> verifySession(UserSession session, int accessLevel = 1)
  {
   User? u = await _user.getUser(session.token.username);
   AuthenticationResponse response = new AuthenticationResponse();

   if (u == null)
   {
    response.success = false;
    response.errorMessage = "User does not exist.";
    return response;
   }

   if (u.access_level < accessLevel)
   {
    response.success = false;
    response.errorMessage = "User does not have clearance.";
    return response;
   }

   if (u.locked_out != 0)
   {
    response.success = false;
    response.errorMessage = "User is locked.";
    return response;
   }

   TokenValidationResponse tvr = session.token.verify(u.password);

   if (!tvr.isValid)
   {
    response.success = false;
    response.errorMessage = "Failed to validate user token: " + tvr.errorMessage;
    return response;
   }

   if (!session.verifysignature(Program.mySign))
   {
    response.success = false;
    response.errorMessage = "Could not validate user session signature.";
    return response;
   }

   response.success = true;

   return response;



  }

 }

 public class AuthenticationResponse
 {
  public bool success = false;
  public string? errorMessage;
 }
}
