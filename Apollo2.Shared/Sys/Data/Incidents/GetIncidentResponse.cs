using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Incidents
{
 public class GetIncidentResponse
 {
  public string? errorMessage { get; set; }
  public bool success { get; set; } = false; 
  public List<Incident>? Incidents { get; set; }

 }
}
