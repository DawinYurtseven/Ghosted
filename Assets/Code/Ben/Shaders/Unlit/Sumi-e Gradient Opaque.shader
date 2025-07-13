Shader "ForgottenColours/Unlit/Sumi-E Gradient Opaque"
{
    Properties
    {
        // === BASE MATERIAL SETTINGS ===
        [Header(Base Colours)][Space(10)]
        _LightTint("Light Tint", Color) = (1,1,1,1)
        _DarkTint("Dark Tint", Color) = (1,1,1,1)
        _BaseTexture("Base Texture", 2D) = "white" {}
        _Alpha("Alpha", Range(0,1)) = 1

        [Toggle(SWITCH)] _Switch("Swap Colours", float) = 0

        [Header(Emissiveness)][Space(10)]
        [Toggle(USEEMISSIVE)] _UseEmissive("Use Emissive", float) = 0
        _LightEmissiveness("Light Tint Emissiveness", float) = 1
        _DarkEmissiveness("Dark Tint Emissiveness", float) = 1

        [Header(Gradient Controls)][Space(10)]
        _HeightMin("Height Min", Float) = 0.0
        _HeightMax("Height Max", Float) = 5.0

        [Header(Gradient Band Controls)][Space(10)]
        _BandCenter("Band Center", Float) = 0.5
        _BandWidth("Band Width", Float) = 1.0

        [Header(Texture Coordinates)][Space(10)]
        [Enum(UV, 2, Object, 3, World, 4)] _TextureSpace("Texture Space", Float) = 2

    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "Queue" = "Geometry"
        }


        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            ZWrite On

            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature SWITCH
            #pragma shader_feature USEEMISSIVE
            #pragma multi_compile_fog
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD2;
            };

            struct v2f
            {
                float4 fragHCS : SV_POSITION;
                float3 fragWorldPos : TEXCOORD0;
                float3 fragLocalPos : TEXCOORD1;
                float3x3 TBN : TEXCOORD2;
                float3 generatedCoord : TEXCOORD5;
                float2 uv : TEXCOORD6;
            };

            float4 _LightTint;
            float4 _DarkTint;
            float _Alpha;

            float _LightEmissiveness;
            float _DarkEmissiveness;

            float _HeightMin;
            float _HeightMax;

            float _BandCenter;
            float _BandWidth;

            int _TextureSpace;
            float _MixAmount;

            sampler2D _BaseTexture;

            // Function Prototypes
            half3 ProcessNormals(v2f input);

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/TextureCoordinate.hlsl"


            // Vertex shader: transforms vertex data and prepares inputs for the fragment shader
            v2f vert(appdata input)
            {
                v2f output;

                // Transform object-space position into world space and clip space
                VertexPositionInputs positions = GetVertexPositionInputs(input.vertex);
                output.fragLocalPos = input.vertex; // Store local-space position (useful for procedural effects)
                output.fragWorldPos = positions.positionWS; // Store world-space position (used for lighting, shadows, etc.)
                output.fragHCS = positions.positionCS; // Store homogeneous clip space position (used for screen-space effects)

                // Pass transformed texture coordinates to the fragment shader
                output.uv = input.uv;

                return output;
            }

            half4 frag(v2f input) : SV_Target
            {
                float3 texCoord = GetTextureSpace(_TextureSpace, float3(0, 0, 0), input.fragLocalPos, input.fragWorldPos, input.uv);
                float fac = saturate((texCoord.y - _BandCenter) / _BandWidth + 0.5);

                #ifdef SWITCH
                half3 temp = _LightTint.rgb;
                _LightTint.rgb = _DarkTint.rgb;
                _DarkTint.rgb = temp;
                #endif


                half3 rampColour = lerp(_LightTint.rgb, _DarkTint.rgb * tex2D(_BaseTexture, input.uv), fac);

                // === NEW adjustable offset and power ===
                float heightFactor = smoothstep(_HeightMin, _HeightMax, input.fragWorldPos.y);
                half3 finalColour = lerp(rampColour, _LightTint.rgb, heightFactor);

                #ifdef USEEMISSIVE
                    float3 emissive = lerp(_LightTint.rgb * _LightEmissiveness, _DarkTint.rgb * _DarkEmissiveness, fac);
                    finalColour += emissive;
                #endif

                return half4(finalColour, _Alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}