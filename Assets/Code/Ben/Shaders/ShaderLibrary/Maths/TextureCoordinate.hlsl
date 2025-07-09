#ifndef INCLUDE_TEXTURECOORDINATES
#define INCLUDE_TEXTURECOORDINATES

half3 GetTextureSpace(int space, float3 normal, float3 localPos, float3 worldPos, float2 uv)
{
    switch (space)
    {
    case 0:
        {
            // Convert object-space position to [0, 1] by remapping the cube's bounds.
            // Unity's default cube is centered at (0,0,0) with extents from -0.5 to 0.5.
            float3 boundsMin = float3(-0.5, -0.5, -0.5);
            float3 boundsMax = float3(0.5, 0.5, 0.5);

            return (localPos - boundsMin) / (boundsMax - boundsMin);
        }
    case 1: return normal;
    case 2: return float3(uv, 0.0);
    case 3: return localPos;
    case 4: return worldPos;
    default: return float3(0.0, 0.0, 0.0);
    }
}


#endif
