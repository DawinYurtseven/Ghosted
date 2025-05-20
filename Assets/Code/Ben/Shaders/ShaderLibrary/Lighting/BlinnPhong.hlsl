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
    half NdotL = max(dot(n, l), 0);

    half Ia = _k.x;
    half Id = _k.y * NdotL;

    half brightness = Ia * SampleSH(n) + Id * mainLight.color;

    half3 stylised = GenerateGradient(brightness, albedoTexture);

    half3 ambient = Ia * stylised * SampleSH(n);
    half3 diffuse = Id * stylised * mainLight.color;


    return ambient + diffuse;
}

#endif // BLINN_PHONG_INCLUDED
