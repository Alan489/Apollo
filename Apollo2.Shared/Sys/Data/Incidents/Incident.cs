using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Incidents
{
 public class Incident
 {


  public int incident_id { get; set; }
  public string? call_number {  get; set; }
  public string? call_type { get; set; }
  public string? call_details { get; set; }
  public DateTime? ts_opened { get; set; }
  public DateTime? ts_dispatch {  get; set; }
  public DateTime? ts_arrival { get; set; }
  public DateTime? ts_complete { get; set; }
  public string? location { get; set; }
  public string? location_num { get; set; }
  public string? reporting_pty { get; set; }
  public string? contact_at { get; set; }
  public string? disposition { get; set; }
  public DateTime? updated { get; set; }
  public int duplicate_of_incident_id { get; set; } = -1;
  public string? incident_status { get; set; }
  public string? unit_number { get; set; }
  public string? attachedUnits { get; set; }
 }

}
