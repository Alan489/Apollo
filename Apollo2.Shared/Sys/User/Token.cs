using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.User
{
 public class Token
 {
  public string username { get; set; }
  public DateTime expiration { get; set; }
  public string hash { get; set; }

  public Token() { }

  public Token(string username, string password)
  {
   username = username.ToLower();
   password = Crypt.getHash(password + username);

   this.username = username;
   expiration = DateTime.UtcNow.AddDays(2);

   hash = Crypt.getHash(password + expiration.ToString("hh:mm:ss dd/mm/yyyy"));
  }

  public TokenValidationResponse verify(string hashword)
  {
   TokenValidationResponse tvr = new TokenValidationResponse();

   if (expiration < DateTime.UtcNow)
   {
    tvr.isValid = false;
    tvr.errorMessage = "Token has expired.";
    return tvr;
   }

   string hashTest = Crypt.getHash(hashword + expiration.ToString("hh:mm:ss dd/mm/yyyy"));

   if (hashTest != hash)
   {
    tvr.isValid = false;
    tvr.errorMessage = "Invalid password.";
    return tvr;
   }

   tvr.isValid = true;

   return tvr;
  }

 }
 public class TokenValidationResponse
 {
  public bool isValid { get; set; }
  public string? errorMessage { get; set; }
 }
}
