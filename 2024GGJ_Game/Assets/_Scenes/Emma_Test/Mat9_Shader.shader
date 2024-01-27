Shader "Custom/Mat9_Shader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (1,1,0,1) // Changed to yellow
        _ScanSpeed ("Scan Speed", Range(0.1,30)) = 0.2 // Adjusted for 5 seconds
        _BlackWidth ("Black Width", Range(0.1,3)) = 0.2 // Adjusted for 1/5 of the object
        _WhiteWidth ("White Width", Range(0.1,3)) = 0.04 // Adjusted for 1/5 of the object
        _ScanAngle ("Scan Angle", Range(-45,45)) = 45
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
            float _WhiteWidth;
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
                half scanline = sin((_Time.y * _ScanSpeed + dot(i.positionWS.xy, float2(1, 1)) * 10.0 + _ScanAngle) * _BlackWidth);
                half whiteline = sin((_Time.y * _ScanSpeed + dot(i.positionWS.xy, float2(1, 1)) * 10.0 + _ScanAngle) * _WhiteWidth);
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                col += half4(_ScanlineColor) * scanline;
                col += half4(1,1,1,1) * whiteline;
                return col;
            }
            ENDHLSL
        }
    }
}