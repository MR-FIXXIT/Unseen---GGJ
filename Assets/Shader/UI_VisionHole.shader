Shader "UI/VisionHole"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,0.85)
        _Center ("Center (Screen Pixels)", Vector) = (0,0,0,0)
        _Radius ("Radius (Pixels)", Float) = 200
        _Softness ("Edge Softness (Pixels)", Float) = 50
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _Center;
            float _Radius;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // screenPos is in clip space -> convert to pixel coordinates
                float2 uv = i.screenPos.xy / i.screenPos.w;  // 0..1
                float2 pixelPos = float2(uv.x * _ScreenParams.x, uv.y * _ScreenParams.y);

                float dist = distance(pixelPos, _Center.xy);

                // Alpha = 0 inside radius, Alpha = _Color.a outside, with soft edge
                float edge0 = _Radius;
                float edge1 = _Radius + max(_Softness, 0.0001);

                float t = saturate((dist - edge0) / (edge1 - edge0)); // 0 inside, 1 outside after softness

                fixed4 col = _Color;
                col.a *= t; // fade in darkness outside the circle

                return col;
            }
            ENDCG
        }
    }
}
