using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;
using Apollo2.Shared.Sys.User;
using Apollo2.Sys;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Apollo2.Sys.Data
{
 public class Units
 {
  public static Units instance { get; set; }
  public static List<UnitGlance> units = new List<UnitGlance>();

  public delegate void unitsUpdatedHandler();

  public static event unitsUpdatedHandler unitsUpdated;

  private HttpClient _http;
  private NavigationManager _navi;
  private SystemInformation _systemInformation;
  public Units(HttpClient http, NavigationManager navi, SystemInformation systemInformation)
  {
   instance = this;
   SysTimers.fifteenSecondTimer.Elapsed += FifteenSecondTimer_Elapsed;
   _http = http;
   _navi = navi;
   _systemInformation = systemInformation;
  }

  public async Task<Unit?> getFullUnitDetails(string unit)
  {
    HttpResponseMessage resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + $"/API/Units/Units/get/{unit}", SystemInformation.session);
   if (!resp.IsSuccessStatusCode)
    return null;

   return await resp.Content.ReadFromJsonAsync<Unit>();
  }

  public async Task<List<string>?> getRoles()
  {
   HttpResponseMessage resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + $"/API/Units/Units/roles", SystemInformation.session);
   if (!resp.IsSuccessStatusCode)
    return null;

   return await resp.Content.ReadFromJsonAsync<List<string>>();
  }


  public async Task reqUpdate()
  {
   try
   {
    HttpResponseMessage resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + "/API/Units/Units/get/list", SystemInformation.session);

    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
     SystemInformation.sysURL = null;
     SystemInformation.session = null;
     SystemInformation.token = null;
     await _systemInformation.logout();
     _navi.Refresh();
    }

    if (!resp.IsSuccessStatusCode)
    {
     return;
    }

    List<UnitGlance>? tmpUnits = await resp.Content.ReadFromJsonAsync<List<UnitGlance>>();

    if (tmpUnits == null || tmpUnits.Count == 0) { return; }

    units = tmpUnits;
    unitsUpdated?.Invoke();
   }
   catch { }
  }

  public async Task<List<UnitAttachment>> getAttachedUnits(string incID)
  {
   List<UnitAttachment> units = new List<UnitAttachment>();
   try
   {
    HttpResponseMessage resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + "/API/Units/Units/get/attachment/" + incID, SystemInformation.session);

    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
     SystemInformation.sysURL = null;
     SystemInformation.session = null;
     SystemInformation.token = null;
     await _systemInformation.logout();
     _navi.Refresh();
    }

    if (!resp.IsSuccessStatusCode)
    {
     return units;
    }

    List<UnitAttachment>? tmpUnits = await resp.Content.ReadFromJsonAsync<List<UnitAttachment>>();

    if (tmpUnits == null || tmpUnits.Count == 0) { return units; }

    return tmpUnits;
   }
   catch { return units; }


  }



  private async void FifteenSecondTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
  {
   if (!SystemInformation.IsLoggedIn)
    return;

   await reqUpdate();

  }
 }
}
