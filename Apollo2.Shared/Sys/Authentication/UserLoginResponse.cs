using Apollo2.Shared.Sys.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Authentication
{
 public class UserLoginResponse
 {
  public UserSession? session {  get; set; }
  public bool successful { get; set; }
  public string? errorMessage { get; set; }
 }
}
