Shader "URP/UI/Default_OverlayNoZTest_URP"

{

    Properties

    {

        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)



        _StencilComp ("Stencil Comparison", Int) = 8

        _Stencil ("Stencil ID", Int) = 0

        _StencilOp ("Stencil Operation", Int) = 0

        _StencilWriteMask ("Stencil Write Mask", Int) = 255

        _StencilReadMask ("Stencil Read Mask", Int) = 255



        _ColorMask ("Color Mask", Int) = 15

    }



    SubShader

    {

        Tags

        {

            "Queue"="Overlay"

            "IgnoreProjector"="True"

            "RenderType"="Transparent"

            "PreviewType"="Plane"

            "CanUseSpriteAtlas"="True"

        }



        Stencil

        {

            Ref [_Stencil]

            Comp [_StencilComp]

            Pass [_StencilOp]

            ReadMask [_StencilReadMask]

            WriteMask [_StencilWriteMask]

        }



        Cull Off

        Lighting Off

        ZWrite Off

        ZTest Off

        Blend SrcAlpha OneMinusSrcAlpha

        ColorMask [_ColorMask]



        Pass

        {

            Name "UIOverlayNoZTest"

            Tags
            {
                "LightMode" = "UniversalForward"
            } // Important for URP rendering



            HLSLPROGRAM
            #pragma vertex vert

            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" // Essential URP core functions



            struct appdata_t

            {
                float4 vertex : POSITION;

                float4 color : COLOR;

                float2 texcoord : TEXCOORD0;
            };


            struct v2f

            {
                float4 vertex : SV_POSITION;

                half4 color : COLOR;

                half2 texcoord : TEXCOORD0;
            };


            half4 _Color;

            sampler2D _MainTex;


            v2f vert(appdata_t IN)

            {
                v2f OUT;

                OUT.vertex = TransformObjectToHClip(IN.vertex.xyz); // Recommended URP matrix function

                OUT.texcoord = IN.texcoord;

                OUT.color = IN.color * _Color;

                return OUT;
            }


            half4 frag(v2f IN) : SV_Target

            {
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

                clip(color.a - 0.01);

                return color;
            }
            ENDHLSL

        }

    }

}