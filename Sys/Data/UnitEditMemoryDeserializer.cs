using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;
using System.Data;
using System.Text.Json;

namespace Apollo2.Sys.Data
{
 public static class UnitEditMemoryDeserializer
 {
  public static Dictionary<string, object> deserialize(string json)
  {

   Dictionary<string, object>? result = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
   

   if (result == null ) return new Dictionary<string, object>();

   Dictionary<string, object> ret = new Dictionary<string, object>();

   ////////////////////////

   Unit? MYUNIT = JsonSerializer.Deserialize<Unit>(result["MYUNIT"].ToString());

   if (MYUNIT == null ) return new Dictionary<string, object>();

   ret["MYUNIT"] = MYUNIT;
   Console.WriteLine("Successfully populated MYUNIT");

   ////////////////////////

   List<string>? ROLES = JsonSerializer.Deserialize<List<string>>(result["ROLES"].ToString());

   if (ROLES == null) return new Dictionary<string, object>();

   ret["ROLES"] = ROLES;
   Console.WriteLine("Successfully populated ROLES");


   return ret;

  }
 }
}
