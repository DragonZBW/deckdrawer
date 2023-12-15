Shader "Unlit/CircleShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 0.5
        _Thickness("Thickness", Float) = 0.1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _Color;
            float _Radius;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, float2(0.5, 0.5));
                fixed4 col = fixed4(_Color.x, _Color.y, _Color.z, step(dist, _Radius + _Thickness) - step(dist, _Radius));
                return col;
            }
            ENDCG
        }
    }
}
