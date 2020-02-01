Shader "Unlit/Two Sided" {
    Properties {
        _FrontTex ("Front", 2D) = "white" {}
        _BackTex ("Back", 2D) = "white" {}
    }
    
    SubShader {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _FrontTex;
            sampler2D _BackTex;

            struct appdata {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
        
            fixed4 frag (v2f i, fixed isFront : VFACE) : SV_TARGET {
                if (isFront > 0.0f) {
                    return tex2D(_FrontTex, i.uv);
                }
                else {
                    return tex2D(_BackTex, i.uv);
                }
            }

            ENDCG
        }
    }
}