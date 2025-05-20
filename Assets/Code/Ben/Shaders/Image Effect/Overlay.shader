Shader "ForgottenColours/ScriptableRenderFeatures/Overlay"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Overlay" "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Overlay"
            ZTest Always Cull Off ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _OverlayColor;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return lerp(screenColor, _OverlayColor, _OverlayColor.a); // Blend manually
            }
            ENDHLSL
        }
    }
}