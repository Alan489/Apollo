using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo2.Shared.Sys.Data.Map
{
 public class POICategory
 {
  public int cat_id {  get; set; }
  public string name { get; set; }
  public string color { get; set; } = "0,0,0";
  public string bgcolor { get; set; } = "0,0,0";

  public string hexbg
  {
   get
   {
    return $"#{bgr:X2}{bgg:X2}{bgb:X2}";
   }
  }

  public int bgr
  {
   get
   {
    try
    {
     return int.Parse(bgcolor.Split(",")[0]);
    } catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     bgcolor = $"{0},{bgg},{bgb}";
    }
    if (value > 255)
    {
     bgcolor = $"{255},{bgg},{bgb}";
    }
    bgcolor = $"{value},{bgg},{bgb}";
   }
  }

  public int bgg
  {
   get
   {
    try
    {
     return int.Parse(bgcolor.Split(",")[1]);
    }
    catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     bgcolor = $"{bgr},{0},{bgb}";
    }
    if (value > 255)
    {
     bgcolor = $"{bgr},{255},{bgb}";
    }

    bgcolor = $"{bgr},{value},{bgb}";
   }
  }

  public int bgb
  {
   get
   {
    try
    {
     return int.Parse(bgcolor.Split(",")[2]);
    }
    catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     bgcolor = $"{bgr},{bgg},{0}";
    }
    if (value > 255)
    {
     bgcolor = $"{bgr},{bgg},{255}";
    }
    bgcolor = $"{bgr},{bgg},{value}";
   }
  }



  public int fgr
  {
   get
   {
    try
    {
     return int.Parse(color.Split(",")[0]);
    }
    catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     color = $"{0},{fgg},{fgb}";
    }
    if (value > 255)
    {
     color = $"{255},{fgg},{fgb}";
    }
    color = $"{value},{fgg},{fgb}";
   }
  }

  public int fgg
  {
   get
   {
    try
    {
     return int.Parse(color.Split(",")[1]);
    }
    catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     color = $"{fgr},{0},{fgb}";
    }
    if (value > 255)
    {
     color = $"{fgr},{255},{fgb}";
    }
    color = $"{fgr},{value},{fgb}";
   }
  }

  public int fgb
  {
   get
   {
    try
    {
     return int.Parse(color.Split(",")[2]);
    }
    catch (Exception e)
    {
     return 0;
    }
   }
   set
   {
    if (value < 0)
    {
     color = $"{fgr},{fgg},{0}";
    }
    if (value > 255)
    {
     color = $"{fgr},{fgg},{255}";
    }
    color = $"{fgr},{fgg},{value}";
   }
  }




 }
}
