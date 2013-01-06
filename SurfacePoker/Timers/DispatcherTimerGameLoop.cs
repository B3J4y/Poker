using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SurfacePoker
{
    /// <summary>
    /// GameLoop benötigt für das durchlaufen der Loop ohne den Tisch zu freezen
    /// </summary>
    public class DispatcherTimerGameLoop : GameLoop
    {
        DispatcherTimer t = new DispatcherTimer();
        public DispatcherTimerGameLoop() : this(0)
        {
        }

        public DispatcherTimerGameLoop(double milliseconds)
        {
            t.Interval = TimeSpan.FromMilliseconds(milliseconds);
            t.Tick += new EventHandler(t_Tick);
        }

        public override void Start()
        {
            t.Start();
            base.Start();
        }

        public override void Stop()
        {
            t.Stop();
            base.Stop();
        }

        void t_Tick(object sender, EventArgs e)
        {
            base.Tick();
        }

    }
}
