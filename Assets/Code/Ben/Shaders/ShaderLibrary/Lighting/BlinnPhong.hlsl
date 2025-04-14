#ifndef BLINN_PHONG_INCLUDED
#define BLINN_PHONG_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// Blinn-Phong lighting model
half3 BlinnPhong(half3 n, half3 l, half3 v, Light mainLight, float3 k, float specularExponent, float3 diffuseColour)
{
    half NdotL = max(dot(n, l), 0);
    half3 h = normalize(l + v);

    half Ia = k.x;
    half Id = k.y * NdotL;
    half Is = k.z * pow(max(dot(h, n), 0.0), specularExponent);

    half3 ambient = Ia * diffuseColour;
    half3 diffuse = Id * diffuseColour * mainLight.color;
    half3 specular = Is * mainLight.color;

    return ambient + diffuse + specular;
}

#endif // BLINN_PHONG_INCLUDED
