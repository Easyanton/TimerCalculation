using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerCalculation.Data;

namespace TimerCalculation
{
    public static class TimeHandler
    {
      
        public static bool ConvertTimeStringToDouble(string time, AvrTimer timer)
        {

            if (string.IsNullOrWhiteSpace(time))
            {
                return false;
            }

            if (time.EndsWith("us"))
            {
                if (double.TryParse(time.Substring(0, time.Length - 2), out double value))
                {
                    timer.Seconds = value / 1000000;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (time.EndsWith("ms"))
            {
                if (double.TryParse(time.Substring(0, time.Length - 2), out double value))
                {
                    timer.Seconds = value / 1000;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (time.EndsWith("s"))
            {
                if (double.TryParse(time.Substring(0, time.Length - 1), out double value))
                {
                    timer.Seconds = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;

        }
    }
}
