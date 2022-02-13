using Gravity.Primitives;
using Microsoft.VisualBasic.FileIO;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Gravity
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings { 
                //RenderFrequency=50,
                //UpdateFrequency=50,
            };
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                Flags = OpenTK.Windowing.Common.ContextFlags.Default,
                WindowBorder = OpenTK.Windowing.Common.WindowBorder.Resizable,
                WindowState = OpenTK.Windowing.Common.WindowState.Normal,
                //WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen,
                Size = new OpenTK.Mathematics.Vector2i(1740, 1024),
                //Size = new OpenTK.Mathematics.Vector2i(800, 800),
                Profile = OpenTK.Windowing.Common.ContextProfile.Compatability,
            };
            //try
            {
                using (TextFieldParser textFieldParser = new TextFieldParser(new FileStream("Data/celestial_init.csv",FileMode.OpenOrCreate)))
                {
                    textFieldParser.CommentTokens = new[] { "#", "//" };
                    textFieldParser.Delimiters = new[] { "\t", ";"," " };

                    List<Particle> particles = new List<Particle>();

                    while (!textFieldParser.EndOfData)
                    {
                        var data = textFieldParser.ReadFields();
                        try
                        {
                            double mass = double.Parse(data[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                            double posX = double.Parse(data[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                            double posY = double.Parse(data[2].Replace(',', '.'), CultureInfo.InvariantCulture);
                            double speedX = double.Parse(data[3].Replace(',', '.'), CultureInfo.InvariantCulture);
                            double speedY = double.Parse(data[4].Replace(',', '.'), CultureInfo.InvariantCulture);
                            bool fix = data[5] == "1" || data[5] == "y";
                            Particle newCelestial = new Particle(new Vector2d(posX, posY), mass, 1.0)
                            {
                                Speed=new Vector2d(speedX,speedY),
                                Fixed = fix
                            };

                            particles.Add(newCelestial);
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"File parse line error: {e.Message}");
                            Console.ResetColor();
                        }
                    }
                    if (particles.Count==0)
                    {
                        throw new Exception("No celestials for simulate!");
                    }
                    Console.Clear();
                    using (MainWindow mainWindow = new MainWindow(gameWindowSettings, nativeWindowSettings, particles))
                    {
                        mainWindow.Title = "Gravity simulation";
                        mainWindow.Run();
                    }
                }
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    throw e;
            //}

        }
    }
}
