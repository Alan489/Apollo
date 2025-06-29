using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.User;
using Apollo2.Sys;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Apollo2.Sys.Data
{
 public class Incidents
 {
  public static Incidents instance {  get; set; }
  public static List<Incident> incidents = new List<Incident>();

  public delegate void incidentsUpdatedHandler();

  public static event incidentsUpdatedHandler incidentsUpdated;

  private HttpClient _http;
  private NavigationManager _navi;
  private SystemInformation _systemInformation;

  public Incidents(HttpClient http, NavigationManager navi, SystemInformation systemInformation)
  {
   instance = this;
   SysTimers.fifteenSecondTimer.Elapsed += FifteenSecondTimer_Elapsed;
   _http = http;
   _navi = navi;
   _systemInformation = systemInformation;
  }

  public async Task reqUpdate()
  {
   try
   {
    HttpResponseMessage resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + "/API/Incidents/Incidents/get/active", SystemInformation.session);

    GetIncidentResponse? gir = await resp.Content.ReadFromJsonAsync<GetIncidentResponse>();
    if (gir == null)
     return;
    if (gir.success == false)
    {
     SystemInformation.sysURL = null;
     SystemInformation.session = null;
     SystemInformation.token = null;
     await _systemInformation.logout();
     _navi.Refresh();
    }
    if (gir.Incidents == null)
     return;

    incidents = gir.Incidents.OrderBy(a => a.incident_id).ToList();
    incidentsUpdated?.Invoke();
   }
   catch { }


  }

  private async void FifteenSecondTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
  {
   if (!SystemInformation.IsLoggedIn)
    return;

   await reqUpdate();

  }
 }
}
