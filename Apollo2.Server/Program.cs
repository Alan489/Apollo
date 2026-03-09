using Apollo2.Server.Database;
using Apollo2.Server.Services;
using Apollo2.Shared.Sys;

namespace Apollo2.Server
{
 public class Program
 {
  private static string _conStr {  get; set; }
  public static string connectionString {  
   get
   {
    return _conStr;
   }
  }

  private static string _sysName { get; set; }
  public static string sysName
  {
   get
   {
    return _sysName;
   }
  }

  private static string _mySign { get; set; }
  public static string mySign
  {
   get
   {
    return _mySign;
   }
  }

  private static double _myLat { get; set; }
  public static double myLat
  {
   get
   {
    return _myLat;
   }
  }

  private static double _myLong { get; set; }
  public static double myLong
  {
   get
   {
    return _myLong;
   }
  }

  private static int _myZoom { get; set; }
  public static int myZoom
  {
   get
   {
    return _myZoom;
   }
  }

  private static string _googleLink { get; set; }
  public static string googleLink
  {
   get
   {
    return _googleLink;
   }
  }

  public static void Main(string[] args)
  {

   if (args.Length == 1)
    if (args[0].ToLower() == "init")
    {
     Console.WriteLine("User: administrator");
     Console.WriteLine("Password: 8675309");
     Console.WriteLine("Hashword: " + Crypt.getHash("8675309" + "administrator"));
    }

     Console.WriteLine("To print a valid username/hashword, run this program with \"INIT\"");

   var builder = WebApplication.CreateBuilder(args);

   IConfigurationSection dbSection = builder.Configuration.GetSection("Database");
   _conStr = $"Server={dbSection.GetValue<string>("Host")};Port={dbSection.GetValue<string>("Port")};Database={dbSection.GetValue<string>("DB")};Uid={dbSection.GetValue<string>("Username")};Pwd={dbSection.GetValue<string>("Password")}";
   _sysName = builder.Configuration.GetValue<string>("SysName");
   _mySign = builder.Configuration.GetValue<string>("Signature");
   _myLat = builder.Configuration.GetValue<double>("SysMapLat");
   _myLong = builder.Configuration.GetValue<double>("SysMapLong");
   _myZoom = builder.Configuration.GetValue<int>("SysZoom");
   _googleLink = builder.Configuration.GetValue<string>("googleAPI");

   builder.Services.AddCors(options =>
   {
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                       policy.AllowAnyOrigin()
                       .AllowAnyHeader().AllowAnyMethod();
                      });
   });

   builder.Services.AddScoped<UserDBContext, UserDBContext>();
   builder.Services.AddScoped<IncidentsDbContext, IncidentsDbContext>();
   builder.Services.AddScoped<UnitDBContext, UnitDBContext>();
   builder.Services.AddScoped<UnitUserDBContext, UnitUserDBContext>();
   builder.Services.AddScoped<POIDBContext, POIDBContext>();
   builder.Services.AddScoped<Authentication, Authentication>();
   builder.Services.AddControllers();

   var app = builder.Build();

   // Configure the HTTP request pipeline.

   app.UseHttpsRedirection();

   app.UseCors("AllowAll");

   app.UseAuthorization();


   app.MapControllers();

   app.Run();
  }
 }
}
