using Apollo2.Shared.Sys.User;
using System.ComponentModel;
using System.Net.Http.Json;

namespace Apollo2.Sys.Management
{
 public class UserManagement
 {

  public bool unauthorized = false;
  public bool error = false;
  public List<UserManagementObject> Users { get; set; } = new List<UserManagementObject>();

  private HttpClient _http;

  public UserManagement(HttpClient http) 
  { 
   _http = http;

  }

  public async Task<List<UserManagementObject>> refreshUsers()
  {
   List<UserManagementObject> umol = new List<UserManagementObject>();
   try
   {
    // API/Sys/User/User/get/umos
    // ACCESS LEVEL 8
    var resp = await _http.PostAsJsonAsync<UserSession>(SystemInformation.sysURL + "/API/Sys/User/User/get/umos", SystemInformation.session);
    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
     unauthorized = true;
     return umol;
    }

    unauthorized = false;

    List<UserManagementObject>? umo = await resp.Content.ReadFromJsonAsync<List<UserManagementObject>>();
    if (umo != null)
    {
     umol = umo;
     error = false;
    }
    else
     error = true;

     Users = umol;
    return Users;

   }
   catch (Exception ex)
   {
    error = true;
    Console.WriteLine("Failed to refresh users\n" + ex.Message);
   }


   return umol;


  }

 }
}
