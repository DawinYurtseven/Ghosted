Shader "ForgottenColours/Unlit/Sumi-E"
{
    Properties
    {
        // === BASE MATERIAL SETTINGS ===
        [Header(Base Colours)][Space(10)]
        _LightTint("Light Tint", Color) = (1,1,1,1)
        _DarkTint("Dark Tint", Color) = (1,1,1,1)

        [Header(Textures)][Space(10)]
        _AlbedoTex ("Albedo Texture", 2D) = "white"{}

        [Space(10)]
        [Toggle(USETRANSPARENT)] _UseTransparent("Use Transparent", float) = 0
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5

        [Space(10)]
        _NormalTex ("Normal Map", 2D) = "bump"{}
        _NormalStrength ("Normal Strength", Float) = 1

        [Space(10)]
        [Toggle(USEEMISSIVE)] _Emissive("Emissive", float) = 0
        _EmissiveTex ("Emissive Map", 2D) = "emissive" {}
        [HDR] _EmissiveColour("Emissive Colour", Color) = (1,1,1,1)

        [Space(10)]
        [Toggle(BACKFACE_LIGHTING)] _BackfaceLighting("Backface Lighting", Float) = 1

        // === BLINN-PHONG LIGHTING ===
        [Header(Blinn Phong Lighting)][Space(10)]
        [Toggle(SPECULAR)] _Specular("Enable Specular Highlight", Float) = 0
        _k ("k Coefficients (Ambient, Diffuse, Specular)", Vector) = (0.5, 0.5, 0.8)
        _SpecularExponent("Specular Exponent", Float) = 80

        // === NOISE SETTINGS ===
        [Header(Texture Coordinates)][Space(10)]
        [Enum(Generated, 0, Normal, 1, UV, 2, Object, 3)] _TextureSpace("Texture Space", Float) = 1

        [Header(FBM Noise Settings)][Space(10)]
        _FbmScale("FBM Noise Scale", Float) = 10
        _FbmDetail("FBM Noise Detail", Range(1,15)) = 15
        _FbmSmoothness("FBM Roughness", Range(0,1))= 0.5
        _FbmLacunarity("FBM Lacunarity", Float) = 2

        [Header(Voronoi Noise Settings)][Space(10)]
        [Enum(Euclidean, 1, Manhattan, 2, Chebyshev, 3, Minkowski, 4)]
        _DistanceMetric ("Distance Metric", Float) = 1
        _VoronoiScale ("Voronoi Scale", Float) = 2.2
        _VoronoiExponent ("Voronoi Exponent", Float) = 1.0
        _VoronoiSmoothness ("Voronoi Smoothness", Range(0,1)) = 0.5
        _VoronoiRandomness ("Voronoi Randomness", Range(0,1)) = 1.0


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
            Cull Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma shader_feature SPECULAR
            #pragma shader_feature USECOLOURRAMP
            #pragma shader_feature USEEMISSIVE
            #pragma shader_feature USETRANSPARENT
            #pragma shader_feature BACKFACE_LIGHTING
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
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
                float3 generatedCoord : TEXCOORD5;
                float2 uv_Albedo : TEXCOORD6;
                float2 uv_Emissive : TEXCOORD7;
            };

            // ============================
            // BASE MATERIAL UNIFORMS
            // ============================
            float4 _LightTint;
            float4 _DarkTint;

            sampler2D _AlbedoTex;
            half4 _AlbedoTex_ST;

            half _AlphaCutoff;

            sampler2D _NormalTex;
            half4 _NormalTex_ST;
            half _NormalStrength;

            sampler2D _EmissiveTex;
            half4 _EmissiveTex_ST;
            half4 _EmissiveColour;

            // ============================
            // BLINN-PHONG LIGHTING UNIFORMS
            // ============================
            float3 _k; // (Ambient, Diffuse, Specular)
            float _SpecularExponent;
            #ifdef SPECULAR
            float _Specular; // Only if [Toggle(SPECULAR)] is active
            #endif

            // ============================
            // TEXTURE COORDINATES SETTINGS
            // ============================
            int _TextureSpace;
            float _MixAmount;

            // ============================
            // NOISE: FBM
            // ============================
            float _FbmScale;
            float _FbmDetail;
            float _FbmSmoothness;
            float _FbmRandomness;
            float _FbmLacunarity;
            // ============================
            // NOISE: VORONOI
            // ============================
            float _VoronoiScale;
            float _VoronoiExponent;
            float _VoronoiSmoothness;
            float _VoronoiRandomness;
            int _DistanceMetric;


            // Function Prototypes
            half3 ProcessNormals(v2f input);

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Lighting/BlinnPhong.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/Voronoi.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/TextureCoordinate.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/FbmNoise.hlsl"
            #include "Assets/Code/Ben/Shaders/ShaderLibrary/Maths/LinearLight.hlsl"

            // Vertex shader: transforms vertex data and prepares inputs for the fragment shader
            v2f vert(appdata input)
            {
                v2f output;

                // Transform object-space position into world space and clip space
                VertexPositionInputs positions = GetVertexPositionInputs(input.vertex);
                output.fragLocalPos = input.vertex;              // Store local-space position (useful for procedural effects)
                output.fragWorldPos = positions.positionWS;      // Store world-space position (used for lighting, shadows, etc.)
                output.fragHCS = positions.positionCS;           // Store homogeneous clip space position (used for screen-space effects)

                // Convert normal, tangent, and bitangent from object space to world space
                float3 worldNormal = TransformObjectToWorldNormal(input.normal);                   // Object-space normal → world-space
                float3 worldTangent = mul((float3x3)unity_ObjectToWorld, input.tangent);           // Transform tangent to world-space
                float3 bitangent = cross(worldNormal, worldTangent);                               // Reconstruct bitangent
                float3 worldBitangent = mul((float3x3)unity_ObjectToWorld, bitangent);             // Transform bitangent to world-space

                // Construct TBN matrix for transforming tangent-space normals to world space
                output.TBN = float3x3(worldTangent, worldBitangent, worldNormal);

                // Pass transformed texture coordinates to the fragment shader
                output.uv_Albedo = TRANSFORM_TEX(input.uv, _AlbedoTex);     // Albedo texture UVs
                output.uv_Emissive = TRANSFORM_TEX(input.uv, _AlbedoTex);   // Emissive texture UVs (currently using same set)

                return output;
            }

            half4 frag(v2f input, bool frontFace : SV_IsFrontFace) : SV_Target
            {
                // Sampling shadow coords in fragment shader to avoid cascading seams.
                float4 shadowCoords = TransformWorldToShadowCoord(input.fragWorldPos);

                // Sample albedo texture
                half4 c = tex2D(_AlbedoTex, input.uv_Albedo);

                // Enable alpha clipping
                #ifdef USETRANSPARENT
                clip(c.a - _AlphaCutoff);
                #endif

                half3 emissive;

                // Sample emissive map (if enabled) and apply emissive colour tint
                #ifdef USEEMISSIVE
                emissive = tex2D(_EmissiveTex, input.uv_Emissive) * _EmissiveColour;
                #else
                emissive = 0;
                #endif

                // Retrieve URP's main lighting data for pixel using shadow coordinates
                Light mainLight = GetMainLight(shadowCoords);

                // Sample and transform normal from normal map into world space
                half3 n = ProcessNormals(input);

                // Flip normal for backfaces if lighting is enabled on both sides
                #ifdef BACKFACE_LIGHTING
                if (!frontFace)
                {
                    n *= -1;
                }
                #endif

                // FBM Noise Section
                half3 noiseCoord = GetTextureSpace(_TextureSpace, n, input.fragLocalPos, input.uv_Albedo);
                float fbmNoise = 1-FbmNoise(noiseCoord, _FbmScale, _FbmDetail, 1-_FbmSmoothness,_FbmSmoothness);
                float3 fbmNoise3D = float3(fbmNoise,fbmNoise,fbmNoise);

                // Mix fbm noise and mesh normals
                float3 mixed = LinearLight(n, fbmNoise3D,0.5);

                // Calculate view direction (camera to fragment)
                float3 v = normalize(_WorldSpaceCameraPos - input.fragWorldPos);

                // Select coordinate system for Voronoi noise based on material settings

                // Generate 3D Voronoi noise with smooth blending and distortion options
                float dist;
                float3 col, pos;
                VoronoiSmoothF1_3D(noiseCoord * _VoronoiScale, _VoronoiSmoothness, _VoronoiExponent, _VoronoiRandomness, _DistanceMetric, dist, col, pos);

                // Compute Blinn-Phong lighting with shadows and attenuation from main light
                half3 lighting = BlinnPhong(pos, v, mainLight, c) * mainLight.shadowAttenuation * mainLight.distanceAttenuation;

                // Accumulate lighting from additional point/spot lights if enabled
                #if defined(_ADDITIONAL_LIGHTS_VERTEX) || defined(_ADDITIONAL_LIGHTS)
                int addCount = GetAdditionalLightsCount();
                for (int i = 0; i < addCount; i++)
                {
                    Light additionalLight = GetAdditionalLight(i, input.fragWorldPos);
                    half att = additionalLight.distanceAttenuation * additionalLight.shadowAttenuation;
                    lighting += BlinnPhong(pos, v, additionalLight, c) * att;
                }
                #endif

                return half4(lighting+ emissive, 1.0);
                return half4(mixed,1.0);
            }

            // Samples and transforms the normal map into world space
            half3 ProcessNormals(v2f input)
            {
                // Sample the normal from the normal map texture (in tangent space)
                half3 normalMap = UnpackNormal(tex2D(_NormalTex, input.uv_Albedo));
                // Apply user-defined strength to exaggerate or soften bump detail
                normalMap.xy *= _NormalStrength;
                // Transform from tangent space to world space using the TBN matrix
                return normalize(mul(transpose(input.TBN), normalMap));
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}