namespace Signature.Infrastructure
{
    using System;
    using System.Threading;

    public class CustomSemaphoreSlim
    {
        private readonly long maxValue;
        private readonly object locker;
        private long currentValue;

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

        public void Wait()
        {
            lock (this.locker)
            {
                while (Interlocked.Read(ref this.currentValue) == 0)
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
                if (Interlocked.Read(ref this.currentValue) < this.maxValue)
                {
                    Interlocked.Increment(ref this.currentValue);
                    Monitor.Pulse(this.locker);
                }
            }
        }

        public void Reset()
        {
            lock (this.locker)
            {
                this.currentValue = this.maxValue;
            }
        }
    }
}