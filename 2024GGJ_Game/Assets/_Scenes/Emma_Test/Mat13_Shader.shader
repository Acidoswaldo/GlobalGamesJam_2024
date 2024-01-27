Shader "Unlit/Mat13_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanColor ("Scan Color", Color) = (1,1,0,1)
        _ScanTime ("Scan Time", Range(0,10)) = 5
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
                float4 screenPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ScanColor;
            float _ScanTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.screenPos.xy / i.screenPos.w);
                float scanLine = fmod(_Time.y, _ScanTime);
                if (i.screenPos.x / i.screenPos.w + i.screenPos.y / i.screenPos.w - scanLine < 0.1 && i.screenPos.x / i.screenPos.w + i.screenPos.y / i.screenPos.w - scanLine > -0.1)
                {
                    col = _ScanColor;
                }
                return col;
            }
            ENDCG
        }
    }
}