using KaLib.Utils;

namespace KaLib.IO.Controllers.DualSense;

public class RumbleEmulator
{
    // Oscillator
    private float _leftPos;
    private float _rightPos;
    private float _leftStrO;
    private float _rightStrO;

    public float SampleRate { get; set; } = 48000f;

    public Func<float, float> WaveGenerator { get; set; } = MathF.Sin;
    
    public void Emulate(float[] left, float[] right, float leftFreq, float leftStrength, float rightFreq,
        float rightStrength)
    {
        var len = left.Length;
        leftStrength = Math.Clamp(leftStrength, 0, 1);
        rightStrength = Math.Clamp(rightStrength, 0, 1);

        for (var i = 0; i < len; i++)
        {
            _leftPos += leftFreq / SampleRate * MathF.PI * 2;
            _rightPos += rightFreq / SampleRate * MathF.PI * 2;
            _leftPos %= MathF.PI * 2;
            _rightPos %= MathF.PI * 2;
            
            var leftVal = WaveGenerator(_leftPos) * MathHelper.Lerp(_leftStrO, leftStrength, i * 1f / len);
            var rightVal = WaveGenerator(_rightPos) * MathHelper.Lerp(_rightStrO, rightStrength, i * 1f / len);
            left[i] = leftVal;
            right[i] = rightVal;
        }
        _leftStrO = leftStrength;
        _rightStrO = rightStrength;
    }
}