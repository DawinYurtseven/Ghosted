#ifndef BLINN_PHONG_INCLUDED
#define BLINN_PHONG_INCLUDED

// Blinn-Phong lighting model
half3 BlinnPhong(half3 n, half3 v, Light mainLight, half3 albedoTexture)
{
    half3 c = _DiffuseColour * albedoTexture;
    half3 l = mainLight.direction;
    half NdotL = max(dot(n, l), 0);
    half3 h = normalize(l + v);

    half Ia = _k.x;
    half Id = _k.y * NdotL;

    half Is;
    #ifdef SPECULAR
    Is = _k.z * pow(max(dot(h, n), 0.0), _SpecularExponent);
    #else
    Is = 0;
    #endif

    half3 ambient = Ia * c * unity_AmbientSky;
    half3 diffuse = Id * c * mainLight.color;
    half3 specular = Is * mainLight.color;

    return ambient + diffuse + specular;
}

#endif // BLINN_PHONG_INCLUDED
