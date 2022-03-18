using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Primitives
{
    public class Particle
    {
        public const double G = 6.67e-11;
        public const double CollapseRadius = 0.00175;
        double _trackSegmentSize = 0.000025;
        const double maxSpeed = 300000000;

        protected Vector2d _position;
        protected Vector2d _speed;
        protected Vector2d _acceleration;
        
        protected double _mass;
        protected double _interactionRadius;
        protected double _interactionRadiusSquare;
        protected double _collapseRadiusSqr= MathHelper.Pow(CollapseRadius,2);
        protected int _positionStoryDepth;


        public double TrackSegmentSize { get=>_trackSegmentSize; set=>_trackSegmentSize=value; }
        public Vector2d Position { 
            get => _position; set
            {
                //if (value.X<-1)
                //{
                //    value.X = 1+(value.X+1);
                //}
                //if (value.Y < -1)
                //{
                //    value.Y =   1+(value.Y+1);
                //}
                //if (value.X > 1)
                //{
                //    value.X = (1-value.X)-1;
                //}
                //if (value.Y > 1)
                //{
                //    value.Y = (1-value.Y)-1;
                //}

                _position = value;
            }
        }
        public Vector2d Speed { get => _speed; set => _speed = value; }
        public Vector2d Acceleration { get => _acceleration; set => _acceleration = value; }

        public bool Fixed { get; set; }

        public Double Mass { get => _mass; set => _mass = value; }
        public Double InteractionRadius { 
            get => _interactionRadius; 
            set {
                _interactionRadius = value;
                _interactionRadiusSquare = MathHelper.Pow(value, 2);
            } 
        }

        public Particle(Vector2d position,double mass,double interactionRadius=0.1)
        {
            _position = position;
            _mass = mass;
            InteractionRadius = interactionRadius;
            Fixed = false;
            _positionStoryDepth = 200;
        }
        protected List<Vector2d> _positionStory = new List<Vector2d>();

        public int PositionStoryDepth { get=>_positionStoryDepth; set=>_positionStoryDepth=value; }
        public List<Vector2d> PositionStory { get => _positionStory; }

        Vector2d lastPos;
        public void Tick(double dTime)
        {
            if (dTime > 0.2)
            {
                return;
            }
            _speed += _acceleration;
            if (_speed.Length > maxSpeed)
            {
                _speed = Vector2d.NormalizeFast(_speed) * maxSpeed;
            }

            if (Fixed)
            {
                _speed = new Vector2d(0);
                _acceleration = new Vector2d(0);
            }

            if(Vector2d.DistanceSquared(Position, lastPos)>_trackSegmentSize)
            {
                lastPos = Position;
                _positionStory.Add(Position);
                if (_positionStory.Count > _positionStoryDepth)
                {
                    _positionStory.RemoveAt(0);
                }
            }
            Position += ((_acceleration * MathHelper.Pow(dTime, 2)) / 2.0) + (_speed * dTime);
            //Position += _speed * dTime;
        }

        public List<Vector2d> Accelerations { get; set; }

        public void Interaction(ICollection<Particle> particles,double dTime)
        {
            if (_mass == 0)
                return;
            _acceleration = new Vector2d();
            Accelerations = new List<Vector2d>();
            foreach (var otherParticle in particles)
            {
                if (otherParticle==this)
                {
                    continue;
                }

                Vector2d direction = otherParticle.Position - Position;
                double d2 = direction.LengthSquared;
                if (
                    _mass>=otherParticle._mass &&
                    d2 <= _collapseRadiusSqr && 
                    MathHelper.Abs((_speed - otherParticle._speed).Length) < 0.2
                    )
                {
                    _mass += otherParticle._mass;
                    _speed += ((otherParticle._speed * otherParticle._mass) / _mass);
                    
                    otherParticle._mass = 0;
                }
                else
                {
                    var a=Vector2d.NormalizeFast(direction) * ((G * (_mass * otherParticle._mass) / d2) / (_mass));
                    Accelerations.Add(a);
                    _acceleration += a;
                
                }                
            }

            //Accelerations = Accelerations.Select(x => Vector2d.NormalizeFast(x)*0.3).ToList();
            //Accelerations = Accelerations.Select(x => Vector2d.NormalizeFast(x) * 0.03).ToList();
        }
    }
}
