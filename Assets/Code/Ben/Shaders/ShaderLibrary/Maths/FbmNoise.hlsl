#ifndef FBM_NOISE_INCLUDED
#define FBM_NOISE_INCLUDED

float _Random(float3 st)
{
    return frac(sin(dot(st, float3(12.9898, 78.233, 45.164))) * 43758.5453123);
}

// Adapted from: Morgan McGuire (https://www.shadertoy.com/view/4dS3Wd)
float _Noise(float3 st)
{
    float3 i = floor(st);
    float3 f = frac(st);

    float a  = _Random(i);
    float b  = _Random(i + float3(1.0, 0.0, 0.0));
    float c  = _Random(i + float3(0.0, 1.0, 0.0));
    float d  = _Random(i + float3(1.0, 1.0, 0.0));
    float e  = _Random(i + float3(0.0, 0.0, 1.0));
    float f1 = _Random(i + float3(1.0, 0.0, 1.0));
    float g  = _Random(i + float3(0.0, 1.0, 1.0));
    float h  = _Random(i + float3(1.0, 1.0, 1.0));

    float3 u = f * f * (3.0 - 2.0 * f);

    float x00 = lerp(a, b, u.x);
    float x10 = lerp(c, d, u.x);
    float x01 = lerp(e, f1, u.x);
    float x11 = lerp(g, h, u.x);

    float y0 = lerp(x00, x10, u.y);
    float y1 = lerp(x01, x11, u.y);

    return lerp(y0, y1, u.z);
}

float3 FbmNoise(float3 st, float scale, float roughness, int detail)
{
    st *= scale;

    float value = 0.0;
    float amplitude = 0.5;

    for (int i = 0; i < detail; ++i)
    {
        value += amplitude * _Noise(st);
        st *= 2.0;
        amplitude *= roughness;
    }

    return float3(value,value,value);
}

#endif