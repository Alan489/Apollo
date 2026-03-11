using Apollo2.Shared.Sys.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Map
{
 public class UpdateCategoryRequest
 {
  public UserSession session {  get; set; }
  public POICategory Category { get; set; }
 }
}
