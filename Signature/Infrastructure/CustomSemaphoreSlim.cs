namespace Signature.Infrastructure
{
    using System;
    using System.Threading;

    public class CustomSemaphoreSlim
    {
        private readonly int maxValue;
        private readonly object locker;
        private volatile int currentValue;

        public CustomSemaphoreSlim(
            int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            this.maxValue = maxValue;
            this.currentValue = maxValue;
            this.locker = new object();
        }

        public void WaitOne()
        {
            lock (this.locker)
            {
                while (this.currentValue == 0)
                {
                    Monitor.Wait(this.locker);
                }

                Interlocked.Decrement(ref this.currentValue);
            }
        }

        public void Release()
        {
            lock (this.locker)
            {
                if (this.currentValue < this.maxValue)
                {
                    Interlocked.Increment(ref this.currentValue);
                    Monitor.Pulse(this.locker);
                }
            }
        }
    }
}