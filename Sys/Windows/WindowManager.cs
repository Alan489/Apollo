namespace Apollo2.Sys.Windows
{
 public static class WindowManager
 {
  public static Window? activeWindow = null;
  public static List<Window> Windows = new List<Window>();
  public static List<Modal> Modals = new List<Modal>();

  public delegate void WindowChangeEventHandler(windowEvent we);

  public static event WindowChangeEventHandler WindowChanged;

  public static void openOrMaximizeApplication(string app, string param = "")
  {
   if (activeWindow != null && activeWindow.WINDOWTYPE == app)
    return;

   Window? w = Windows.FirstOrDefault(w => w.WINDOWTYPE == app);
   bool created = false;
   if (w == null)
   {
    created = true;
    w = new Window();
    Windows.Add(w);
   }

   w.WINDOWTYPE = app;
   w.property = param;
   w.Title = app;

   activeWindow = w;

   windowEvent we = new windowEvent();
   we.window = w;

   if (created)
    we.type = windowEvent.WindowEventType.opened;
   else
    we.type = windowEvent.WindowEventType.maximized;



   WindowChanged?.Invoke(we);

  }

  public static void RequestRefresh()
  {
   windowEvent we = new windowEvent();
   we.window = activeWindow;

   we.type = windowEvent.WindowEventType.maximized;
   WindowChanged?.Invoke(we);

  }

  public static void openPage(Window window)
  {
   if (Windows.Contains(window))
   {
    Maximize(window);
    return;
   }

   Window? w = Windows.FirstOrDefault(w => w.Title == window.Title && w.WINDOWTYPE == window.WINDOWTYPE && w.property == window.property);
   if (w == null)
    Windows.Add(window);

   Maximize(w ?? window);
  }

  public static void openPage(string page, string title, string param = "")
  {
   Window? w = Windows.FirstOrDefault(w => w.Title == title && w.WINDOWTYPE == page && w.property == param);
   bool created = false;
   if (w == null || page == "NEWINC")
   {
    created = true;
    w = new Window();
    Windows.Add(w);
   }

   w.WINDOWTYPE = page;
   w.property = param ?? "";
   w.Title = title;

   activeWindow = w;

   windowEvent we = new windowEvent();
   we.window = w;

   if (created)
    we.type = windowEvent.WindowEventType.opened;
   else
    we.type = windowEvent.WindowEventType.maximized;



   WindowChanged?.Invoke(we);
  }

  //pointer-events:none
  public static void AddModal(string type, string title, string message)
  {
   Modal m = new Modal();
   m.type = type;
   m.title = title;
   m.message = message;
   m.created = DateTime.Now;
   Modals.Add(m);
  }

  public static void Minimize(Window window)
  {
   if (activeWindow == window)
    activeWindow = null;

   windowEvent we = new windowEvent();
   we.type = windowEvent.WindowEventType.minimized;
   we.window = window;

   WindowChanged?.Invoke(we);
  }

  public static void closePage(Window window)
  {
   if (activeWindow == window)
    activeWindow = null;

   Windows.Remove(window);

   windowEvent we = new windowEvent();
   we.type = windowEvent.WindowEventType.closed;
   we.window = window;

   WindowChanged?.Invoke(we);

   if (Windows.Count > 0)
    activeWindow = Windows[0];

   windowEvent we1 = new windowEvent();
   we1.type = windowEvent.WindowEventType.maximized;
   we1.window = activeWindow;

   WindowChanged?.Invoke(we1);
  }

  public static void Maximize(Window window)
  {
   if (activeWindow != null)
    Minimize(activeWindow);

   if (!Windows.Contains(window))
    return;

   activeWindow = window;


   windowEvent we = new windowEvent();
   we.type = windowEvent.WindowEventType.maximized;
   we.window = window;

   WindowChanged?.Invoke(we);

  }


 }

 public class Modal
 {
  public DateTime created;
  public string type;
  public string title;
  public string message;
 }

  public class windowEvent
  {
   public enum WindowEventType { opened, minimized, maximized, closed }
   public Window window;
   public WindowEventType type;

  }
}
