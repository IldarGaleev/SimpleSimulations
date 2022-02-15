using Gravity.Helpers;
using Gravity.Primitives;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Gravity
{
    public class MainWindow:GameWindow
    {

        List<Particle> particles = new List<Particle>();

        double interractionK = 0.07;

        public MainWindow(GameWindowSettings gameWindowSettings,NativeWindowSettings nativeWindowSettings, List<Particle> particles)
            :base(gameWindowSettings,nativeWindowSettings)
        {
            this.particles = particles;
        }

        Vector2d mouseFactor;
        Vector2i windowCenter;

        Vector2d cursorPosition;

        bool createParticle;
        Vector2d newParticlePos;

        List<(Vector2d, Vector2d ,Color)> accelerations;

        bool isMoveScene = false;

        Vector2d moveStart;

        double scale = 1;
        double viewScale = 1;

        bool track = false;
        bool showVectors = false;

        public bool ShowTracks { get=>track; set => track = value; }
        public bool ShowInteractions { get => showVectors; set => showVectors = value; }

        private double _meterScale;
        private double _worldSize;
        public double WorldSize { 
            get=> _worldSize;
            set {
                _worldSize = value;
                _meterScale = 1.0 / value;
                scale = viewScale * _meterScale;
            } 
        }

        //public double TimeScale { get; set; }

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

            if (isMoveScene)
            {
                moveScene((cursorPosition-moveStart)/scale);
                moveStart = cursorPosition;
            }

            base.OnMouseMove(e);
        }

        private void InitRandomParticles()
        {
            for (int i = 0; i < 10; i++)
            {
                particles.Add(new Particle(Helpers.Randomizer.GetVector()/scale, 1));
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                createParticle = true;
                newParticlePos = cursorPosition;
            }

            if (e.Button==OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right)
            {
                createParticle = false;
            }

            if (e.Button==OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle)
            {
                moveStart = cursorPosition;
                isMoveScene = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (createParticle)
            {
                double mass = 1;
                if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift))
                {
                    mass *= 100;
                }

                if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl))
                {
                    mass *= 1000;
                }
                particles.Add(new Particle(newParticlePos/scale, mass, 1) { 
                    Speed=(cursorPosition-newParticlePos)
                });
                createParticle = false;
            }

            isMoveScene = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            viewScale+=e.OffsetY * 0.01;
            if (viewScale < 0.02)
            {
                viewScale = 0.02;
            }
            if (viewScale > 3)
            {
                viewScale = 3;
            }

            scale = viewScale * _meterScale;
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key==OpenTK.Windowing.GraphicsLibraryFramework.Keys.T)
            {
                track = !track;
            }

            if (e.Key==OpenTK.Windowing.GraphicsLibraryFramework.Keys.A)
            {
                showVectors = !showVectors;
            }

            if (e.Key==OpenTK.Windowing.GraphicsLibraryFramework.Keys.R)
            {
                InitRandomParticles();
            }

            if (e.Key==OpenTK.Windowing.GraphicsLibraryFramework.Keys.E)
            {
                particles.Clear();
            }

            if (e.Key==OpenTK.Windowing.GraphicsLibraryFramework.Keys.M)
            {
                var heavyObject = particles.Find(x => x.Mass == maxMass);
                heavyObject.PositionStory.Clear();
                var deltaPos = heavyObject.Position;
                var heavyObjectSpeed=heavyObject.Speed;
                foreach (var item in particles)
                {
                    item.Speed -= heavyObjectSpeed;
                    item.Position -= deltaPos;
                    for (var i=0;i<item.PositionStory.Count;i++)
                    {
                        item.PositionStory[i] -= deltaPos;
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.S && e.Modifiers == OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers.Control)
            {

                var part = particles.ToList();
                string fileName = $"data/{DateTime.Now.ToString().Replace(':', '-')}.csv";
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var item in part)
                    {
                        writer.WriteLine($"{item.Mass}\t{item.Position.X.ToString(CultureInfo.InvariantCulture)}\t{item.Position.Y.ToString(CultureInfo.InvariantCulture)}\t{item.Speed.X.ToString(CultureInfo.InvariantCulture)}\t{item.Speed.Y.ToString(CultureInfo.InvariantCulture)}\t{(item.Fixed?'1':'0')}");
                    }
                    writer.Close();
                }
                Console.WriteLine($"Saved to \"{fileName}\"");

            }

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(Color.Black);
            VSync = OpenTK.Windowing.Common.VSyncMode.On;

            GL.PointSize(5.0f);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.Enable(EnableCap.Blend);

            CursorVisible = false;
            
        }


        private void moveScene(Vector2d delta)
        {
            foreach (var item in particles)
            {
                int sCount = item.PositionStory.Count;
                for (int i = 0; i < sCount; i++)
                {
                    item.PositionStory[i] += delta;
                }
                item.Position += delta;
            }
        }


        double maxMass = 0;
        double cK = 0;
        double pixelSize = 0;
        TimeSpan epoch = new TimeSpan(0);
        protected override void OnUpdateFrame(FrameEventArgs args)
        {

            double trackSegmentSize = 0.005 / _meterScale;
            
            if (FPS.Tick(args.Time))
            {
                var c = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"FPS           : {FPS.Value}");
                Console.WriteLine($"Particle count: {particles.Count}      ");
                Console.WriteLine($"Max mass      : {maxMass}      ");
                Console.WriteLine($"Time          : {epoch}");
                Console.WriteLine($"Scale         : {scale}");
                Console.SetCursorPosition(c.Left, c.Top);
            }
            epoch += TimeSpan.FromSeconds(args.Time);
                        

            base.OnUpdateFrame(args);
            List<Particle> removed = new List<Particle>();
            accelerations = new List<(Vector2d, Vector2d, Color)>();
            foreach (var item in particles)
            {
                item.TrackSegmentSize = (trackSegmentSize* trackSegmentSize);
                item.Interaction(particles, args.Time);
                item.Tick(args.Time);
                item.InteractionRadius = item.Mass * interractionK;

                if ((item.Accelerations?.Count??0) >0 && showVectors)
                {
                    var max = item.Accelerations.Max(x => x.LengthSquared);

                    foreach (var a in item.Accelerations)
                    {
                        bool isMax = a.LengthSquared == max;
                        var r = (Vector2d.NormalizeFast(a) * (isMax ? 0.04 : 0.02)/_meterScale);
                        accelerations.Add((item.Position, item.Position + r, Color.FromArgb(200, (isMax ? Color.Red : Color.Yellow))));
                    }
                }

            }

            particles.RemoveAll(item => (item.Mass == 0) || ((item.Position.Length*_meterScale) > 300));

            //particles.RemoveAll(item=> (item.Mass==0));

            if (particles.Count > 0)
            {
                maxMass = particles.Max(x => x.Mass);
                cK = 255.0 / maxMass;
                pixelSize = 3 / maxMass;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


            GL.Clear(ClearBufferMask.ColorBufferBit);
           

            if (accelerations != null && showVectors)
            {
                GL.Begin(PrimitiveType.Lines);

                foreach (var item in accelerations)
                {                    
                    GL.Color4(item.Item3);
                    GL.Vertex2(item.Item1*scale);
                    GL.Vertex2(item.Item2*scale);
                }
                GL.End();
            }



            int count = particles.Count;
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < count; i++)
            {
                GL.Color4(Color.FromArgb((int)(particles[i].Mass * cK), 255, 0));
                GL.Vertex2(particles[i].Position*scale);
            }
            GL.End();

            if (track)
            {
                for (int i = 0; i < count; i++)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    

                    int sCount = particles[i].PositionStory.Count;
                    float cK = 1.0f / sCount;
                    for (int j = 0; j < sCount; j++)
                    {
                        GL.Color4(1,1,1,j*cK);
                        GL.Vertex2(particles[i].PositionStory[j] * scale);
                    }
                    GL.Vertex2(particles[i].Position * scale);

                    GL.End();
                }
            }

            if (createParticle)
            {
                GL.Color3(Color.Red);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(newParticlePos);
                GL.Vertex2(cursorPosition);
                GL.End();
            }


            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Blue);
            GL.Vertex2(cursorPosition);
            if (createParticle)
            {
                GL.Color3(Color.Red);
                GL.Vertex2(newParticlePos);
            }
            GL.End();

            GL.Flush();
            SwapBuffers();
        }

    }
}
