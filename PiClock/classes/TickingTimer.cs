using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PiClock.classes
{
    class TickingTimer
    {
        DispatcherTimer DTimer { get; set; }
        DateTime DateTime { get; set; }

        public TickingTimer()
        {
            DTimer.Tick += DispatcherTimer_Tick;
            DTimer.Interval = new TimeSpan(0, 0, 1);
            DTimer.Start();
        }

        void DispatcherTimer_Tick(object sender, object e)
        { DateTime = DateTime.Now; }

        public void DispatcherTimerSetup()
        {
            ////Create a new timer and assign the Tick event
            //DispatcherTimer = new DispatcherTimer();
            //DispatcherTimer.Tick += DispatcherTimer_Tick;

            ////Set how often the tick will happen
            //DispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            ////Start the Timer Function
            //DispatcherTimer.Start();
        }
    }
}
