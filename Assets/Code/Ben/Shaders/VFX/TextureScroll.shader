Shader "ForgottenColours/VFX/TextureScroll"
{
    Properties
    {
        [HDR]_BaseColour("Base Colour", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 1.0
    }
    SubShader
    {
        Name "TextureScroll"
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }
        LOD 100

        ZWrite Off
        Blend One One
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


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

            float4 _BaseColour;
            sampler2D _MainTex;
            float _ScrollSpeed;

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
                float2 uv = IN.uv;
                uv.x += -_Time * _ScrollSpeed;

                float4 c = tex2D(_MainTex, uv) * _BaseColour;
                return c;

            }
            ENDHLSL
        }
    }
}