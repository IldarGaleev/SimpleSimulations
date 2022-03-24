//#define FULLSCREEN


using OpenTK.Windowing.Desktop;
using System.Threading;
using System.Threading.Tasks;

namespace SwarmIntelligence
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();
            Task renderTask = StartRender(ctSource.Token);
            renderTask.Wait();
            //while (true)
            //{
            //    string command = Console.ReadLine();
            //    switch (command)
            //    {
            //        case "reset":
            //            ctSource.Cancel();
            //            renderTask.ContinueWith((x) =>
            //            {
            //                ctSource = new CancellationTokenSource();
            //                StartRender(ctSource.Token);
            //            });

            //            break;
            //        default:
            //            break;
            //    }
            //}
        }


        private static Task StartRender(CancellationToken ct)
        {
            return Task.Factory.StartNew(() =>
            {

                NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
                {
#if FULLSCREEN
                WindowState=OpenTK.Windowing.Common.WindowState.Fullscreen,
#else
                    Size = new OpenTK.Mathematics.Vector2i(1024, 768),
                    WindowState = OpenTK.Windowing.Common.WindowState.Normal,
                    WindowBorder = OpenTK.Windowing.Common.WindowBorder.Fixed,
#endif

                    Flags = OpenTK.Windowing.Common.ContextFlags.Default,
                    Profile = OpenTK.Windowing.Common.ContextProfile.Compatability,

                };
                GameWindowSettings gameWindowSettings = new GameWindowSettings()
                {
                    //IsMultiThreaded = true,
                };


                using (MainWindow mainWindow = new MainWindow(gameWindowSettings, nativeWindowSettings))
                {
                    mainWindow.UpdateFrame += (x) =>
                    {
                        if (ct.IsCancellationRequested)
                        {
                            mainWindow.Dispose();

                        }
                    };

                    mainWindow.Run();

                }
            }, ct);
        }

    }
}
