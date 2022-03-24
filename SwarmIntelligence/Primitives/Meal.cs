using OpenTK.Mathematics;

namespace SwarmIntelligence.Primitives
{
    public class Meal
    {
        public Vector2d Pos { get; set; }
        public bool NotEmpty { get; private set; }
        private double _volume = 0;
        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (_volume > 0.8)
                {
                    NotEmpty = true;
                }
            }
        }

        public bool GetMeal()
        {
            if (_volume >= 0.001 && NotEmpty)
            {
                _volume -= 0.001;
                return true;
            }
            NotEmpty = false;
            return false;
        }
    }
}
