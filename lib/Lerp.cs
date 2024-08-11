class Lerp
{
    public static float Transit(float from, float to, float by)
    {
        return from * (1 - by) + to * by;
    }

}