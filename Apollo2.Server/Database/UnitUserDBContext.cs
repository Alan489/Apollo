using Apollo2.Server.Sys.Obj;
using Apollo2.Shared.Sys.User;
using MySqlConnector;

namespace Apollo2.Server.Database
{
 public class UnitUserDBContext
 {
  public async Task<User?> getUser(string uname)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();
     command.CommandText = @"SELECT id,username,password,name,access_level,change_password,locked_out,failed_login_count,last_login_time FROM users WHERE username = @user;";

     command.Parameters.AddWithValue("@user", uname);

     using var reader = await command.ExecuteReaderAsync();
     while (reader.Read())
     {
      User ret = new User();
      if (!reader.IsDBNull(0))
       ret.id = reader.GetInt32(0);
      if (!reader.IsDBNull(1))
       ret.username = reader.GetString(1);
      if (!reader.IsDBNull(2))
       ret.password = reader.GetString(2);
      if (!reader.IsDBNull(3))
       ret.name = reader.GetString(3);
      if (!reader.IsDBNull(4))
       ret.access_level = reader.GetInt32(4);
      if (!reader.IsDBNull(5))
       ret.change_password = reader.GetInt32(5);
      if (!reader.IsDBNull(6))
       ret.locked_out = reader.GetInt32(6);
      if (!reader.IsDBNull(7))
       ret.failed_login_count = reader.GetInt32(7);
      if (!reader.IsDBNull(8))
       ret.last_login_time = reader.GetDateTime(8);

      return ret;
     }
     return null;
    }
   }
   catch (Exception ex)
   {
   }

   return null;
  }

  public async Task setPassword(int id, string newPassHash, bool requireChange = false)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     if (requireChange)
      command.CommandText = @"UPDATE users SET password = @newpass, change_password = 1 WHERE id = @id;";
       else
      command.CommandText = @"UPDATE users SET password = @newpass, change_password = 0 WHERE id = @id;";


     command.Parameters.AddWithValue("@id", id);
     command.Parameters.AddWithValue("@newpass", newPassHash);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
  }

  public async Task<List<UserManagementObject>> getUmos()
  {

   List<UserManagementObject> list = new List<UserManagementObject>();

   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = 
      @"SELECT id,username,name,access_level,timeout,change_password,locked_out,failed_login_count FROM users";



     var reader = await command.ExecuteReaderAsync();

     while (reader.Read())
     {
      UserManagementObject userManagementObject = new UserManagementObject();

      if (!reader.IsDBNull(0))
       userManagementObject.id = reader.GetInt32(0);
      if (!reader.IsDBNull(1))
       userManagementObject.username = reader.GetString(1);
      if (!reader.IsDBNull(2))
       userManagementObject.name = reader.GetString(2);
      if (!reader.IsDBNull(3))
       userManagementObject.access_level = reader.GetInt32(3);
      if (!reader.IsDBNull(4))
       userManagementObject.timeout = reader.GetInt32(4);
      if (!reader.IsDBNull(5))
       userManagementObject.change_password = reader.GetInt32(5);
      if (!reader.IsDBNull(5))
       userManagementObject.locked_out = reader.GetInt32(6);
      if (!reader.IsDBNull(6))
       userManagementObject.failed_login_count = reader.GetInt32(7);

      list.Add(userManagementObject);
     }

    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }
   return list;

  }

  public async Task setLock(int id, bool locka)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     if (locka)
      command.CommandText = @"UPDATE users SET locked_out = 1 WHERE id = @id;";
     else
      command.CommandText = @"UPDATE users SET locked_out = 0 WHERE id = @id;";


     command.Parameters.AddWithValue("@id", id);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }


  }

  public async Task setUserName(int id, string name)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"UPDATE users SET name = @NAME WHERE id = @id;";


     command.Parameters.AddWithValue("@id", id);
     command.Parameters.AddWithValue("@NAME", name);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }


  }

  public async Task setUserAccess(int id, int access)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"UPDATE users SET access_level = @LEVEL WHERE id = @id;";


     command.Parameters.AddWithValue("@id", id);
     command.Parameters.AddWithValue("@LEVEL", access);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }


  }



  public async Task setExpired(int id, bool expir)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     if (expir)
      command.CommandText = @"UPDATE users SET change_password = 1 WHERE id = @id;";
     else
      command.CommandText = @"UPDATE users SET change_password = 0 WHERE id = @id;";


     command.Parameters.AddWithValue("@id", id);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }


  }

  public async Task createUser(UserManagementObject cu)
  {
   try
   {

    using (var mysqlconnection = new MySqlConnection(Program.connectionString))
    {
     await mysqlconnection.OpenAsync();

     using var command = mysqlconnection.CreateCommand();

     command.CommandText = @"INSERT INTO users 
             (username, password, name, access_level, timeout, change_password,locked_out,failed_login_count)
      VALUES (@USER, @PASSWORD, @NAME, @ACCESS_LEVEL, @TIMEOUT, @CHANGE_PASSWORD, @LOCKED_OUT, @FAILED_LOGIN_COUNT) ";


     command.Parameters.AddWithValue("@USER", cu.username);
     command.Parameters.AddWithValue("@PASSWORD", cu.newHashword);
     command.Parameters.AddWithValue("@NAME", cu.name);
     command.Parameters.AddWithValue("@ACCESS_LEVEL", cu.access_level);
     command.Parameters.AddWithValue("@TIMEOUT", cu.timeout);
     command.Parameters.AddWithValue("@CHANGE_PASSWORD", cu.change_password);
     command.Parameters.AddWithValue("@LOCKED_OUT", cu.locked_out);
     command.Parameters.AddWithValue("@FAILED_LOGIN_COUNT", cu.failed_login_count);

     await command.ExecuteNonQueryAsync();
    }
   }
   catch (Exception ex)
   {
    Console.WriteLine(ex.ToString());
   }


  }




 }
}
