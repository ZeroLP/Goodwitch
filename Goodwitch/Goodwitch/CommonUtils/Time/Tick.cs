using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goodwitch.CommonUtils.Time
{
    public delegate void TickElapsedEvent();

    internal class Tick
    {
        private static Tick Instance { get; set; } = new Tick();
        private static Timer Ticker;

        private Tick()
        {
            Ticker = new Timer(0.1f);
            Ticker.Elapsed += OnTickElapsed;
            Ticker.Start();
            Instance = this;
        }

        public static ToggleResult Toggle(TickElapsed Value)
        {
            if (OnTick != null)
            {
                foreach (TickElapsed s in OnTick.GetInvocationList())
                {
                    if (s == Value)
                    {
                        OnTick -= Value;
                        return ToggleResult.Disabled;
                    }
                }
            }
            OnTick += Value;
            return ToggleResult.Enabled;
        }

        internal static event TickElapsed OnTick;

        private static void OnTickElapsed(object Sender, TimerElapsedEventArgs e)
        {
            OnTick?.Invoke();
        }
    }

    internal enum ToggleResult
    {
        Disabled,
        Enabled,
    }

    internal delegate void TickElapsed();
}
