namespace Apollo2.Sys.Windows
{
 public class Window
 {
  //These are reserved for da future
  //public int x,y,w,h;
  //public bool isMaximized = true;
  //public bool isMinimized = false;


  public string Title { get; set; }
  public string WINDOWTYPE { get; set; }
  public string property { get; set; }
  public Dictionary<string, object> memory = new Dictionary<string, object>();
  public bool firstRender { get; set; } = true;
  public string ID { get; set; } = Guid.NewGuid().ToString();
 }
}
