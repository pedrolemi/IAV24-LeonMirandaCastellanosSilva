Shader "Unlit/outline shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0,0,0,0)
        _Width("Width", Range(0.0,5.0)) = 1.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // Outline
        Pass
        {
            Tags
            {
                "Queue" = "Transparent"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Zwrite Off

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
            float4 _MainTex_ST;
            float4 _Color;
            float _Width;

            float4 outline(float4 vertexPos, float outValue)
            {
                float4x4 scale = float4x4
                (
                    1 + outValue, 0, 0, 0,
                    0, 1 + outValue, 0, 0,
                    0, 0, 1 + outValue, 0,
                    0, 0, 0, 1 + outValue
                );
                return mul(scale, vertexPos);
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertexPos = outline(v.vertex, _Width);
                o.vertex = UnityObjectToClipPos(vertexPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return float4(_Color.r, _Color.g, _Color.b, col.a);
            }
            ENDCG
        }

        // Texture
        Pass
        {
            Tags
            {
                "Queue" = "Transparent+1"
            }
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
