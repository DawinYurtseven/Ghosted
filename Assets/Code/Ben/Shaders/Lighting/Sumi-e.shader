Shader "ForgottenColours/Sumi-e"
{
    Properties
    {
        _DiffuseColour("Diffuse Colour", Color) = (1,1,1,1)

        [Header(Blinn Phong)][Space(10)]
        [Toggle(SPECULAR)] _Specular("Specular Highlight", float) = 1

        _k ("Coefficients (Ambient, Diffuse, Specular)", Vector) = (0.5,0.5,0.8)
        _SpecularExponent("Specular Exponent", Float) = 80
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Lighting/BlinnPhong.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/Voronoi.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/FbmNoise.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/LinearLight.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Colour/ColourRamp.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            // Uniforms
            float4 _DiffuseColour;
            float3 _k;
            float _SpecularExponent;

            v2f vert(appdata input)
            {
                v2f output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(v2f input) : SV_Target
            {
                Light mainLight = GetMainLight();

                // Normalize normal
                half3 n = normalize(input.normalWS);
                half3 l = mainLight.direction;
                float3 v = normalize(_WorldSpaceCameraPos - input.positionWS); // _WorldSpaceCameraPos and input.positionWS are usually large floats

                half fbmScale = 2;
                float fbmRoughness = 0.5;
                int fbmDetail = 15;

                float3 noise = FbmNoise(n, fbmScale, fbmRoughness, fbmDetail);
                float3 blended = LinearLight(n, noise, 0.5);

                float vScale = 2.2;
                float smoothness = 0.5;
                float exponent = 1.0;
                float randomness = 1.0;
                int metricMode = EUCLIDEAN;
                float dist;
                float3 col;
                float3 pos;

                VoronoiSmoothF1_3D(blended * vScale, smoothness, exponent, randomness, metricMode, dist, col, pos);

                half3 lighting = BlinnPhong(pos, l, v, mainLight, _k, _SpecularExponent, _DiffuseColour);

                half3 mapped = ColourRamp(lighting);

                return half4(mapped, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}