using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.User
{
 public class UserManagementObject
 {
  public int id { get; set; } = -1;
  public string username { get; set; } = "";
  public string? newHashword { get; set; }
  public string name { get; set; } = "";
  public int access_level { get; set; } = 1;
  public int timeout { get; set; } = 300;
  public int change_password { get; set; } = 1;
  public int locked_out { get; set; } = 0;
  public int failed_login_count { get; set; } = 0;

  public UserManagementObject() { }
  public void generateHashword(string password)
  {
   username = username.ToLower();
   newHashword = Crypt.getHash(password + username);
  }
 }
}
