#ifndef COLOUR_RAMP_INCLUDED
#define COLOUR_RAMP_INCLUDED

float3 _HexToRgb(int hex)
{
    float r = float((hex >> 16) & 0xFF) / 255.0;
    float g = float((hex >> 8) & 0xFF) / 255.0;
    float b = float(hex & 0xFF) / 255.0;
    return float3(r, g, b);
}

float3 ColourRamp(float3 colour)
{
    float t = dot(colour, float3(0.299, 0.587, 0.114)); // perceived brightness

    // Colour stops
    float3 c0 = _HexToRgb(0x090966);
    float3 c1 = _HexToRgb(0x20208c);
    float3 c2 = _HexToRgb(0x3944db);
    float3 c3 = _HexToRgb(0x6e83f8);
    float3 c4 = _HexToRgb(0xc0cbe9);
    float3 c5 = _HexToRgb(0xebedec);
    float3 c6 = float3(1.0, 1.0, 1.0);

    // Ramp positions
    float p0 = 0.06;
    float p1 = 0.15;
    float p2 = 0.2;
    float p3 = 0.4;
    float p4 = 0.5;
    float p5 = 0.8;
    float p6 = 0.9;

    // Interpolate between colour stops based on luminance t
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
        return lerp(c5, c6, smoothstep(p5, p6, t));
}

#endif // COLOUR_RAMP_INCLUDED
