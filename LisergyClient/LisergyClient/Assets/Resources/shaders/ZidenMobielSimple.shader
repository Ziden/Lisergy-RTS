Shader "Ziden/MobileSimple"
{
    Properties
    {
        _Base_color("Base_Color", Color) = (0.2720588,0.2720588,0.2720588,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Detail_color("Detail_color", Color) = (0.2720588,0.2720588,0.2720588,0)
        _DetailTex ("Detail Texture", 2D) = "white" {}
        _DetailStrength ("Detail Strength", Range(0,1)) = 1
        _Metallic ("Metallic", Range(0,1)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Standard noforwardadd 

        struct Input {
            float2 uv_MainTex;
            float2 uv_DetailTex;
            float3 worldPos;
            float3 worldNormal;
        };

        uniform sampler2D _MainTex;
        uniform sampler2D _DetailTex;
        uniform sampler2D _BumpMap;
        uniform sampler2D _Alpha_cutout_mask_map_input;
		uniform float4 _Alpha_cutout_mask_map_input_ST;

        uniform float4 _Base_color;
        uniform float _Cutoff;
        uniform float _DetailStrength;
        uniform fixed4 _Detail_color;
        uniform float _Metallic;
        uniform float _Glossiness;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Alpha = 1;
            
            // Detail
            float4 detail = tex2D(_DetailTex, IN.uv_DetailTex);
            float detailStrength = _DetailStrength * detail.r;
            o.Albedo += detail.rgb * detailStrength;
            o.Albedo *= _Base_color;
            // Metallic and smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // Normal mapping
            //o.Normal = UnpackScaleNormal( tex2D( _BumpMap, IN.uv_MainTex ) , _BumpStrength );
            //o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            //o.Normal = (o.Normal * 2.0 - 1.0) * _BumpStrength;
            //o.Normal = normalize(UnityObjectToWorldNormal(o.Normal));

            // Alpha cutoff
            // fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            //clip(o.Alpha - _Cutoff);
           
            //float2 uv_Alpha_cutout_mask_map_input = IN.uv_texcoord * _Alpha_cutout_mask_map_input_ST.xy + _Alpha_cutout_mask_map_input_ST.zw;
			//float4 _alpha114 = ( tex2D( _Alpha_cutout_mask_map_input, uv_Alpha_cutout_mask_map_input ) * _Alpha_cutout_level );
			//clip( _alpha114.a - _Cutoff );
           
        }
        ENDCG
    }
    FallBack "Diffuse"
}