using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.Data.Incidents;
using Apollo2.Shared.Sys.Data.Map;
using MySqlConnector;
using System.Security.AccessControl;

namespace Apollo2.Server.Database
{
 public class POIDBContext
 {
  public async Task<List<poi>> getPOIs()
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT poi_id, poi_name, poi_shortname, poi_lat, poi_lng, poi_category FROM points_of_interest;";

     using var reader = await command.ExecuteReaderAsync();

     List<poi> result = new List<poi>(); 

     while (reader.Read())
     {
      poi r1 = new poi();
      if (!reader.IsDBNull(0))
       r1.poi_id = reader.GetInt32(0);
      if (!reader.IsDBNull(1))
       r1.poi_name = reader.GetString(1);
      if (!reader.IsDBNull(2))
       r1.poi_shortname = reader.GetString(2);
      if (!reader.IsDBNull(3))
       r1.poi_lat = reader.GetDouble(3);
      if (!reader.IsDBNull(4))
       r1.poi_lng = reader.GetDouble(4);
      if (!reader.IsDBNull(5))
       r1.poi_category = reader.GetString(5);
      result.Add(r1);
     }
     return result;
    }
   }
   catch (Exception ex)
   {
   }
   return new List<poi>();
  }

  public async Task<List<POICategory>> getCats()
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT id, name, color, bgcolor FROM poi_categories;";

     using var reader = await command.ExecuteReaderAsync();

     List<POICategory> result = new List<POICategory>();

     while (reader.Read())
     {
      POICategory r1 = new POICategory();
      if (!reader.IsDBNull(0))
       r1.cat_id = reader.GetInt32(0);
      if (!reader.IsDBNull(1))
       r1.name = reader.GetString(1);
      if (!reader.IsDBNull(2))
       r1.color = reader.GetString(2);
      if (!reader.IsDBNull(3))
       r1.bgcolor = reader.GetString(3);
      result.Add(r1);
     }
     return result;
    }
   }
   catch (Exception ex)
   {
   }

   return new List<POICategory>();
  }

  public async Task deletePOI(int id)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"DELETE FROM points_of_interest WHERE poi_id = @id;";
     command.Parameters.AddWithValue("id", id);

     command.ExecuteNonQuery();
     return;
    }
   }
   catch (Exception ex)
   {
   }
  }

  public async Task createPOI(poi POI)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"INSERT INTO points_of_interest (poi_name, poi_shortname, poi_lat, poi_lng, poi_category) VALUES (@name, @shortname, @lat, @lng, @cat)";

     command.Parameters.AddWithValue("name", POI.poi_name);
     command.Parameters.AddWithValue("shortname", POI.poi_shortname);
     command.Parameters.AddWithValue("lat", POI.poi_lat);
     command.Parameters.AddWithValue("lng", POI.poi_lng);
     command.Parameters.AddWithValue("cat", POI.poi_category);

     command.ExecuteNonQuery();
    }
   }
   catch (Exception ex)
   {
   }
  }

  public async Task<int> createPOICategory(POICategory cat)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"INSERT INTO poi_categories (name,color,bgcolor) VALUES(@name,@color,@bgcolor);";
      command.Parameters.AddWithValue("name", cat.name);
      command.Parameters.AddWithValue("color", cat.color);
      command.Parameters.AddWithValue("bgcolor", cat.bgcolor);
      command.ExecuteNonQuery();
     }

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT LAST_INSERT_ID();";
      var reader = command.ExecuteReader();
      while (reader.Read())
      {
       return reader.GetInt32(0);
      }

     }

     
    }

   }
   catch (Exception ex) { return -1; }
   return -1;
  }

  public async Task updatePOICategory(POICategory cat)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"UPDATE poi_categories SET color=@color,bgcolor=@bgcolor WHERE id=@id";
      command.Parameters.AddWithValue("color", cat.color);
      command.Parameters.AddWithValue("bgcolor", cat.bgcolor);
      command.Parameters.AddWithValue("id", cat.cat_id);
      command.ExecuteNonQuery();
     }
    }

   }
   catch (Exception ex) { }
  }

  public async Task<List<Poly>> getPolys()
  {
   List<Poly> response = new List<Poly>();

   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT id, type, category, name FROM polygon_definition";

      var reader = command.ExecuteReader();

      while (reader.Read())
      {
       Poly p = new Poly();
       if (!reader.IsDBNull(0))
        p.id = reader.GetInt32(0);
       if (!reader.IsDBNull(1))
        p.type = reader.GetString(1);
       if (!reader.IsDBNull(2))
        p.category = reader.GetString(2);
       if (!reader.IsDBNull(3))
        p.name = reader.GetString(3);
       p.coordinates = new Dictionary<int, CoordinatePair>();
       response.Add(p);
      }
      reader.Close();
      await reader.DisposeAsync();
      command.Dispose();

     }


     foreach (Poly poly in response)
     {
      using (var command = mysqlconnection.CreateCommand())
      {
       command.CommandText = @"SELECT lat, lng FROM polygon_coordinates WHERE definition_id = @pid";
       command.Parameters.AddWithValue("pid", poly.id);

       var reader = command.ExecuteReader();
       int index = 0;
       while (reader.Read())
       {
        CoordinatePair cp = new CoordinatePair();
        cp.lat = reader.GetDouble(0);
        cp.lng = reader.GetDouble(1);
        poly.coordinates.Add(index, cp);
        index++;
       }
       reader.Close();
       await reader.DisposeAsync();
       command.Dispose();
      }
     }

    }

   }
   catch (Exception ex) {
    Console.WriteLine(ex);
   }


   return response;
  }

  public async Task deletePoly(int id)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"DELETE FROM polygon_definition WHERE id = @pid";
      command.Parameters.AddWithValue("pid", id);
      command.ExecuteNonQuery();
     }

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"DELETE FROM polygon_coordinates WHERE definition_id = @pid";
      command.Parameters.AddWithValue("pid", id);
      command.ExecuteNonQuery();
     }
    }

   }
   catch (Exception ex) { Console.WriteLine(ex); }
  }

  public async Task createPoly(Poly poly)
  {
   try
   {
    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"INSERT INTO polygon_definition (type,category,name) VALUES (@type,@category,@name)";
      command.Parameters.AddWithValue("type", poly.type);
      command.Parameters.AddWithValue("category", poly.category);
      command.Parameters.AddWithValue("name", poly.name);
      command.ExecuteNonQuery();
      command.Dispose();
     }

     int id = -1;

     using (var command = mysqlconnection.CreateCommand())
     {
      command.CommandText = @"SELECT LAST_INSERT_ID();";
      var reader = command.ExecuteReader();
      reader.Read();
      id = reader.GetInt32(0);
      await reader.DisposeAsync();
      command.Dispose();
     }

     foreach (CoordinatePair pair in poly.coordinatePairList())
      using (var command = mysqlconnection.CreateCommand())
      {
       Console.WriteLine(pair.lng);
       command.CommandText = @"INSERT INTO polygon_coordinates (definition_id,lat,lng) VALUES (@definition_id,@lat,@lng)";
       command.Parameters.AddWithValue("definition_id", id);
       command.Parameters.AddWithValue("lat", pair.lat);
       command.Parameters.AddWithValue("lng", pair.lng);
       command.ExecuteNonQuery();
       command.Dispose();
      }
    }

   }
   catch (Exception ex) { Console.WriteLine(ex); }
  }



 }
}
