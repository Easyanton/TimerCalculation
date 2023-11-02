using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerCalculation.Enums;

namespace TimerCalculation.Data
{
    public class AvrTimer
    {
        public int Frekvens { get; set; } = 16000000;
        public TimerModeEnum Mode { get; set; }
        public double Seconds { get; set; }
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 65535;
        public int SettingsPick { get; set; }
        public PrescalerEnum PS { get; set; }
        //public int PS { get; set; }
        public int OCR { get; set; }
        public List<PSSettings> PSSettins { get; set; } = new List<PSSettings>();
    }
}
