using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Units
{
 public class Unit
 {
  public string unit { get; set; } = "";
  public string status { get; set; } = "";
  public DateTime update_ts { get; set; } = DateTime.MinValue;
  public string role { get; set; } = "";
  public string type { get; set; } = "";
  public string personnel { get; set; } = "";
 }
}
