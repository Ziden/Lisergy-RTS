Shader "Revision3/FogOfWar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_PointCount("Point count", Range(0,512)) = 0
		_Scale("Scale", Float) = 1.0
		_RadarRange("Range", Float) = .5
		_MaxAlpha("Maximum Alpha", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _RadarRange;
			uint _PointCount;
			float _Scale;
			float _MaxAlpha;

			float2 _PointArray[512];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float getDistance(float2 pa[512], float2 uv) {

				float cdist = 99999.0;


				for (uint i = 0; i < _PointCount; i++) {
					cdist = min(cdist, distance(pa[i]*_Scale, uv));
				}


				return cdist;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				i.uv *= _Scale;

				if (_PointCount > 0)
					col.w = min(_MaxAlpha, max(0.0f, getDistance(_PointArray, i.uv) - _RadarRange));
				else
					col.w = _MaxAlpha;

				return col;
            }
            ENDCG
        }
    }
}
