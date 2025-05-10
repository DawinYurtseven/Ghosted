#ifndef BLINN_PHONG_INCLUDED
#define BLINN_PHONG_INCLUDED

void GetSafeTints(out half3 safeDark, out half3 safeLight)
{
    safeDark  = _DarkTint;
    safeLight = _LightTint;

    // Clamp safeDark to be no brighter than safeLight in each channel
    safeDark.r = min(safeDark.r, safeLight.r);
    safeDark.g = min(safeDark.g, safeLight.g);
    safeDark.b = min(safeDark.b, safeLight.b);

    // Clamp safeLight to be no darker than safeDark in each channel
    safeLight.r = max(safeLight.r, safeDark.r);
    safeLight.g = max(safeLight.g, safeDark.g);
    safeLight.b = max(safeLight.b, safeDark.b);
}


half3 GenerateGradient(half brightness, half3 albedoTexture)
{
    // Clamp brightness safely
    brightness = saturate(brightness);

    half3 dark, light;
    GetSafeTints(dark, light); // enforce valid bounds

    // Build the ramp
    half3 c0 = dark;
    half3 c1 = lerp(dark, light, 0.2);
    half3 c2 = lerp(dark, light, 0.4);
    half3 c3 = lerp(dark, light, 0.6);
    half3 c4 = lerp(dark, light, 0.8);
    half3 c5 = light;

    // Fixed thresholds for now
    half p0 = 0.0, p1 = 0.1, p2 = 0.25, p3 = 0.45, p4 = 0.7, p5 = 1.0;

    half3 tone;
    if (brightness < p1)
        tone = lerp(c0, c1, smoothstep(p0, p1, brightness));
    else if (brightness < p2)
        tone = lerp(c1, c2, smoothstep(p1, p2, brightness));
    else if (brightness < p3)
        tone = lerp(c2, c3, smoothstep(p2, p3, brightness));
    else if (brightness < p4)
        tone = lerp(c3, c4, smoothstep(p3, p4, brightness));
    else if (brightness < p5)
        tone = lerp(c4, c5, smoothstep(p4, p5, brightness));
    else
        tone = c5;

    // Finally apply albedo detail to stylised ramp colour
    return tone * albedoTexture;
}



// Blinn-Phong lighting model
half3 BlinnPhong(half3 n, half3 v, Light mainLight, half3 albedoTexture)
{
    half3 l = mainLight.direction;
    half NdotL = max(dot(n, l), 0);
    half3 h = normalize(l + v);

    half Ia = _k.x;
    half Id = _k.y * NdotL;

    half Is;
    // #ifdef SPECULAR
    // Is = _k.z * pow(max(dot(h, n), 0.0), _SpecularExponent);
    // #else
    // #endif
    Is = 0;

    half3 brightness = saturate(Ia + Id + Is);

    half3 stylised = GenerateGradient(brightness, albedoTexture);

    half3 ambient = Ia * stylised * SampleSH(n);;
    half3 diffuse = Id * stylised * mainLight.color;
    half3 specular = Is * mainLight.color;


    return ambient + diffuse + specular;
}

#endif // BLINN_PHONG_INCLUDED
