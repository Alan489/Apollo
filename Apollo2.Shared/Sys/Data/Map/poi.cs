using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Map
{
 public class poi
 {
  public int poi_id {  get; set; }
  public string poi_name { get; set; }
  public string poi_shortname { get; set; }
  public double poi_lat { get; set; }
  public double poi_lng { get; set; }
  public string poi_category { get; set; }
 }
}
