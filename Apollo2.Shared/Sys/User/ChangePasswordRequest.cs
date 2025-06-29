using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.User
{
 public class ChangePasswordRequest
 {
  public Token token {  get; set; }

  public DateTime timeStamp { get; set; }
  public string newHashword { get; set; }
  public string signature { get; set; }

  public ChangePasswordRequest() { }

  public ChangePasswordRequest(Token token, string newPassword)
  {
   this.token = token;
   timeStamp = DateTime.Now;

   newHashword = Crypt.getHash(newPassword + token.username);

   signature = Crypt.getHash(newHashword + token.hash + timeStamp.ToString("YY-MM-DD HH:mm:ss"));
  }

  public ChangePasswordVerify verify(string oldHashword)
  {
   TokenValidationResponse tvr = token.verify(oldHashword);
   ChangePasswordVerify cpv = new ChangePasswordVerify();
   if (!tvr.isValid)
   {
    cpv.Success = false;
    cpv.errorMessage = "Unable to verify identity: " + tvr.errorMessage;
    return cpv;
   }

   
   cpv.Success = true;
   return cpv;

  }

 }

 public class ChangePasswordVerify
 {
  public bool Success = false;
  public string errorMessage = "";
 }
}
