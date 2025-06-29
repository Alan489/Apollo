using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.User
{
 public class UserSession
 {
  public int Id { get; set; }
  public string Username { get; set; }
  public string Name { get; set; }
  public string sysName { get; set; }
  public int AccessLevel { get; set; }
  public Token token { get; set; }
  public string signature { get; set; }

  public void sign(string signature)
  {
   string str = "{" + Id + "}";
   str += " " + Username + " " + sysName + " " + token.hash;
   str += signature;
   this.signature = Crypt.getHash(str);
  }

  public bool verifysignature(string signature)
  {
   string str = "{" + Id + "}";
   str += " " + Username + " " + sysName + " " + token.hash;
   str += signature;
   str = Crypt.getHash(str);

   return str == this.signature;
  }

 }
}
