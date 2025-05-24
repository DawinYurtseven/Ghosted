Shader "ForgottenColours/VFX/SelectionOverlay"
{
    Properties
    {
        [HDR]_OverlayColour("Overlay Colour", Color) = (1, 1, 1, 0.5) // Red, semi-transparent

        _AlbedoTex ("Albedo Map", 2D) = "bump"{}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent+100"
        }
        LOD 100

        Pass
        {
            Name "OverlayPass"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            ZTest Off
            ZTest Always // Always render on top
            Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Now correctly inside the shader program block
            #pragma shader_feature _DRAWONTOP


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 fragHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 fragWorldPos : TEXCOORD1;
                float3 fragWorldNormal: TEXCOORD2;
            };

            float4 _OverlayColour;

            sampler2D _AlbedoTex;


            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.uv = IN.uv;

                VertexPositionInputs positions = GetVertexPositionInputs(IN.vertex);
                OUT.fragWorldPos = positions.positionWS;
                OUT.fragHCS = positions.positionCS;
                OUT.fragWorldNormal = TransformObjectToWorldNormal(IN.normal);

                return OUT;
            }

            half4 frag(v2f IN) : SV_Target
            {
                float4 c = tex2D(_AlbedoTex, IN.uv);
                float3 v = normalize(_WorldSpaceCameraPos - IN.fragWorldPos);
                half3 n = normalize(IN.fragWorldNormal);
                float fresnel = pow(1.0 - saturate(dot(v, n)), 5.0);
                float outlineIntensity = lerp(0.01, 1.0, fresnel);
                return _OverlayColour * outlineIntensity * c;
            }

            ENDHLSL
        }
    }
}