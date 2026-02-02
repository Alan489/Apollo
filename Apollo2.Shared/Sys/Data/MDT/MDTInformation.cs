using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.MDT
{
 public class MDTInformation
 {
  public Unit unit { get; set; }
  public List<IncidentNote> incidentNotes { get; set; }
 }
}
