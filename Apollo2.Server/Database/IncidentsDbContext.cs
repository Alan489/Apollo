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
      command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number,lat,log,alternate_cad FROM incidents WHERE disposition IS NULL OR disposition = '';";
      //Console.WriteLine(command.CommandText);
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
      command.CommandText = @"INSERT INTO incidents (" + getNewColString<Incident>(inc) + ") VALUES(" + getNewValString<Incident>(inc) + ");";
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

   return "F" + DateTime.Now.ToString("yy") + ID;
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
     command.CommandText = @"SELECT call_type FROM incident_types WHERE active = 'Y';";

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
    Console.WriteLine(ex);
   }

   return null;
  }

  public async Task<List<TypeModification>?> getIncidentModificationTypes()
  {
   try
   {
    List<TypeModification> ret = new List<TypeModification>();
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT call_type, active FROM incident_types;";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      TypeModification modification = new TypeModification();
      if (!reader.IsDBNull(0))
       modification.Type = reader.GetString(0);
      if (!reader.IsDBNull(1))
       modification.Active = reader.GetChar(1);

      ret.Add(modification);
     }
     return ret;
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

   return null;
  }

  public async Task createIncidentType(string type)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"INSERT INTO incident_types (call_type) VALUES (@type)";
     command.Parameters.AddWithValue("type", type);
     command.ExecuteNonQuery();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

  }

  public async Task updateIncidentType(string type, char val)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();
     Console.WriteLine($"{type} {val}");
     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"UPDATE incident_types SET active = @val WHERE call_type = @type";
     command.Parameters.AddWithValue("val", val);
     command.Parameters.AddWithValue("type", type);
     command.ExecuteNonQuery();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

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
     command.CommandText = @"SELECT disposition FROM incident_disposition_types WHERE active = 'Y';";

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
    Console.WriteLine(ex);
   }

   return null;
  }
  public async Task<List<DispositionModification>?> getIncidentDispoModificationTypes()
  {
   try
   {
    List<DispositionModification> ret = new List<DispositionModification>();
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT disposition, active FROM incident_disposition_types;";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      DispositionModification dm = new DispositionModification();
      if (!reader.IsDBNull(0))
       dm.Disposition = reader.GetString(0);
      if (!reader.IsDBNull(1))
       dm.Active = reader.GetChar(1);

      ret.Add(dm);
     }
     return ret;
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

   return null;
  }

  public async Task createDispoType(string type)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"INSERT INTO incident_disposition_types (disposition) VALUES (@type)";
     command.Parameters.AddWithValue("type", type);
     command.ExecuteNonQuery();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

  }

  public async Task updateDispoType(string type, char val)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"UPDATE incident_disposition_types SET active = @val WHERE disposition = @type";
     command.Parameters.AddWithValue("val", val);
     command.Parameters.AddWithValue("type", type);
     command.ExecuteNonQuery();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

  }

  public async Task<List<Incident>> getIncidentsByDay(DateTime dt)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"SELECT incident_id,call_number,call_type,call_details,ts_opened,ts_dispatch,ts_arrival,ts_complete,location,location_num,reporting_pty,
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number ,lat, log, alternate_cad
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
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number, lat, log, alternate_cad
                             FROM incidents 
                             WHERE (call_number LIKE @CALLNUM OR alternate_cad LIKE @ALTNUM) AND incident_status IN ('Dispositioned', 'Closed')";

     command.Parameters.AddWithValue("@CALLNUM", $"%{val}%");
     command.Parameters.AddWithValue("@ALTNUM", $"%{val}%");

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
                             contact_at,disposition,updated,duplicate_of_incident_id,incident_status,unit_number, lat, log, alternate_cad
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

  public async Task createPage(string unit, int inc)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"INSERT INTO paging (unit, incident_id, pageout_ts) VALUES (@UNIT, @INC, @NOW)";

     command.Parameters.AddWithValue("@UNIT", unit);
     command.Parameters.AddWithValue("@INC", inc);
     command.Parameters.AddWithValue("@NOW", DateTime.UtcNow);

     command.ExecuteNonQuery();

    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
  }

  public async Task<Dictionary<string, Incident>> getActivePages()
  {
   Dictionary<string, Incident> ret = new Dictionary<string, Incident>();

   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT unit, incident_id, pageout_ts FROM paging
                             WHERE pageout_ts > @NOW";

      command.Parameters.AddWithValue("@NOW", DateTime.UtcNow - TimeSpan.FromSeconds(10));

      using (var reader = await command.ExecuteReaderAsync())
      {
       while (reader.Read())
       {

        if (reader.IsDBNull(0) || reader.IsDBNull(1))
         continue;

        Incident? inc = await getIncidentByIncId(reader.GetInt32(1));
        if (inc == null) continue;

        ret[reader.GetString(0)] = inc;

       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return null;
   }


   return ret;
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

     command.CommandText = @"UPDATE incidents SET incident_status = 'Open', disposition = '', ts_complete = NULL, updated = NOW()
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

  public async Task markOnScene(int inc_id)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"UPDATE incidents SET ts_arrival = NOW(), updated = NOW()
                             WHERE incident_id = @ID";

     command.Parameters.AddWithValue("@ID", inc_id);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
    return;
   }
  }

  public async Task markDispatch(int inc_id)
  {
   try
   {
    List<Incident> incs = new List<Incident>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"UPDATE incidents SET ts_dispatch = NOW(), updated = NOW()
                             WHERE incident_id = @ID";

     command.Parameters.AddWithValue("@ID", inc_id);

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
    if (prop.Name == "attachedUnits" || prop.Name == "incident_id" || prop.Name == "ts_dispatch" || prop.Name == "ts_arrival") continue;
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
