using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Widget
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }

    class WidgetJobPunchesPaired : Widget
    {
        //public string Id { get; set; }
        //public string TimeStamp { get; set; }
        //public string Type { get; set; }
    }

    class WidgetRegularPunchPaired : Widget
    {
        public string TimeStampIn { get; set; }
        public string TimeStampOut { get; set; }
    }

    class WidgetRegularPunchOpen : Widget
    {
        //These may not even be needed
    }


    class WidgetJobPunchOpen : Widget
    {
    }

    class WidgetJobPunchPaired : Widget
    {
    }
}
