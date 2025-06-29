using Apollo2.Server.Database;
using Apollo2.Server.Services;

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

  public static void Main(string[] args)
  {
   var builder = WebApplication.CreateBuilder(args);

   IConfigurationSection dbSection = builder.Configuration.GetSection("Database");
   _conStr = $"Server={dbSection.GetValue<string>("Host")};Port={dbSection.GetValue<string>("Port")};Database={dbSection.GetValue<string>("DB")};Uid={dbSection.GetValue<string>("Username")};Pwd={dbSection.GetValue<string>("Password")}";
   _sysName = builder.Configuration.GetValue<string>("SysName");
   _mySign = builder.Configuration.GetValue<string>("Signature");

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
