Shader "ForgottenColours/Sumi-E"
{
    Properties
    {
        // === BASE MATERIAL SETTINGS ===
        [Header(Base Colour)][Space(10)]
        _DiffuseColour("Diffuse Colour", Color) = (1,1,1,1)
        _AlbedoTex ("Albedo Texture", 2D) = "white"{}
        _NormalTex ("Normal Map", 2D) = "bump"{}
        _NormalStrength ("Normal Strength", Float) = 1

        // === BLINN-PHONG LIGHTING ===
        [Header(Blinn Phong Lighting)][Space(10)]
        [Toggle(SPECULAR)] _Specular("Enable Specular Highlight", Float) = 1
        _k ("k Coefficients (Ambient, Diffuse, Specular)", Vector) = (0.5, 0.5, 0.8)
        _SpecularExponent("Specular Exponent", Float) = 80


        // === COLOUR RAMP ===
        [Header(Colour Ramp Tones)][Space(10)]
        _DarkTone ("Dark Tone", Color) = (0.05, 0.05, 0.2, 1)
        _MidDarkTone ("Mid Dark Tone", Color) = (0.1, 0.1, 0.3, 1)
        _MiddleTone ("Middle Tone", Color) = (0.25, 0.3, 0.7, 1)
        _MidLightTone ("Mid Light Tone", Color) = (0.5, 0.6, 0.9, 1)
        _LightTone ("Light Tone", Color) = (0.8, 0.85, 0.95, 1)
        _Highlight ("Highlight", Color) = (1.0, 1.0, 1.0, 1)

        [Header(Colour Ramp Positions)][Space(5)]
        _RampPositions0 ("Positions p0–p2 (xyz)", Vector) = (0.2, 0.4, 0.6)
        _RampPositions1 ("Positions p3–p5 (xyz)", Vector) = (0.8, 0.9, 1.0)

        // === NOISE SETTINGS ===
        [Header(Noise Settings)][Space(10)]
        [Enum(Normal, 0, Position, 1)] _SamplingSpace("Sampling Space", Float) = 1
        // _MixAmount ("Noise Mix Amount", Range(0,1)) = 0.5

        [Header(Voronoi Noise Settings)][Space(10)]
        [Enum(Euclidean, 1, Manhattan, 2, Chebyshev, 3, Minkowski, 4)]
        _DistanceMetric ("Distance Metric", Float) = 1
        _VoronoiScale ("Voronoi Scale", Float) = 2.2
        _VoronoiExponent ("Voronoi Exponent", Float) = 1.0
        _VoronoiSmoothness ("Voronoi Smoothness", Range(0,1)) = 0.5
        _VoronoiRandomness ("Voronoi Randomness", Range(0,1)) = 1.0

        //        [Header(Fractal Brownian Motion (FBM) Noise)][Space(10)]
        //        _FbmScale ("FBM Scale", Float) = 2.0
        //        _FbmRoughness ("FBM Roughness", Range(0,1)) = 0.5
        //        _FbmLacunarity ("FBM Lacunarity", Float) = 2.0
        //        _FbmAmplitude ("FBM Amplitude", Float) = 1.0
        //        _FbmFrequency ("FBM Frequency", Float) = 1.0
        //        _FbmShift ("FBM Shift", Vector) = (8,8,8,8)
        //        _FbmDetail ("FBM Detail", Range(1,15)) = 15

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
            #pragma shader_feature SPECULAR
            #define MAX_RAMP_STOPS 64

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
                float2 uv_Albedo : TEXCOORD5;
                float2 uv_Normal : TEXCOORD6;
                float2 uv_Emissive : TEXCOORD7;
                float3 generatedCoord : TEXCOORD8;
            };

            // ============================
            // BASE MATERIAL UNIFORMS
            // ============================
            float4 _DiffuseColour;
            sampler2D _AlbedoTex;
            half4 _AlbedoTex_ST;
            sampler2D _NormalTex;
            half4 _NormalTex_ST;
            half _NormalStrength;

            // ============================
            // BLINN-PHONG LIGHTING UNIFORMS
            // ============================
            float3 _k; // (Ambient, Diffuse, Specular)
            float _SpecularExponent;
            #ifdef SPECULAR
            float _Specular; // Only if [Toggle(SPECULAR)] is active
            #endif


            // ============================
            // COLOUR RAMP UNIFORMS
            // ============================
            half3 _DarkTone;
            half3 _MidDarkTone;
            half3 _MiddleTone;
            half3 _MidLightTone;
            half3 _LightTone;
            half3 _Highlight;

            half4 _RampPositions0; // (p0, p1, p2)
            half4 _RampPositions1; // (p3, p4, p5)

            // ============================
            // NOISE SETTINGS
            // ============================
            int _SamplingSpace;
            float _MixAmount;

            // ============================
            // NOISE: VORONOI
            // ============================
            float _VoronoiScale;
            float _VoronoiExponent;
            float _VoronoiSmoothness;
            float _VoronoiRandomness;
            int _DistanceMetric;

            // ============================
            // NOISE: FBM
            // ============================
            float _FbmScale;
            float _FbmRoughness;
            float _FbmLacunarity;
            float _FbmAmplitude;
            float _FbmFrequency;
            float3 _FbmShift;
            int _FbmDetail;

            // Function Prototypes
            half3 ProcessNormals(v2f input);


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Lighting/BlinnPhong.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/Voronoi.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/FbmNoise.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/LinearLight.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Colour/ColourRamp.hlsl"

            v2f vert(appdata input)
            {
                v2f output;
                output.fragLocalPos = input.vertex;
                output.fragWorldPos = TransformObjectToWorld(input.vertex.xyz);
                output.fragHCS = TransformWorldToHClip(output.fragWorldPos);

                float3 worldNormal = TransformObjectToWorldNormal(input.normal);
                float3 worldTangent = mul((float3x3)unity_ObjectToWorld, input.tangent);
                float3 bitangent = cross(worldNormal, worldTangent);
                float3 worldBitangent = mul((float3x3)unity_ObjectToWorld, bitangent);

                output.TBN = float3x3(worldTangent, worldBitangent, worldNormal);

                output.uv_Albedo = TRANSFORM_TEX(input.uv, _AlbedoTex);
                output.uv_Normal = TRANSFORM_TEX(input.uv, _NormalTex);

                output.generatedCoord = (input.vertex.xyz * 0.5) + 0.5;


                return output;
            }

            half4 frag(v2f input) : SV_Target
            {
                half3 albedoTexture = tex2D(_AlbedoTex, input.uv_Albedo);
                Light mainLight = GetMainLight();

                half3 n = ProcessNormals(input);
                half3 l = mainLight.direction;
                float3 v = normalize(_WorldSpaceCameraPos - input.fragWorldPos); // _WorldSpaceCameraPos and input.positionWS are usually large floats

                // BUG: Problematic noise
                // float3 noise = FbmNoise(n, _FbmScale, _FbmDetail, _FbmRoughness, _FbmLacunarity);
                // float3 blended = LinearLight(n, noise, _MixAmount);

                float dist;
                float3 col;
                float3 pos;

                float3 sampleCoord;
                if (_SamplingSpace == 0) sampleCoord = n; // Normal
                else if (_SamplingSpace == 1) sampleCoord = input.fragLocalPos; // Local Fragment Position

                VoronoiSmoothF1_3D(sampleCoord * _VoronoiScale, _VoronoiSmoothness, _VoronoiExponent, _VoronoiRandomness, _DistanceMetric, dist, col, pos);

                half3 lighting = BlinnPhong(pos, l, v, mainLight, albedoTexture);

                half3 mapped = ColourRamp(lighting);

                return half4(mapped, 1.0);
            }

            half3 ProcessNormals(v2f input)
            {
                half3 normalMap = UnpackNormal(tex2D(_NormalTex, input.uv_Normal));
                normalMap.xy *= _NormalStrength;
                return normalize(mul(transpose(input.TBN), normalMap));
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}