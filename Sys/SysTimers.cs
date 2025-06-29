using System.Timers;

namespace Apollo2.Sys
{
 public static class SysTimers
 {

  public static System.Timers.Timer fifteenSecondTimer;
  public static System.Timers.Timer secondTimer;
  public static System.Timers.Timer minuteTimer;

  public static void CreateTimers()
  {
   fifteenSecondTimer = startTimer(5000);
   secondTimer = startTimer(1000);
   minuteTimer = startTimer(60000);
  }

  private static System.Timers.Timer startTimer(int length)
  {
   System.Timers.Timer time = new System.Timers.Timer(length);
   time.AutoReset = true;
   time.Enabled = true;

   return time;
  }
 }
}
