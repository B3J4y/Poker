using System;

namespace SurfacePoker
{
    /// <summary>
    /// GameLoop benötigt für das durchlaufen der Loop ohne den Tisch zu freezen
    /// </summary>
    public abstract class GameLoop
    {
        protected DateTime lastTick;
        public delegate void UpdateHandler(object sender, TimeSpan elapsed);
        public event UpdateHandler Update;

        public void Tick()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastTick;
            lastTick = now;
            if (Update != null) Update(this, elapsed);
        }

        public virtual void Start()
        {
            lastTick = DateTime.Now;
        }

        public virtual void Stop()
        {
        }
    }
}
