using System;
using System.Windows.Media;

namespace SurfacePoker
{
    /// <summary>
    /// GameLoop benötigt für das durchlaufen der Loop ohne den Tisch zu freezen
    /// </summary>
    public class CompositionTargetGameLoop : GameLoop
    {
        public CompositionTargetGameLoop()
        {
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            base.Tick();            
        }

        public override void Start()
        {
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            base.Start();
        }

        public override void Stop()
        {
            CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
            base.Stop();
        }
    }
}
