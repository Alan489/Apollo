using Apollo2.Shared.Sys.User;
using Apollo2.Sys.Windows;
using Blazored.LocalStorage;
using System;

namespace Apollo2.Sys
{
 public class SystemInformation
 {

  public static bool IsLoggedIn
  {
   get
   {
    return (sysURL != null && token != null && session != null && token.expiration > DateTime.UtcNow);
   }
  }

  private static ILocalStorageService _storage;

  public static string? sysURL {  get; set; }
  public static Token? token { get; set; }
  public static UserSession? session { get; set; }

  private static string _myID = "";

  private static Random random = new Random();

  public static string windowid
  {
   get
   {
    if (string.IsNullOrEmpty(_myID))
    {
     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
     _myID = new string(Enumerable.Repeat(chars, 6)
         .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    return _myID;
   }
  }

  public SystemInformation(ILocalStorageService storage)
  {
   _storage = storage;
   _storage.Changed += storageChanged;
  }

  private void storageChanged(object? sender, ChangedEventArgs e)
  {
   Console.WriteLine(e.Key);
   if (e.Key == "CONSUMED")
   {
    Console.WriteLine("Something consumed.");
    string[] b = ((string)e.NewValue ?? "").Split("//");
    if (b.Length != 2)
     return;
    Console.WriteLine("is 2.");

    if (b[1] == windowid) return;
    Console.WriteLine("isn't me");

    Window? w = WindowManager.Windows.FirstOrDefault(m => m.ID == b[0]);
    if (w == null) return;
    Console.WriteLine("isn't null");
    WindowManager.closePage(w);
   }
  }

  public async Task logout()
  {
   List<string> list = new List<string>();
   list.Add("SYSURL");
   list.Add("TOKEN");
   list.Add("SESSION");
   await _storage.RemoveItemsAsync(list);
  }

  public async Task save()
  {
   await _storage.SetItemAsync<string>("SYSURL", sysURL);
   await _storage.SetItemAsync<Token>("TOKEN", token);
   await _storage.SetItemAsync<UserSession>("SESSION", session);
  }

  public async Task<bool> loadSavedData()
  {
   string? sysurl = await _storage.GetItemAsync<string>("SYSURL");
   Token? t = await _storage.GetItemAsync<Token>("TOKEN");
   UserSession? sess = await _storage.GetItemAsync<UserSession>("SESSION");
   if (sysurl != null && t != null && sess != null && t.expiration > DateTime.Now)
   {
    sysURL = sysurl;
    token = t;
    session = sess;
    return true;
   }
   return false;
  }
 }
}
