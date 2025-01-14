using SystemStopwatch = System.Diagnostics.Stopwatch;

namespace Toon.Extend
{
    public enum SWFormat { H, h, Hour, hour, M, m, Minute, minute, S, s, Second, second, MS, ms }
    public class Stopwatch
    {
        SystemStopwatch sw;
        public Stopwatch() => sw = new();
        public Stopwatch(bool b) { sw = new(); sw.Start(); }
        public void Start() => sw.Start();
        public void Stop() => sw.Stop();
        public void Restart() => sw.Restart();
        public void Reset() => sw.Reset();
        public bool IsOngoing() => sw.IsRunning;
        public int Hour() => sw.Elapsed.Hours;
        public float HourF(int digit = 6) => numeric.round((float)sw.Elapsed.TotalHours, digit);
        public int Minute() => sw.Elapsed.Minutes;
        public float MinuteF(int digit = 6) => numeric.round((float)sw.Elapsed.TotalMinutes, digit);
        public int Second() => sw.Elapsed.Seconds;
        public float SecondF(int digit = 6) => numeric.round((float)sw.Elapsed.TotalSeconds, digit);
        public int MSecond() => sw.Elapsed.Milliseconds;
        public float MSecondF(int digit = 6) => numeric.round((float)sw.Elapsed.TotalMilliseconds, digit);
        public string Spent() => sw.Elapsed.ToString();
        public int Spent(SWFormat style)
        {
            switch (style)
            {
                case SWFormat.H: case SWFormat.h: case SWFormat.Hour: case SWFormat.hour: return Hour();
                case SWFormat.M: case SWFormat.m: case SWFormat.Minute: case SWFormat.minute: return Minute();
                case SWFormat.S: case SWFormat.s: case SWFormat.Second: case SWFormat.second: return Second();
                case SWFormat.MS: case SWFormat.ms: return MSecond();
                default: return 0;
            }
        }
        public float SpentF(SWFormat style)
        {
            switch (style)
            {
                case SWFormat.H: case SWFormat.h: case SWFormat.Hour: case SWFormat.hour: return HourF();
                case SWFormat.M: case SWFormat.m: case SWFormat.Minute: case SWFormat.minute: return MinuteF();
                case SWFormat.S: case SWFormat.s: case SWFormat.Second: case SWFormat.second: return SecondF();
                case SWFormat.MS: case SWFormat.ms: return MSecondF();
                default: return 0f;
            }
        }
    }
}
