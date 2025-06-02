#ifndef LINEAR_LIGHT_INCLUDED
#define LINEAR_LIGHT_INCLUDED

float3 LinearLight(float3 a, float3 b, float factor)
{
    float3 blended = clamp(2.0 * a * b + a - 1.0, 0.0, 1.0);
    return lerp(a, blended, factor);
}

#endif // LINEAR_LIGHT_INCLUDED
