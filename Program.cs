using Apollo2.Shared.Sys;
using Apollo2.Sys;
using Apollo2.Sys.Data;
using Apollo2.Sys.Management;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Apollo2
{
 public class Program
 {
  public static async Task Main(string[] args)
  {
   SysTimers.CreateTimers();
   var builder = WebAssemblyHostBuilder.CreateDefault(args);
   builder.RootComponents.Add<App>("#app");
   builder.RootComponents.Add<HeadOutlet>("head::after");

   builder.Services.AddBlazoredLocalStorageAsSingleton(config =>
   {
   });
   builder.Services.AddBlazoredSessionStorageAsSingleton();

   builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
   
   builder.Services.AddSingleton<Incidents, Incidents>();
   builder.Services.AddSingleton<Units, Units>();
   builder.Services.AddSingleton<UserManagement, UserManagement>();
   builder.Services.AddSingleton<SystemInformation, SystemInformation>();

   await builder.Build().RunAsync();
  }
 }
}
