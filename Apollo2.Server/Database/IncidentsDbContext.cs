using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.Data.Incidents;
using MySqlConnector;
using System.Data;
using System.Reflection;

namespace Apollo2.Server.Database
{
 public class IncidentsDbContext
 {
  public async Task<List<Incident>?> getCurrentIncidents()
  {
   try
   {
    List<Incident> incidents = new List<Incident>();
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number FROM incidents WHERE disposition IS NULL OR disposition = '';";
      using var reader = await command.ExecuteReaderAsync();
      incidents = DataReaderMapToList<Incident>(reader);
     }

     foreach (var incident in incidents)
     {
      using (var command = mysqlconnection.CreateCommand())
      {
       command.CommandText = $"SELECT unit FROM incident_units WHERE incident_id = {incident.incident_id} AND cleared_time IS NULL";
       using var reader = await command.ExecuteReaderAsync();
       string units = "";
       while (reader.Read())
       {
        if (!reader.IsDBNull(0))
         if (string.IsNullOrEmpty(units))
          units = reader.GetString(0);
         else
          units += ", " + reader.GetString(0);
       }
       incident.attachedUnits = units;
      }
     }

     return incidents;

    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
   return null;
  }

  public async Task saveIncident(Incident inc)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"UPDATE incidents SET " + getUpdateString<Incident>(inc) + " WHERE incident_id=" + inc.incident_id + ";";
      setParameters<Incident>(inc, command);
      //Console.WriteLine(command.CommandText);
      command.ExecuteNonQuery();

      
     }
    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }
  }

  public async Task newIncident(Incident inc)
  {
   inc.updated = DateTime.Now;
   inc.ts_opened = DateTime.Now;
   inc.incident_status = "Open";
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"INSERT INTO incidents (" +getNewColString<Incident>(inc)+ ") VALUES(" + getNewValString<Incident>(inc) + ");";
      setParameters<Incident>(inc, command);
      //Console.WriteLine(command.CommandText);
      command.ExecuteNonQuery();
     }

     string CallNum = "";
     int it = -1;

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT LAST_INSERT_ID();";
      var reader = command.ExecuteReader();
      while (reader.Read())
      {
       CallNum = getCallNum(reader.GetInt32(0));
       it = reader.GetInt32(0);
      }
      reader.Close();
      command.Dispose();
     }

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = $"UPDATE incidents SET call_number = '{CallNum}' WHERE incident_id = {it}";
      setParameters<Incident>(inc, command);
      //Console.WriteLine(command.CommandText);
      command.ExecuteNonQuery();
      inc.incident_id = it;
      inc.call_number = CallNum;
     }


    }

   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }
  }

  private string getCallNum(int id)
  {
   string ID = id.ToString();

   while (ID.Length < 4)
   {
    ID = "0" + ID;
   }

   return DateTime.Now.ToString("yyyy-") + ID;
  }

  public async Task<List<string>?> getIncidentTypes()
  {
   try
   {
    List<string> ret = new List<string>();
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT call_type FROM incident_types;";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      if (!reader.IsDBNull(0))
       ret.Add(reader.GetString(0));

      
     }
     return ret;
    }
   }
   catch (Exception ex)
   {
   }

   return null;
  }

  public async Task<List<string>?> getIncidentDispoTypes()
  {
   try
   {
    List<string> ret = new List<string>();
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT disposition FROM incident_disposition_types;";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      if (!reader.IsDBNull(0))
       ret.Add(reader.GetString(0));


     }
     return ret;
    }
   }
   catch (Exception ex)
   {
   }

   return null;
  }

  public async Task<List<Incident>> getIncidentsByDay(DateTime dt)
  {
   try
   {
    List<Incident> incs= new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     
     command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number 
                             FROM incidents 
                             WHERE (incident_status IN ('Dispositioned', 'Closed')) AND
                             (ts_complete IS NOT NULL AND 
                                (ts_complete BETWEEN @START AND @END) OR 
                             (ts_opened IS NOT NULL AND 
                                (ts_opened BETWEEN @START AND @END)))";

     command.Parameters.AddWithValue("@START", dt);
     command.Parameters.AddWithValue("@END", dt.AddDays(1));

     using var reader = await command.ExecuteReaderAsync();
     incs = DataReaderMapToList<Incident>(reader);
     return incs;
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return new List<Incident>();
   }
  }

  public async Task<List<Incident>> getIncidentsByInc(string val)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number 
                             FROM incidents 
                             WHERE (call_number LIKE @CALLNUM) AND incident_status IN ('Dispositioned', 'Closed')";

     command.Parameters.AddWithValue("@CALLNUM", $"%{val}%");

     using var reader = await command.ExecuteReaderAsync();
     incs = DataReaderMapToList<Incident>(reader);
     return incs;
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return new List<Incident>();
   }
  }

  public async Task<Incident?> getIncidentByIncId(int id)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number 
                             FROM incidents 
                             WHERE incident_id = @ID";

     command.Parameters.AddWithValue("@ID", id);

     using var reader = await command.ExecuteReaderAsync();
     incs = DataReaderMapToList<Incident>(reader);
     if (incs.Count == 0) return null;
     return incs[0];
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return null;
   }
  }


  public async Task reopenInc(int id)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"UPDATE incidents SET incident_status = 'Open', disposition = '', ts_complete = NULL
                             WHERE incident_id = @ID";

     command.Parameters.AddWithValue("@ID", id);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return;
   }
  }

  public static string GetDisplayName(Type t)
  {
   if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
    return string.Format("{0}", GetDisplayName(t.GetGenericArguments()[0]));

   return t.Name;
  }

  public static void setParameters<T>(T obj, MySqlCommand cmd)
  {
   string str = "";


   foreach (PropertyInfo prop in obj.GetType().GetProperties())
   {
    if (prop.Name == "attachedUnits" || prop.Name == "incident_id") continue;
    if (prop.GetValue(obj) == null) continue;

    cmd.Parameters.AddWithValue("@PARA" + prop.Name, prop.GetValue(obj));


   }//2025-06-24 20:25:17

   return;
  }

  public static string getUpdateString<T>(T obj)
  {
   string str = "";


   foreach (PropertyInfo prop in obj.GetType().GetProperties())
   {
    if (prop.Name == "attachedUnits" || prop.Name == "incident_id") continue;
    if (prop.GetValue(obj) == null) continue;
    str += ", " + prop.Name + " = @PARA" + prop.Name;


   }//2025-06-24 20:25:17

   return str.Substring(2);
  }
  
  public static string getNewColString<T>(T obj)
  {
   string str = "";


   foreach (PropertyInfo prop in obj.GetType().GetProperties())
   {
    if (prop.Name == "attachedUnits" || prop.Name == "incident_id") continue;
    if (prop.GetValue(obj) == null) continue;
    str += ", " + prop.Name;


   }//2025-06-24 20:25:17

   return str.Substring(2);
  }
  
  public static string getNewValString<T>(T obj)
  {
   string str = "";


   foreach (PropertyInfo prop in obj.GetType().GetProperties())
   {
    if (prop.Name == "attachedUnits" || prop.Name == "incident_id") continue;
    if (prop.GetValue(obj) == null) continue;
    str += ", @PARA" + prop.Name;


   }//2025-06-24 20:25:17

   return str.Substring(2);
  }


  public static List<T> DataReaderMapToList<T>(IDataReader dr)
  {
   List<T> list = new List<T>();
   T obj = default(T);
   while (dr.Read())
   {
    obj = Activator.CreateInstance<T>();
    foreach (PropertyInfo prop in obj.GetType().GetProperties())
    {
     if (prop.Name == "attachedUnits") continue;
     if (!object.Equals(dr[prop.Name], DBNull.Value))
     {
      prop.SetValue(obj, dr[prop.Name], null);
     }
    }
    list.Add(obj);
   }
   return list;
  }



 }
}
