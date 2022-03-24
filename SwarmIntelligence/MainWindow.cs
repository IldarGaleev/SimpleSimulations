using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SwarmIntelligence.Helpers;
using SwarmIntelligence.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SwarmIntelligence
{
    public class MainWindow : GameWindow
    {

        const int antsCount = 900;
        const int mealCount = 15;
        const int finderCount = 10;
        const double acceleration = 0.07;
        const double listenDistance = acceleration * 1.1;
        const double mealDistance = acceleration * 0.5;


        Random random = new Random();
        Vector2d vel = new Vector2d(0, 0.5);
        Quaterniond mealVertexRotate = new Quaterniond(MathHelper.DegreesToRadians(2), 0, 0);

        ParallelOptions opt = new ParallelOptions() { MaxDegreeOfParallelism = 1 };


        double[] mealVertexes = new double[mealCount * 2 * 3];
        double[] mealColors = new double[mealCount * 4 * 3];

        double[] insectVertexes = new double[antsCount * 2];
        double[] insectColors = new double[antsCount * 4];

        static Vector2d a = new Vector2d(0, 0.02);
        static Vector2d b = Insect.Rotate(a, 120);
        static Vector2d c = Insect.Rotate(a, -120);


        Ant[] ants = new Ant[antsCount];
        List<Meal> meals = new List<Meal>(mealCount);
        Dictionary<int, Color> colors = new Dictionary<int, Color>() {
            {0,Color.FromArgb(100,Color.Red) }
        };

        Vector2d mouseFactor;
        Vector2i windowCenter;

        Vector2d cursorPosition;

        private Color getColor(int id)
        {
            if (colors.TryGetValue(id, out Color result))
            {
                return result;
            }
            else
            {
                Color res = Color.FromArgb(140, random.Next(60, 255), random.Next(100, 255), random.Next(30, 255));
                colors.Add(id, res);
                return res;
            }
        }
        public override void Close()
        {

            base.Close();
        }

        public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) :
            base(gameWindowSettings, nativeWindowSettings)
        {

        }


        private Vector2d RandomCoordinate()
        {
            double x = (0.5 - random.NextDouble()) * 2;
            double y = (0.5 - random.NextDouble()) * 2;
            return new Vector2d(x, y);
        }

        protected override void OnLoad()
        {
            Console.WriteLine($"Version  : { GL.GetString(StringName.Version)}");
            Console.WriteLine($"Vendor   : { GL.GetString(StringName.Vendor)}");
            Console.WriteLine($"Renderer : { GL.GetString(StringName.Renderer)}");

            Console.WriteLine($"Multithreaded : { IsMultiThreaded}");

            Console.WriteLine("FPS: 0");

            InitSimulation();

            VSync = VSyncMode.On;


            //VSync = VSyncMode.Off;
            //CursorGrabbed = true;
            //CursorVisible = true;
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.Enable(EnableCap.Blend);

            GL.ClearColor(Color.Black);
            base.OnLoad();
        }


        private void InitSimulation()
        {
            meals = new List<Meal>();
            ants = new Ant[antsCount];

            for (int i = 0; i < mealCount; i++)
            {
                var newMeal = new Meal
                {
                    Pos = RandomCoordinate(),
                    Volume = random.Next(50, 100) / 100.0
                };

                meals.Add(newMeal);
            }

            for (int i = 0; i < mealCount; i++)
            {
                var color = getColor(i);
                int offset = i * 4 * 3;

                mealColors[offset + 0] = color.R / 255.0;
                mealColors[offset + 1] = color.G / 255.0;
                mealColors[offset + 2] = color.B / 255.0;
                mealColors[offset + 3] = i == 0 ? 100 : color.A / 100.0;

                mealColors[offset + 4] = color.R / 255.0;
                mealColors[offset + 5] = color.G / 255.0;
                mealColors[offset + 6] = color.B / 255.0;
                mealColors[offset + 7] = i == 0 ? 100 : color.A / 100.0;

                mealColors[offset + 8] = color.R / 255.0;
                mealColors[offset + 9] = color.G / 255.0;
                mealColors[offset + 10] = color.B / 255.0;
                mealColors[offset + 11] = i == 0 ? 100 : color.A / 100.0;
            }

            for (int i = 0; i < ants.Length; i++)
            {
                ants[i] = new Ant(RandomCoordinate(), listenDistance);
                ants[i].DirectTo(RandomCoordinate(), acceleration);
                ants[i].Speed = acceleration;
                ants[i].Distances = Enumerable.Repeat<double>(1000, mealCount).ToList();
                ants[i].Target = random.Next(1, mealCount - 1);
                ants[i].Finder = (i < finderCount);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            mouseFactor = new Vector2d(2.0 / e.Width, -2.0 / e.Height);
            windowCenter = new Vector2i(e.Width / 2, e.Height / 2);


            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            GL.LoadIdentity();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            cursorPosition = (MousePosition - windowCenter) * mouseFactor;

            base.OnMouseMove(e);
        }




        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (FPS.Tick(args.Time))
            {
                var cur = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 4);
                Title = $"FPS: {FPS.Value}";
                Console.WriteLine(Title);
                Console.SetCursorPosition(cur.Left, cur.Top);
            }

            //double r =  Vector2d.Distance(cursorPosition,ant.Position)*0.5;
            //ant.AccelerationMagnitude = r>0.3?0.3:r;

            Parallel.For(0, mealCount, opt, id =>
             {
                 if (!meals[id].NotEmpty)
                 {
                     meals[id].Volume += 0.016 * args.Time;
                 }

                 if (meals[id].Volume > 1)
                 {
                     meals[id].Volume = 1;
                 }

                 double k = (id == 0 ? 1 : meals[id].Volume) + (meals[id].NotEmpty || id == 0 ? 2 : 0);

                 var vA = (meals[id].Pos + a * k);
                 var vB = (meals[id].Pos + b * k);
                 var vC = (meals[id].Pos + c * k);

                 int offset = id * 2 * 3;

                 mealVertexes[offset + 0] = vA.X;
                 mealVertexes[offset + 1] = vA.Y;
                 mealVertexes[offset + 2] = vB.X;
                 mealVertexes[offset + 3] = vB.Y;
                 mealVertexes[offset + 4] = vC.X;
                 mealVertexes[offset + 5] = vC.Y;

                 int offsetColor = id * 4 * 3;
                 mealColors[offsetColor + 3] = meals[id].Volume + 0.2;
                 mealColors[offsetColor + 7] = meals[id].Volume + 0.2;
                 mealColors[offsetColor + 11] = meals[id].Volume + 0.2;
             });

            int[] availableMeal = meals.GetRange(1, mealCount - 1).Where(x => x.NotEmpty).Select(x => meals.IndexOf(x)).ToArray();

            Parallel.For(0, ants.Length, opt, id =>
             {
                 var ant = ants[id];

                 if ((ant.Position.X > 1 && ant.Acceleration.X > 0) || (ant.Position.X < -1 && ant.Acceleration.X < 0))
                 {
                     ant.Acceleration = new Vector2d(-ant.Acceleration.X, ant.Acceleration.Y);
                 }

                 if ((ant.Position.Y > 1 && ant.Acceleration.Y > 0) || (ant.Position.Y < -1 && ant.Acceleration.Y < 0))
                 {
                     ant.Acceleration = new Vector2d(ant.Acceleration.X, -ant.Acceleration.Y);
                 }

                 ant.Step(args.Time);


                 Parallel.ForEach(ants, opt, a => ant.Meet(a));
                 Parallel.For(0, mealCount, opt, id =>
                   {
                       if (Vector2d.Distance(meals[id].Pos, ant.Position) <= (mealDistance * (ant.Finder ? 2 : 1)))
                       {
                           if (ant.Target == id)
                           {
                               if (ant.Target != 0)
                               {
                                   if (meals[id].GetMeal())
                                   {
                                       ant.NextTarget();
                                   }
                                   else
                                   {
                                       ant.NextTarget(availableMeal);
                                   }

                               }
                               else
                               {
                                   ant.NextTarget(availableMeal);
                               }

                               ant.Rotate(180);
                           }

                           ant.Distances[id] = 0;

                       }

                   });


                 int offset = id * 2;
                 insectVertexes[offset + 0] = ant.Position.X;
                 insectVertexes[offset + 1] = ant.Position.Y;

                 var color = getColor(ant.Target);
                 int colorOffset = id * 4;

                 insectColors[colorOffset + 0] = color.R / 255.0;
                 insectColors[colorOffset + 1] = color.G / 255.0;
                 insectColors[colorOffset + 2] = color.B / 255.0;
                 insectColors[colorOffset + 3] = 0.6;
             }
            );

            base.OnUpdateFrame(args);
        }





        private void DrawMeals()
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.ColorPointer(4, ColorPointerType.Double, 0, mealColors);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(2, VertexPointerType.Double, 0, mealVertexes);

            GL.DrawArrays(PrimitiveType.Triangles, 0, mealCount * 3);

            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);

        }

        private void DrawInsects()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(2, VertexPointerType.Double, 0, insectVertexes);

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.ColorPointer(4, ColorPointerType.Double, 0, insectColors);

            GL.DrawArrays(PrimitiveType.Points, 0, ants.Length);

            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.PointSize(2.0f);
            DrawMeals();
            DrawInsects();

            //GL.Begin(PrimitiveType.Points);
            //for (int i = 0;i<mealCount;i++)
            //{
            //    GL.Vertex2(mealVertexes[i*2], mealVertexes[i * 2+1]);
            //}
            //GL.End();

            GL.Flush();
            SwapBuffers();
            base.OnRenderFrame(args);
        }


    }
}
