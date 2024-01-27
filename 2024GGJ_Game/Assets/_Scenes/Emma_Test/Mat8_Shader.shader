Shader "Mat8_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanLineTex ("ScanLine Texture", 2D) = "white" {}
        _ScanTime ("Scan Time", Range(0, 3)) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ScanLineTex;
            float _ScanTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float scanLine = tex2D(_ScanLineTex, float2(i.uv.x, _Time.y * _ScanTime)).r;
                return col * scanLine;
            }
            ENDCG
        }
    }
}