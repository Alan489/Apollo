using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;
using System.Text.Json;

namespace Apollo2.Sys.Data
{
 public static class IncidentMemoryDeserializer
 {
  public static Dictionary<string, object> deserialize(string json)
  {

   Dictionary<string, object>? result = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
   Console.WriteLine("MYINCIDENT::::::::: " + result["MYINCIDENT"].ToString());
   

   if (result == null ) return new Dictionary<string, object>();

   Dictionary<string, object> ret = new Dictionary<string, object>();

   ////////////////////////

   Incident? myIncident = JsonSerializer.Deserialize<Incident>(result["MYINCIDENT"].ToString());

   if (myIncident == null ) return new Dictionary<string, object>();

   ret["MYINCIDENT"] = myIncident;
   Console.WriteLine("Successfully populated MYINCIDENT");

   ////////////////////////

   List<string>? DISPOTYPES = JsonSerializer.Deserialize<List<string>>(result["DISPOTYPES"].ToString());

   if (DISPOTYPES == null) return new Dictionary<string, object>();

   ret["DISPOTYPES"] = DISPOTYPES;
   Console.WriteLine("Successfully populated DISPOTYPES");

   ////////////////////////

   List<string>? INCTYPES = JsonSerializer.Deserialize<List<string>>(result["INCTYPES"].ToString());

   if (INCTYPES == null) return new Dictionary<string, object>();

   ret["INCTYPES"] = INCTYPES;
   Console.WriteLine("Successfully populated INCTYPES");

   ////////////////////////

   if (result.ContainsKey("attachingUnit"))
    ret["attachingUnit"] = "YES";

   Console.WriteLine("Successfully populated attachingUnit");

   ////////////////////////

   if (result.ContainsKey("selectingUnit"))
    ret["selectingUnit"] = "YES";

   Console.WriteLine("Successfully populated selectingUnit");

   ////////////////////////

   List<UnitAttachment>? attachments = JsonSerializer.Deserialize<List<UnitAttachment>>(result["UNITATTACHMENTS"].ToString());

   if (attachments == null) return new Dictionary<string, object>();

   ret["UNITATTACHMENTS"] = attachments;
   Console.WriteLine("Successfully populated UNITATTACHMENTS");

   ////////////////////////

   ret["NOTESUNIT"] = result["NOTESUNIT"].ToString();
   Console.WriteLine("Successfully populated NOTESUNIT");






   return ret;

  }
 }
}
