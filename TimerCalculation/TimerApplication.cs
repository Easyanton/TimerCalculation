using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TimerCalculation.Data;
using TimerCalculation.Enums;

namespace TimerCalculation
{
    public class TimerApplication
    {
        public async Task<bool> NormalModeCalc(AvrTimer timer)
        {

            bool first = true;
            int number = 0;

            foreach (PrescalerEnum PS in PrescalerEnum.GetValues(typeof(PrescalerEnum)))
            {

                int prescaler = (int)PS;

                double ocr = ((timer.Seconds * timer.Frekvens) / prescaler); // - 1;

                PSSettings newSettings = new PSSettings()
                {
                    number = ++number,
                    PS = PS,
                    OCR = (int)Math.Round(ocr),
                };

                if (ocr > timer.Min && ocr < timer.Max)
                {
                    if (first)
                    {
                        newSettings.recommended = true;
                        newSettings.Doable = true;

                        first = false;
                    }
                    else
                    {
                        newSettings.Doable = true;

                    }

                }


                timer.PSSettins.Add(newSettings);

            }

            Console.ForegroundColor = ConsoleColor.Blue;
            await Console.Out.WriteLineAsync("Vælg en opsætning:");
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                foreach (var item in timer.PSSettins)
                {
                    await Console.Out.WriteAsync($"{item.number} - ");

                    if (item.Doable)
                    {
                        if (item.recommended)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            await Console.Out.WriteLineAsync($"Prescaler: {(int)item.PS} - OCR:{item.OCR} - Anbefaldes");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            await Console.Out.WriteLineAsync($"Prescaler: {(int)item.PS} - OCR: {item.OCR}");
                        }
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync($"Prescaler: {(int)item.PS} - #####");
                    }
                }

                if (!timer.PSSettins.Exists(x => x.Doable == true))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Det er ikke muligt at konstruere den ønskede tid");
                    Console.ForegroundColor = ConsoleColor.Red;
                    while (true){}
                }

                // Settings valg.
                await Console.Out.WriteLineAsync();
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                Console.Clear();

                int.TryParse(keyInfo.KeyChar.ToString(), out int indtastetTal);

                if (indtastetTal > 0 && timer.PSSettins.Exists(x => x.number == indtastetTal) && timer.PSSettins.Where(x => x.number == indtastetTal).FirstOrDefault().Doable)
                {
                    timer.SettingsPick = indtastetTal;
                    timer.PS = timer.PSSettins[indtastetTal - 1].PS;
                    timer.OCR = (int)timer.PSSettins[indtastetTal - 1].OCR;
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Out.WriteLineAsync("ERROR: Den prescaler er ikke mulig");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }


            Console.ForegroundColor = ConsoleColor.Blue;
            await Console.Out.WriteLineAsync("Settings:");
            Console.ForegroundColor = ConsoleColor.White;
            await Console.Out.WriteLineAsync($"Mode: {timer.Mode}");
            await Console.Out.WriteLineAsync($"Frekvens: {timer.Frekvens}");
            await Console.Out.WriteLineAsync($"Tid: {timer.Seconds}s");
            await Console.Out.WriteLineAsync($"Prescaler: {(int)timer.PS}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            await Console.Out.WriteLineAsync($"OCR: {timer.OCR}");
            Console.ForegroundColor = ConsoleColor.White;
            await Console.Out.WriteLineAsync();

            Console.ForegroundColor = ConsoleColor.Blue;
            await Console.Out.WriteLineAsync("Kode eksempel for timer1 på AVR 2560:");
            Console.ForegroundColor = ConsoleColor.Yellow;

            StringBuilder code = new StringBuilder();
            code.Append("void Delay()\n");
            code.Append("{\n");
            code.Append($"   TCNT1 = ({timer.Max} - {timer.OCR});\n");
            code.Append("   TCCR1A &= ~0x01; //Normal mode - Kan udelades.\n");

            // PS
            switch (timer.PS)
            {
                case PrescalerEnum.PS1:
                    code.Append("   TCCR1B |= (1<<CS10) ; // Prescaler = 1.\n");
                    break;
                case PrescalerEnum.PS8:
                    code.Append("   TCCR1B |= (1<<CS11) ; // Prescaler = 8.\n");
                    break;
                case PrescalerEnum.PS64:
                    code.Append("   TCCR1B |= (1<<CS11) | (1<<CS10) ; // Prescaler = 64.\n");
                    break;
                case PrescalerEnum.PS256:
                    code.Append("   TCCR1B |= (1<<CS12) ; // Prescaler = 256.\n");
                    break;
                case PrescalerEnum.PS1024:
                    code.Append("   TCCR1B |= (1<<CS12) | (1<<CS10) ; // Prescaler = 1024.\n");
                    break;
                default:
                    break;
            }

            code.Append("\n");
            code.Append("   while ((TIFR1 & (1 << 0)) == 0){}\n");
            code.Append("\n");
            code.Append("   TCCR1B = 0x00;\n");
            code.Append("   TIFR1 = 0x01;\n");
            code.Append("}\n");

            await Console.Out.WriteLineAsync(code);

            Console.ForegroundColor = ConsoleColor.Blue;
            await Console.Out.WriteLineAsync("ISR:");
            Console.ForegroundColor = ConsoleColor.White;

            await Console.Out.WriteLineAsync("ISR(TIMER1_COMPA_vect) { }");


            while (true) { }

        }

        public async Task<bool> BeregnTid(int PS, int OCR, int frekvens)
        {
            double timeRaw = ((PS * (double)(OCR)) / frekvens);
            string timeString = FormatTime(timeRaw);


            Console.ForegroundColor = ConsoleColor.Blue;
            await Console.Out.WriteLineAsync("Settings:");
            Console.ForegroundColor = ConsoleColor.White;

            await Console.Out.WriteLineAsync($"Frekvens: {frekvens}");
            await Console.Out.WriteLineAsync($"Prescaler: {PS}");
            await Console.Out.WriteLineAsync($"OCR: {OCR}");
            await Console.Out.WriteLineAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            await Console.Out.WriteLineAsync($"Der er et delay på: {timeString}");
            Console.ForegroundColor = ConsoleColor.White;

            while (true) { }

            return default(bool);
        }

        private string FormatTime(double timeInSeconds)
        {
            if (timeInSeconds >= 1.0)
            {
                return $"{timeInSeconds:F2} s";
            }
            else if (timeInSeconds >= 1e-3)
            {
                return $"{timeInSeconds * 1e3:F2} ms";
            }
            else
            {
                return $"{timeInSeconds * 1e6:F2} us";
            }
        }
    }
}
