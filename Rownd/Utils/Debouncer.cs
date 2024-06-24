using System;
/**
 * Example:
 *
 * public void MouseMove(object sender, EventArgs event)
 * {
 *   this.debouncer.Debounce(() => this.DoSomeHeavyTask());
 * }
 *
 */
namespace Rownd.Maui.Utils
{
    using System;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        private Thread thread;
        private volatile Action action;
        private volatile int delay = 0;
        private volatile int frequency;

        public void Debounce(Action action, int delay = 250, int frequency = 10)
        {
            this.action = action;
            this.delay = delay;
            this.frequency = frequency;

            if (thread == null)
            {
                thread = new Thread(() => RunThread());
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void RunThread()
        {
            while (true)
            {
                delay -= frequency;
                Thread.Sleep(frequency);

                if (delay <= 0 && action != null)
                {
                    action();
                    action = null;
                }
            }
        }

        public void Dispose()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }
    }
}
