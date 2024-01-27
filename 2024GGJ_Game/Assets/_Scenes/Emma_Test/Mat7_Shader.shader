Shader "Unlit/Mat7_Shader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanSpeed ("Scan Speed", Range(0.1,3)) = 1
        _BlackWidth ("Black Width", Range(0.1,3)) = 1
        _ScanAngle ("Scan Angle", Range(-45,45)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float4 _ScanlineColor;
            float _ScanSpeed;
            float _BlackWidth;
            float _ScanAngle;

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                half scanline = sin((_Time.y * _ScanSpeed + i.positionWS.y * 10.0 + _ScanAngle) * _BlackWidth);
                col += half4(_ScanlineColor) * scanline;
                return col;
            }
            ENDHLSL
        }
    }
}