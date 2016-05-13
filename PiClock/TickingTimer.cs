using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PiClock
{
    class TickingTimer
    {
        DispatcherTimer Timer;

        public void StartTimer(TimeSpan ts)
        {
            
        }

        public void StopTimer()
        {

        }

        public static DateTime DispatcherTimerSetup()
        {
            //DateTime currentTime = DateTime.Now;

            ////Create a new timer and assign the Tick event
            //dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += dispatcherTimer_Tick;

            ////Set how often the tick will happen
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            ////Start the Timer Function
            //dispatcherTimer.Start();
            return DateTime.Now;
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            //DateTime currentTime = DateTime.Now;
            //textBox1.Text = currentTime.ToString();
        }
    }
}

