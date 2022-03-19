using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    /// <summary>
    /// Get 2D Perlin Noise
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="scale"></param>
    /// <param name="magnitude"></param>
    /// <param name="exponent"></param>
    /// <returns>Noise value</returns>
    public static float Get2D(float x, float y, float scale, float magnitude, float exponent)
    {
        return Mathf.Pow(Mathf.PerlinNoise(x / scale, y / scale) * magnitude, exponent);
    }

    /// <summary>
    /// Get 3D Perlin Noise
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="z">Z position</param>
    /// <param name="scale"></param>
    /// <param name="magnitude"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static float Get3D(float x, float y, float z, float scale, float magnitude, float exponent)
    {
        float AB = Mathf.PerlinNoise(x / scale, y / scale);
        float BC = Mathf.PerlinNoise(y / scale, z / scale);
        float AC = Mathf.PerlinNoise(x / scale, z / scale);

        float BA = Mathf.PerlinNoise(y / scale, x / scale);
        float CB = Mathf.PerlinNoise(z / scale, y / scale);
        float CA = Mathf.PerlinNoise(z / scale, x / scale);

        float ABC = AB + BC + AC + BA + CB + CA;
        return Mathf.Pow((ABC / 6F) * magnitude, exponent);
    }

}
