Shader "Unlit/Mat6_Shader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)
        _ScanSpeed ("Scan Speed", Range(0.1,3)) = 1
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
            };

            TEXTURE2D(_BaseMap);
            float4 _BaseMap_ST;
            float4 _ScanlineColor;
            float _ScanSpeed;

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
                return o;
            }
            
            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.positionWS.xz);
                half scanline = sin(_Time.y * _ScanSpeed - i.positionWS.y * 10.0);
                col += half4(_ScanlineColor) * scanline;
                return col;
            }
            ENDHLSL
        }
    }
}