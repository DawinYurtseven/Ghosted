// https://github.com/kinakomoti-321/Voronoi_textures/blob/main/VoronoiTexture/Voronoi.glsl

#ifndef VORONOI_INCLUDED
#define VORONOI_INCLUDED

// === Distance Metric Constants ===
#define EUCLIDEAN 1
#define MANHATTAN 2
#define CHECYSHEV 3
#define MINKOWSKI 4

// === Hash 3D → 3D ===
float3 _Hash3DTo3D(float3 k)
{
    float3 st = float3(
        dot(k, float3(103.0, 393.0, 293.0)),
        dot(k, float3(593.0, 339.0, 299.0)),
        dot(k, float3(523.0, 334.0, 192.0))
    );
    return frac(sin(st) * 2304.2002);
}

// === Distance Metrics ===

// Manhattan
float _Manhattan3D(float3 p)
{
    return abs(p.x) + abs(p.y) + abs(p.z);
}

// Chebyshev
float _Chebyshev3D(float3 p)
{
    return max(max(abs(p.x), abs(p.y)), abs(p.z));
}

// Minkowski
float _Minkowski3D(float3 k, float p)
{
    float3 k1 = pow(abs(k), float3(p, p, p));
    return pow(dot(k1, float3(1.0, 1.0, 1.0)), 1.0 / p);
}

// Euclidean
#define Euclidean(p) length(p)

// Distance selector
float _VoronoiDistance3D(float3 a, float3 b, int metricMode, float exponent)
{
    float3 delta = b - a;
    if (metricMode == EUCLIDEAN)
        return Euclidean(delta);
    else if (metricMode == MANHATTAN)
        return _Manhattan3D(delta);
    else if (metricMode == CHECYSHEV)
        return _Chebyshev3D(delta);
    else if (metricMode == MINKOWSKI)
        return _Minkowski3D(delta, exponent);
    else
        return 0.0;
}

// === Voronoi Smooth F1 (3D) ===
void VoronoiSmoothF1_3D(float3 coord, float smoothness, float exponent, float randomness, int metricMode, inout float outDistance, inout float3 outColour, inout float3 outPosition)
{
    float3 cellPosition = floor(coord);
    float3 localPosition = coord - cellPosition;

    float smoothDistance = 8;
    float3 smoothColour = float3(0.0, 0.0, 0.0);
    float3 smoothOffset = float3(0.0, 0.0, 0.0);

    for (int j = -2; j <= 2; ++j)
    {
        for (int i = -2; i <= 2; ++i)
        {
            for (int k = -2; k <= 2; ++k)
            {
                float3 cellOffset = float3(i, j, k);
                float3 pointPosition = cellOffset + _Hash3DTo3D(cellPosition + cellOffset) * randomness;

                float distanceToPoint = _VoronoiDistance3D(pointPosition, localPosition, metricMode, exponent);

                float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / smoothness);
                float correction = smoothness * h * (1.0 - h);
                smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correction;

                correction /= (1.0 + 3.0 * smoothness);
                float3 cellColour = _Hash3DTo3D(cellPosition + cellOffset);

                smoothColour = lerp(smoothColour, cellColour, h) - correction;
                smoothOffset = lerp(smoothOffset, pointPosition, h) - correction;
            }
        }
    }

    outDistance = smoothDistance;
    outPosition = cellPosition + smoothOffset;
    outColour = smoothColour;
}

#endif
