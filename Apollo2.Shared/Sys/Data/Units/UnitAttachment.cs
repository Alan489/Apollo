using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Units
{
 public class UnitAttachment
 {
  public int attachmentID {  get; set; }
  public string unit {  get; set; }
  public DateTime dispatch_time { get; set; }
  public DateTime? arrival_time { get; set; }
  public DateTime? transport_time { get; set; }
  public DateTime? transportdone_time { get; set; }
  public DateTime? cleared_time { get; set; }
  public string? color { get; set; }
 }
}


//case "arrived":
// ua.arrival_time = DateTime.Now;
// break;

//case "transport":
// ua.transport_time = DateTime.Now;
// break;

//case "transportdone":
// ua.transportdone_time = DateTime.Now;
// break;

//case "cleared":
// ua.cleared_time = DateTime.Now;
// break;