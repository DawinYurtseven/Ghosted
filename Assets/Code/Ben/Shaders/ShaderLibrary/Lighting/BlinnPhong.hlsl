#ifndef BLINN_PHONG_INCLUDED
#define BLINN_PHONG_INCLUDED

half3 GenerateGradient(half brightness, half3 albedoTexture)
{
    brightness = saturate(brightness);

    half3 shadow = _DarkTint;
    half3 midtone = lerp(_DarkTint, _LightTint, 0.5);
    half3 highlight = _LightTint;

    // Define hard bands or smooth transitions
    half3 tone;


    half shadowWeight = 1 - smoothstep(0.25, 0.35, brightness);
    half midtoneWeight = smoothstep(0.25, 0.35, brightness) * (1 - smoothstep(0.65, 0.75, brightness));
    half highlightWeight = smoothstep(0.65, 0.75, brightness);

    tone = shadow * shadowWeight + midtone * midtoneWeight + highlight * highlightWeight;

    return tone * albedoTexture;
}


// Blinn-Phong lighting model
half3 BlinnPhong(half3 n, half3 v, Light mainLight, half3 albedoTexture)
{
    half3 l = mainLight.direction;
    half3 h = normalize(l + v);
    half NdotL = max(dot(n, l), 0);

    half Ia = _k.x;
    Ia = smoothstep(0.05,0.1,Ia);
    half Id = _k.y * NdotL;
    Id = smoothstep(0.05,0.1,Id);
    half Is = 0;

    #ifdef SPECULAR
    Is = _k.z * pow(saturate(dot(h, n)), _SpecularExponent * _SpecularExponent);
    Is = smoothstep(0.05,0.3,Is);
    #endif

    half brightness = Ia * SampleSH(n) + Id * mainLight.color + Is * mainLight.color;

    half3 stylised = GenerateGradient(brightness, albedoTexture);

    half3 ambient = Ia * stylised;
    half3 diffuse = Id * stylised * mainLight.color;
    half3 specular = Is * stylised* mainLight.color;

    return ambient + diffuse + specular;
}

#endif // BLINN_PHONG_INCLUDED
