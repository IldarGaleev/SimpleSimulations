namespace SwarmIntelligence.Helpers
{
    public struct FPS
    {
        private static int _value;
        public static int Value;
        private static double _time;

        public static bool Tick(double deltaTime)
        {
            _value++;
            if ((_time += deltaTime) >= 1)
            {
                Value = _value;
                _value = 0;
                _time = 0;
                return true;
            }
            return false;
        }
    }
}
