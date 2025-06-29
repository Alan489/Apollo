using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.Data.Units;
using MySqlConnector;
using System.Data;

namespace Apollo2.Server.Database
{
 public class UnitDBContext
 {
  public async Task<List<UnitGlance>?> getUnitList()
  {
   try
   {
    List<UnitGlance> unitList = new List<UnitGlance>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT 
  u.unit,
  status,
  u.role,
  color_html,
  type,
  ai.call_number,
  ai.incident_id AS inc_id
FROM units u
INNER JOIN unit_roles rl ON u.role = rl.role
LEFT JOIN (
    SELECT 
      a.unit, 
      i.call_number, 
      i.incident_id
    FROM incident_units a
    INNER JOIN incidents i ON a.incident_id = i.incident_id
    WHERE a.cleared_time IS NULL
) AS ai ON u.unit = ai.unit
ORDER BY u.unit;";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      UnitGlance unit = new UnitGlance();
      if (!reader.IsDBNull(0))
       unit.unit = reader.GetString(0);
      if (!reader.IsDBNull(1))
       unit.status = reader.GetString(1);
      if (!reader.IsDBNull(2))
       unit.role = reader.GetString(2);
      if (!reader.IsDBNull(3))
       unit.color = reader.GetString(3);
      if (!reader.IsDBNull(4))
       unit.type = reader.GetString(4);
      if (!reader.IsDBNull(5))
       unit.attachment = reader.GetString(5);
      if (!reader.IsDBNull(6))
       unit.incID = reader.GetInt32(6);

      unitList.Add(unit);
     }
     return unitList;
    }
   }
   catch (Exception ex)
   {
   }

   return null;
  }

  public async Task<List<UnitAttachment>?> getUnitAttachments(int incID)
  {
   try
   {
    List<UnitAttachment> unitAttachments = new List<UnitAttachment>();

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT u.unit, a.dispatch_time, a.arrival_time, a.transport_time, a.transportdone_time, a.cleared_time, r.color_html, a.uid
FROM incident_units a LEFT JOIN units u ON u.unit=a.unit LEFT JOIN unit_roles r ON r.role = u.role
WHERE a.incident_id = " + incID + ";";

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      UnitAttachment unit = new UnitAttachment();
      if (!reader.IsDBNull(0))
       unit.unit = reader.GetString(0);
      if (!reader.IsDBNull(1))
       unit.dispatch_time = reader.GetDateTime(1);
      if (!reader.IsDBNull(2))
       unit.arrival_time = reader.GetDateTime(2);
      if (!reader.IsDBNull(3))
       unit.transport_time = reader.GetDateTime(3);
      if (!reader.IsDBNull(4))
       unit.transportdone_time = reader.GetDateTime(4);
      if (!reader.IsDBNull(5))
       unit.cleared_time = reader.GetDateTime(5);
      if (!reader.IsDBNull(6))
       unit.color = reader.GetString(6);
      if (!reader.IsDBNull(7))
       unit.attachmentID = reader.GetInt32(7);

      unitAttachments.Add(unit);
     }
     return unitAttachments;
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

   return null;
  }

  public async Task updateAttachment(int unitAttachmentID, string val)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = "UPDATE incident_units SET ";

     switch (val)
     {
      case "arrived":
       command.CommandText += "arrival_time = ";
       break;

      case "transport":
       command.CommandText += "transport_time = ";
       break;

      case "transportdone":
       command.CommandText += "transportdone_time = ";
       break;

      case "cleared":
       command.CommandText += "cleared_time = ";
       break;
      default:
       return;
     }

     command.CommandText += "NOW() WHERE uid = " + unitAttachmentID;

     await command.ExecuteNonQueryAsync();
    }

    //using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    //{
    // await mysqlconnection.OpenAsync();

    // using var command = mysqlconnection.CreateCommand();

    // command.CommandText = "UPDATE incidents SET updated = NOW() WHERE incident_id IN (SELECT incident_id FROM incident_units WHERE uid = " + unitAttachmentID + ")";

    // await command.ExecuteNonQueryAsync();
    //}


    if (val != "cleared")
     return;

    string? u = null;

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();
     using var getUnit = mysqlconnection.CreateCommand();

     getUnit.CommandText = "SELECT unit FROM incident_units WHERE uid = " + unitAttachmentID;

     var reader = await getUnit.ExecuteReaderAsync();

     reader.Read();



     if (!reader.IsDBNull(0))
      u = reader.GetString(0);

     if (u == null)
      return;
    }

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();
     using var setUnit = mysqlconnection.CreateCommand();

     setUnit.CommandText = "UPDATE units SET status = 'In Service' WHERE unit = '" + u + "'";

     await setUnit.ExecuteNonQueryAsync();

    }

   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

   return;
  }

  public async Task<int> getIncidentFromAttachment(int attachment)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"SELECT incident_id FROM incident_units WHERE uid = " + attachment;

     //Console.WriteLine(command.CommandText);

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      return reader.GetInt32(0);
     }


    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }
   return -1;
  }

  public async Task<string> getUnitFromAttachment(int attachment)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"SELECT unit FROM incident_units WHERE uid = " + attachment;

     var reader = await command.ExecuteReaderAsync();
     reader.Read();

     return reader.GetString(0);

    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }
   return "";
  }

  public async Task attachUnit(int inc, string unit)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"UPDATE units SET status = 'Attached to Incident' WHERE unit = '{unit}'";
     await command.ExecuteNonQueryAsync();

    }

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"INSERT INTO incident_units (incident_id, unit, dispatch_time) VALUES ({inc}, '{unit}', NOW())";

     await command.ExecuteNonQueryAsync();
    }

    //using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    //{
    // await mysqlconnection.OpenAsync();

    // using var command = mysqlconnection.CreateCommand();

    // command.CommandText = "UPDATE incidents SET updated = NOW() WHERE incident_id = " + inc;

    // await command.ExecuteNonQueryAsync();
    //}

   }
   catch (Exception ex)
   {
    Console.WriteLine(ex);
   }

  }

  public async Task setStatus(string unit, string stat)
  {

   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"UPDATE units SET status = @STAT WHERE unit = @UNIT AND status <> 'Attached to Incident'";

     command.Parameters.AddWithValue("@STAT", stat);
     command.Parameters.AddWithValue("@UNIT", unit);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }

  }

  public async Task<Unit?> getUnit(string unit)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"SELECT unit, status, update_ts, role, type, personnel FROM units WHERE unit = @UNIT";
     command.Parameters.AddWithValue("@UNIT", unit);

     var reader = await command.ExecuteReaderAsync();

     while (reader.Read())
     {
      Unit unit1 = new Unit();
      if (!reader.IsDBNull(0))
       unit1.unit = reader.GetString(0);
      if (!reader.IsDBNull(1))
       unit1.status = reader.GetString(1);
      if (!reader.IsDBNull(2))
       unit1.update_ts = reader.GetDateTime(2);
      if (!reader.IsDBNull(3))
       unit1.role = reader.GetString(3);
      if (!reader.IsDBNull(4))
       unit1.type = reader.GetString(4);
      if (!reader.IsDBNull(5))
       unit1.personnel = reader.GetString(5);
      return unit1;
     }

    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }

   return null;

  }

  public async Task<List<string>?> getRoles()
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"SELECT role FROM unit_roles";

     var reader = await command.ExecuteReaderAsync();
     List<string> roles = new List<string>(); 
     while (reader.Read())
     {
      
      roles.Add(reader.GetString(0));

     }
     return roles;
    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }

   return null;

  }


  public async Task updateUnit(Unit u)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"UPDATE units SET status=@STATUS, update_ts=NOW(), role=@ROLE, type=@TYPE, personnel=@PERSONNEL WHERE unit = @UNIT";

     command.Parameters.AddWithValue("@STATUS", u.status);
     command.Parameters.AddWithValue("@ROLE", u.role);
     command.Parameters.AddWithValue("@TYPE", u.type);
     command.Parameters.AddWithValue("@PERSONNEL", u.personnel);
     command.Parameters.AddWithValue("@UNIT", u.unit);
     
     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }

  }

  public async Task newUnit(Unit u)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = $"INSERT INTO units (unit, status, update_ts, role, type, personnel) VALUES (@UNIT, @STATUS, NOW(), @ROLE, @TYPE, @PERSONNEL)";

     command.Parameters.AddWithValue("@STATUS", u.status);
     command.Parameters.AddWithValue("@ROLE", u.role);
     command.Parameters.AddWithValue("@TYPE", u.type);
     command.Parameters.AddWithValue("@PERSONNEL", u.personnel);
     command.Parameters.AddWithValue("@UNIT", u.unit);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex) { Console.WriteLine(ex.ToString()); }

  }



 }
}
