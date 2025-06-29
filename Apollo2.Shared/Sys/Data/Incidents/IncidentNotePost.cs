using Apollo2.Shared.Sys.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Incidents
{
 public class IncidentNotePost
 {

  public UserSession sess {  get; set; }
  public string message {get; set;}
  public string? unit {get; set;}
  public DateTime ts {get; set;}
  public string? creator { get; set; }

 }
}
// API/Incidents/Incidents/get/incnotes/{inc}
//INCNOTES