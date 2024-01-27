Shader "Custom/Mat10_Shader1"
{
    Properties
    {
        [HDR]_Color("Albedo", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BaseMap ("Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanSpeed ("Scan Speed", Range(0.1,30)) = 1
        _BlackWidth ("Black Width", Range(0.001,3)) = 2.9
        _WhiteWidth ("White Width", Range(0.01,3)) = 0.1
        _ScanAngle ("Scan Angle", Range(-45,45)) = 45

        [Header(Stencil)]
        _Stencil ("Stencil ID [0;255]", Float) = 0
        _ReadMask ("ReadMask [0;255]", Int) = 255
        _WriteMask ("WriteMask [0;255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Int) = 0
       
        [Header(Rendering)]
        _Offset("Offset", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull Mode", Int) = 2
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4
        [Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask("Color Mask", Int) = 15
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Stencil
        {
            Ref [_Stencil]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        Pass
        {
            Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
            LOD 100
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]

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
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _MainTex_ST;
            half4 _Color;
            float4 _BaseMap_ST;
            float4 _ScanlineColor;
            float _ScanSpeed;
            float _BlackWidth;
            float _WhiteWidth;
            float _ScanAngle;

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half scanline = sin((_Time.y * _ScanSpeed + dot(i.positionWS.xy, float2(1, 1)) * 10.0 + _ScanAngle) * _BlackWidth);
                half whiteline = sin((_Time.y * _ScanSpeed + dot(i.positionWS.xy, float2(1, 1)) * 10.0 + _ScanAngle) * _WhiteWidth);
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Color;
                half4 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                if (baseCol.r > 0.5 && baseCol.g > 0.5 && baseCol.b > 0.5) // white part
                {
                    col += half4(1,1,1,1) * whiteline * 0.5; // 
                    return col;
                }
                else // black part
                {
                    return col; // 
                }
            }
            ENDHLSL
        }
    }
}