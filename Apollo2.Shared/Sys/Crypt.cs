using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys
{
 public class Crypt
 {
  public static string getHash(string key)
  {
   StringBuilder Sb = new StringBuilder();

   using (SHA256 hash = SHA256.Create())
   {
    Encoding enc = Encoding.UTF8;
    Byte[] result = hash.ComputeHash(enc.GetBytes(key));

    foreach (Byte b in result)
     Sb.Append(b.ToString("x2"));
   }

   return Sb.ToString();
  }
 }
}
