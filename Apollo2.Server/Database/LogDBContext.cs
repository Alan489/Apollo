using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Units;
using Microsoft.AspNetCore.Components;
using MySqlConnector;

namespace Apollo2.Server.Database
{
 public static class LogDBContext
 {
  public static async Task incidentLog(int incident, string message, string? dispatcher = "SYSTEM", string? unit = "")
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = $"INSERT INTO incident_notes (incident_id, ts, unit, message, deleted, creator) VALUES ({incident}, NOW(), '{unit}', @msg, 0, '{dispatcher}');";
     //Console.WriteLine(command.CommandText);
     command.Parameters.AddWithValue("@msg", message);
     command.ExecuteNonQuery();
    }
    
    
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = $"UPDATE incidents SET updated = NOW() WHERE incident_id = {incident} ";
     //Console.WriteLine(command.CommandText);
     command.Parameters.AddWithValue("@msg", message);
     command.ExecuteNonQuery();
    }


   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
  }

  public static async Task<List<IncidentNote>> getIncNotes(int inc)
  {
   List<IncidentNote> ret = new List<IncidentNote>();
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {

     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = $"SELECT ts, unit, message, creator FROM incident_notes WHERE incident_id = " + inc;

     var reader = await command.ExecuteReaderAsync();

     while (reader.Read())
     {
      IncidentNote IN = new IncidentNote();
      if (!reader.IsDBNull(0))
       IN.ts = reader.GetDateTime(0);
      if (!reader.IsDBNull(1))
       IN.unit = reader.GetString(1);
      if (!reader.IsDBNull(2))
       IN.message = reader.GetString(2);
      if (!reader.IsDBNull(3))
       IN.creator = reader.GetString(3);
      ret.Add(IN);
     }
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
   return ret;
  }


 }
}
