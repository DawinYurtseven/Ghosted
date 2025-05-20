Shader "ForgottenColours/Sumi-e Skybox"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.05, 0.05, 0.05, 1)
        _BottomColor("Bottom Color", Color) = (0.95, 0.95, 0.95, 1)
        _PaperTex("Paper Texture", 2D) = "white" {}
        _VignettePower("Vignette Power", Float) = 1.5
        _TexStrength("Paper Texture Strength", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100
        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _BottomColor;
            sampler2D _PaperTex;
            float4 _PaperTex_ST;
            float _VignettePower;
            float _TexStrength;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = (o.positionHCS.xy / o.positionHCS.w) * 0.5 + 0.5;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // Vertical gradient
                half4 col = lerp(_BottomColor, _TopColor, uv.y);

                // Vignette (darken edges)
                float2 centeredUV = (uv - 0.5) * 2;
                float vignette = 1.0 - pow(dot(centeredUV, centeredUV), _VignettePower);
                col.rgb *= saturate(vignette);

                // Paper texture overlay
                half3 paper = tex2D(_PaperTex, uv).rgb;
                col.rgb = lerp(col.rgb, col.rgb * paper, _TexStrength);

                return col;
            }
            ENDHLSL
        }
    }
}