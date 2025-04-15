#ifndef COLOUR_RAMP_INCLUDED
#define COLOUR_RAMP_INCLUDED

float3 ColourRamp(float3 colour)
{
    float t = saturate(dot(colour, float3(0.299, 0.587, 0.114))); // perceived brightness

    // 6 colour stops
    half3 c0 = _DarkTone.rgb;
    half3 c1 = _MidDarkTone.rgb;
    half3 c2 = _MiddleTone.rgb;
    half3 c3 = _MidLightTone.rgb;
    half3 c4 = _LightTone.rgb;
    half3 c5 = _Highlight.rgb;

    // 6 positions stored across two Vector3s
    half p0 = _RampPositions0.x;
    half p1 = _RampPositions0.y;
    half p2 = _RampPositions0.z;
    half p3 = _RampPositions1.x;
    half p4 = _RampPositions1.y;
    half p5 = _RampPositions1.z;

    // Interpolation across 6 positions and 6 colours
    if (t < p1)
        return lerp(c0, c1, smoothstep(p0, p1, t));
    else if (t < p2)
        return lerp(c1, c2, smoothstep(p1, p2, t));
    else if (t < p3)
        return lerp(c2, c3, smoothstep(p2, p3, t));
    else if (t < p4)
        return lerp(c3, c4, smoothstep(p3, p4, t));
    else if (t < p5)
        return lerp(c4, c5, smoothstep(p4, p5, t));
    else
        return c5;
}

#endif // COLOUR_RAMP_INCLUDED
