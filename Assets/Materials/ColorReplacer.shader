Shader "Unlit/Color Replacer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SrcColor ("Source Color", Color) = (1.0, 0.0, 1.0)
        _DestColor ("Destination Color", Color) = (0.0, 1.0, 0.0)
        _HueThreshold ("Hue Threshold", Float) = 0.1
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the __pixel__ (fragment) shader
            #pragma fragment frag

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 vertex : SV_POSITION; // clip space position
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // transform position to clip space
                // (multiply with model*view*projection matrix)
                o.vertex = UnityObjectToClipPos(v.vertex);
                // just pass the texture coordinate
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

            // texture we will sample
            sampler2D _MainTex;
            fixed3 _SrcColor;
            fixed3 _DestColor;
            fixed _HueThreshold;

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 thisRGBA = (tex2D(_MainTex, i.uv));
                fixed3 thisHSV = rgb2hsv((fixed3)thisRGBA);

                fixed3 srcHSV = rgb2hsv(_SrcColor);
                fixed hueDiff = abs(thisHSV.x - srcHSV.x);

                if (hueDiff < _HueThreshold) {
                    fixed3 destHSV = rgb2hsv(_DestColor);
                    fixed4 destRGBA = fixed4(hsv2rgb(fixed3(destHSV.x, destHSV.y, thisHSV.z)), thisRGBA.a);
                    thisRGBA = lerp(destRGBA, thisRGBA, hueDiff / _HueThreshold);
                }

                return thisRGBA;
            }
            ENDCG
        }
    }
}
