using TimerCalculation.Data;
using TimerCalculation.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimerCalculation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AvrTimer timer = new AvrTimer();
            TimerApplication app = new TimerApplication();

            Console.ForegroundColor = ConsoleColor.White;
            await Console.Out.WriteLineAsync("AVR 2560 16 bit timer");
            await Console.Out.WriteLineAsync();

            while (true)
            {
                await Console.Out.WriteLineAsync("Vælg funktion:");
                await Console.Out.WriteLineAsync("1 - Normal mode");
                await Console.Out.WriteLineAsync("2 - CTC mode (kommer)");
                await Console.Out.WriteLineAsync("3 - Beregn tid, hvor man kender OCR og prescaler");
                await Console.Out.WriteLineAsync($"4 - Set frekvens (nuværende: {timer.Frekvens}Hz)");

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                Console.Clear();

                if (keyInfo.KeyChar == '1')
                {
                    timer.Mode = Enums.TimerModeEnum.Normal;

                    Console.Write("Indtast den ønskede tid (f.eks. 1s, 1ms, 1us): ");
                    string input = Console.ReadLine();
                    Console.Clear();

                    bool success = TimeHandler.ConvertTimeStringToDouble(input, timer);
                        

                    if (success)
                    {
                        await app.NormalModeCalc(timer);

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        await Console.Out.WriteLineAsync("ERROR: Den indtastede tid kan ikke benyttes.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                }
                else if (keyInfo.KeyChar == '2')
                {
                    timer.Mode = Enums.TimerModeEnum.CTC;
                }
                else if (keyInfo.KeyChar == '3')
                {
                    string PS;
                    string OCR;

                    Console.Write("Indtast prescaler ");
                    string inputPS = Console.ReadLine();
                    Console.Clear();
                    bool success = int.TryParse(inputPS, out int valuePS);

           

                    if (success && Enum.IsDefined(typeof(PrescalerEnum), valuePS))
                    {
                        Console.Write("Indtast OCR ");
                        string inputOCR = Console.ReadLine();
                        Console.Clear();

                        bool successOCR = int.TryParse(inputOCR, out int valueOCR);


                        if (successOCR && valueOCR > 0)
                        {
                            await app.BeregnTid(valuePS, valueOCR, timer.Frekvens);

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            await Console.Out.WriteLineAsync("ERROR: Den indtastede OCR kan ikke benyttes.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        await Console.Out.WriteLineAsync($"ERROR: Den indtastede prescaler ({valuePS}) findes i på timer 1.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (keyInfo.KeyChar == '4')
                {
     
                    await Console.Out.WriteAsync("Ny frekvens: ");
                    string frekvens = await Console.In.ReadLineAsync();

                    int.TryParse(frekvens, out int value);

                    if (value > 0)
                    {
                        timer.Frekvens = value;

                        Console.Clear();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        await Console.Out.WriteLineAsync("ERROR: Den indtastede frekvens kan ikke benyttes - Eksempel på korrekt input: 8000000");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
    }
}