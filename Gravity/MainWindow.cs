using Gravity.Helpers;
using Gravity.Primitives;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Gravity
{
    public class MainWindow : GameWindow
    {

        List<Particle> particles = new List<Particle>();
        Particle cursorParticle = new Particle(cursorPosition, 1) { NonCollidable=true};

        double interractionK = 0.07;

        public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, List<Particle> particles)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            this.particles = particles;
        }

        Vector2d mouseFactor;
        Vector2i windowCenter;

        static Vector2d cursorPosition = Vector2d.Zero;
        Vector2d originPointPosition = Vector2d.Zero;
        Vector2d originPointMarkerWidth = new Vector2d(0.04, 0);
        Vector2d originPointMarkerHeight = new Vector2d(0, 0.04);

        bool createParticle;
        Vector2d newParticlePos;

        List<(Vector2d, Vector2d, Color)> accelerations;

        bool isMoveScene = false;

        Vector2d moveStart;

        double scale = 1;
        double viewScale = 1;

        bool track = false;
        bool showVectors = false;
        bool displayOriginPoint = false;

        bool findFollow = false;
        Particle followTo = null;

        public bool ShowTracks { get => track; set => track = value; }
        public bool ShowInteractions { get => showVectors; set => showVectors = value; }
        public bool DisplayOriginPoint { get => displayOriginPoint; set => displayOriginPoint = value; }

        private double _meterScale;
        private double _worldSize;
        public double WorldSize
        {
            get => _worldSize;
            set
            {
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

        protected void UpdateCursorParticle()
        {
            if (!createParticle)
            {
                cursorParticle.Position = (cursorPosition - originPointPosition * scale) / scale;
            }            
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            cursorPosition = (MousePosition - windowCenter) * mouseFactor;
            UpdateCursorParticle();

            if (isMoveScene && followTo == null)
            {
                moveScene((cursorPosition - moveStart) / scale);
                moveStart = cursorPosition;
            }

            base.OnMouseMove(e);
        }

        private void InitRandomParticles()
        {
            for (int i = 0; i < 10; i++)
            {
                particles.Add(new Particle(Helpers.Randomizer.GetVector() / scale, 1));
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Left)
            {
                createParticle = true;
                newParticlePos = cursorPosition;
            }

            if (e.Button == MouseButton.Right)
            {
                createParticle = false;
            }

            if (e.Button == MouseButton.Middle)
            {
                moveStart = cursorPosition;
                isMoveScene = true;
            }

            if (findFollow && e.Button == MouseButton.Left)
            {
                var res = particles.FirstOrDefault(x => Vector2d.DistanceSquared((x.Position + originPointPosition) * scale, cursorPosition) < (0.024));
                if (res != null)
                {
                    followTo = res;
                }
                createParticle = false;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (createParticle)
            {
                double mass = 1;
                if (KeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    mass *= 100;
                }

                if (KeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    mass *= 1000;
                }

                Vector2d followSpeed = followTo?.Speed ?? Vector2d.Zero;
                particles.Add(new Particle((newParticlePos - originPointPosition * scale) / scale, mass, 1)
                {
                    Speed = (cursorPosition - newParticlePos) + followSpeed
                });
                createParticle = false;
            }

            isMoveScene = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            viewScale += e.OffsetY * 0.01;
            if (viewScale < 0.02)
            {
                viewScale = 0.02;
            }
            if (viewScale > 3)
            {
                viewScale = 3;
            }

            scale = viewScale * _meterScale;
            UpdateCursorParticle();
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Keys.T: /* Toggle displaying tracks */
                    track = !track;
                    break;

                case Keys.A: /* Toggle displaying vectors */
                    showVectors = !showVectors;
                    break;

                case Keys.R: /* Add random particles */
                    InitRandomParticles();
                    break;

                case Keys.E: /* Erase simulation objects */
                    particles.Clear();
                    break;

                case Keys.M: /* Focus on the heaviest object */
                    var heavyObject = particles.Find(x => x.Mass == maxMass);
                    heavyObject.PositionStory.Clear();
                    var deltaPos = heavyObject.Position;
                    var heavyObjectSpeed = heavyObject.Speed;
                    foreach (var item in particles)
                    {
                        item.Speed -= heavyObjectSpeed;
                        item.Position -= deltaPos;
                        for (var i = 0; i < item.PositionStory.Count; i++)
                        {
                            item.PositionStory[i] -= deltaPos;
                        }
                    }
                    originPointPosition = Vector2d.Zero;
                    break;

                case Keys.U: /* Unfollow */
                    followTo = null;
                    break;

                case Keys.F: /* Find follow toggle */
                    findFollow = !findFollow;
                    break;

                case Keys.Escape: /* Exit */
                    Close();
                    break;

                case Keys.D1: /* Reset scale */
                    scale = 1;
                    viewScale = 1;
                    moveScene(-originPointPosition);
                    break;

                case Keys.O: /* Show pivot */
                    displayOriginPoint = !displayOriginPoint;
                    break;

                case Keys.LeftControl:
                case Keys.RightControl:
                case Keys.LeftShift:
                case Keys.RightShift:
                    double mass = 1;
                    if (KeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        mass *= 100;
                    }

                    if (KeyboardState.IsKeyDown(Keys.LeftControl))
                    {
                        mass *= 1000;
                    }
                    cursorParticle.Mass = mass;
                    break;

                default:
                    break;
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
                        writer.WriteLine($"{item.Mass}\t{item.Position.X.ToString(CultureInfo.InvariantCulture)}\t{item.Position.Y.ToString(CultureInfo.InvariantCulture)}\t{item.Speed.X.ToString(CultureInfo.InvariantCulture)}\t{item.Speed.Y.ToString(CultureInfo.InvariantCulture)}\t{(item.Fixed ? '1' : '0')}");
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
            originPointPosition += delta;
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
            foreach (var item in particles.Concat(new[]{ cursorParticle}))
            {
                item.TrackSegmentSize = (trackSegmentSize * trackSegmentSize);
                item.Interaction(particles, args.Time);
                if (item != cursorParticle)
                {
                    item.Tick(args.Time);
                }                
                item.InteractionRadius = item.Mass * interractionK;

                if ((item.Accelerations?.Count ?? 0) > 0 && showVectors)
                {
                    var max = item.Accelerations.Max(x => x.LengthSquared);
                    var k = 0.01/max;

                    foreach (var a in item.Accelerations)
                    {
                        bool isMax = a.LengthSquared == max;
                        var r = (Vector2d.NormalizeFast(a) * (0.02+k*a.LengthSquared) / _meterScale);
                        accelerations.Add((item.Position, item.Position + r, Color.FromArgb(item==cursorParticle?50:200, (isMax ? Color.Red : Color.Yellow))));
                    }                    
                }

            }

            particles.RemoveAll(item => (item.Mass == 0) || ((item.Position.Length * _meterScale) > 300));

            //particles.RemoveAll(item=> (item.Mass==0));

            if (particles.Count > 0)
            {
                maxMass = particles.Max(x => x.Mass);
                cK = 255.0 / maxMass;
                pixelSize = 3 / maxMass;
            }

            if (followTo != null)
            {
                moveScene(-(followTo.Position + originPointPosition) * scale);
            }
        }

        private void DrawOriginPointMarker()
        {
            if (displayOriginPoint)
            {
                GL.Color4(Color.FromArgb(100, Color.Purple));
                var point = originPointPosition * scale;
                GL.Begin(PrimitiveType.Lines);

                GL.Vertex2(point - originPointMarkerWidth);
                GL.Vertex2(point + originPointMarkerWidth);

                GL.Vertex2(point - originPointMarkerHeight);
                GL.Vertex2(point + originPointMarkerHeight);
                GL.End();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


            GL.Clear(ClearBufferMask.ColorBufferBit);

            DrawOriginPointMarker();

            if (accelerations != null && showVectors)
            {
                GL.Begin(PrimitiveType.Lines);

                foreach (var item in accelerations)
                {
                    GL.Color4(item.Item3);
                    GL.Vertex2((item.Item1 + originPointPosition) * scale);
                    GL.Vertex2((item.Item2 + originPointPosition) * scale);
                }
                GL.End();
            }



            int count = particles.Count;
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < count; i++)
            {
                GL.Color4(Color.FromArgb((int)(particles[i].Mass * cK), 255, 0));
                GL.Vertex2((particles[i].Position + originPointPosition) * scale);
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
                        GL.Color4(1, 1, 1, j * cK);
                        GL.Vertex2((particles[i].PositionStory[j] + originPointPosition) * scale);
                    }
                    GL.Vertex2((particles[i].Position + originPointPosition) * scale);

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
            if (findFollow)
            {
                GL.Color3(Color.Orange);
            }
            else
            {
                GL.Color3(Color.Blue);
            }

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
