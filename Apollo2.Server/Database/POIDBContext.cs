using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.Data.Map;
using MySqlConnector;

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



 }
}
