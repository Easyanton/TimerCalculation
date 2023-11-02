using TimerCalculation.Enums;

namespace TimerCalculation.Data
{
    public class PSSettings
    {
        public int number { get; set; }
        public bool Doable { get; set; }
        public PrescalerEnum PS { get; set; }
        public int OCR { get; set; }
        public bool recommended { get; set; } = false;
    }
}