using Apollo2.Shared.Sys.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Units
{
 public class UnitUpdateRequest
 {
  public Unit unit {  get; set; }
  public UserSession session { get; set; }
 }
}
