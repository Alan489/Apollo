using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Map
{
 public class Poly
 {
  public int id {  get; set; }
  public string type { get; set; } //LINE, POLYGON
  public string category { get; set; }
  public string name { get; set; }
  public Dictionary<int, CoordinatePair> coordinates { get; set; } = new();
  public List<CoordinatePair> c2 { 
   get
   {
    return coordinatePairList();
   }
   set { }
  }
  public string? color { get; set; }


  public List<CoordinatePair> coordinatePairList()
  {
   return coordinates
       .OrderBy(k => k.Key)
       .Select(k => k.Value)
       .ToList();
  }
 }
}
