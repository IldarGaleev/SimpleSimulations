using OpenTK.Mathematics;

namespace SwarmIntelligence.Primitives
{
    public class Insect
    {
        protected Box2d _listenArea;

        protected Vector2d _position;
        public Vector2d Position
        {
            get => _position;
            set
            {
                _position = value;
                _listenArea.Center = _position;
            }
        }

        protected double _listenRadius = 0;

        public double ListenRadius
        {
            get => _listenRadius;
            set
            {
                _listenRadius = value;
                _listenArea.Size = new Vector2d(value);
            }
        }

        protected Vector2d _acceleration;

        public Vector2d Acceleration
        {
            get => _acceleration;
            set
            {
                _acceleration = value;
                _accelerationMagnitude = value.Length;
            }
        }

        protected double _accelerationMagnitude;
        public double AccelerationMagnitude
        {
            get => _accelerationMagnitude;
            set
            {
                _acceleration = Vector2d.NormalizeFast(_acceleration) * value;
            }
        }



        public Insect(Vector2d position, double listenRadius = 0.01)
        {
            _listenArea = new Box2d();
            Position = position;
            ListenRadius = listenRadius;
        }

        public static Vector2d Rotate(Vector2d vector, double angle)
        {
            double g = MathHelper.DegreesToRadians(angle);
            return Vector2d.Transform(vector, new Quaterniond(g, 0, 0));
        }

        public void Rotate(double angle)
        {
            _acceleration = Rotate(_acceleration, angle);
        }

        public virtual bool Collision(Insect other)
        {
            if (other == this)
            {
                return false;
            }
            return _listenArea.Contains(other._listenArea);
        }

        public virtual bool Collision(Vector2d point)
        {
            return _listenArea.Contains(point);
        }

        public virtual bool Collision(Box2d area)
        {
            return _listenArea.Contains(area);
        }

        public virtual void Step(double timeDelta)
        {
            Position += (_acceleration * timeDelta);
        }

        public void DirectTo(Vector2d pos, double acceleration)
        {
            _acceleration = Vector2d.NormalizeFast(pos - _position) * acceleration;
            _accelerationMagnitude = acceleration;
        }
    }
}
