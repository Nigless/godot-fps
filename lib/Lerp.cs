
namespace ExtensionMethods
{
    public static class FloatExtension
    {
        public static float Lerp(this float from, float to, float by)
        {
            return from * (1 - by) + to * by;
        }

    }
}

