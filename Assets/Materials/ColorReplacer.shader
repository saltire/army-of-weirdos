Shader "Unlit/Color Replacer" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SrcColor ("Source Color", Color) = (1.0, 0.0, 1.0)
        _DestColor ("Destination Color", Color) = (0.0, 1.0, 0.0)
        _HueThreshold ("Hue Threshold", Float) = 0.1
    }

    SubShader {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Tags { "Queue" = "Transparent" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            fixed3 _SrcColor;
            fixed3 _DestColor;
            fixed _HueThreshold;

            struct appdata {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // vertex shader
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed3 rgb2hsv(fixed3 c) {
                fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
                fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed3 hsv2rgb(fixed3 c) {
                fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed4 frag (v2f i) : SV_TARGET {
                fixed4 thisRGBA = tex2D(_MainTex, i.uv);
                fixed3 thisHSV = rgb2hsv((fixed3)thisRGBA);

                fixed3 srcHSV = rgb2hsv(_SrcColor);
                fixed hueDiff = abs(thisHSV.x - srcHSV.x);

                if (hueDiff < _HueThreshold) {
                    fixed3 destHSV = rgb2hsv(_DestColor);
                    fixed4 destRGBA = fixed4(hsv2rgb(fixed3(destHSV.x, thisHSV.y, thisHSV.z)), thisRGBA.a);
                    thisRGBA = lerp(destRGBA, thisRGBA, hueDiff / _HueThreshold);
                }

                return thisRGBA;
            }

            ENDCG
        }
    }
}
