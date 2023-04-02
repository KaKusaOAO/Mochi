using Mochi.Utils;

namespace Mochi.IO.Controllers.Test;

public static class Waveform
{
    public static float Triangle(float x)
    {
        if (x <= MathF.PI / 2)
        {
            return Math.Clamp(x / MathF.PI * 2 , 0, 1);
        }

        x -= MathF.PI / 2;

        if (x <= MathF.PI)
        {
            var n = (MathF.PI - x) / MathF.PI;
            return Math.Clamp(Mth.Lerp(-1, 1, n), -1, 1);
        }

        x -= MathF.PI;
        return Math.Clamp(Mth.Lerp(0, -1, (MathF.PI / 2 - x) / MathF.PI * 2), -1, 1);
    }
}