// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "beffio/Medieval_Kingdom/Mobile"
{
	Properties
	{
		_Base_color("Color", Color) = (0.427451,0.227451,0.1372549,0)
		_Cutoff( "Mask Clip Value", Float ) = 0
		_Detail_color("Detail_color", Color) = (0.2720588,0.2720588,0.2720588,0)
		_Detail_level("Detail_level", Range( 0 , 1)) = 1
		_Alpha_cutout_level("Alpha_cutout_level", Range( 0 , 1)) = 1
		_Smoothness_shift("Smoothness_shift", Range( 0 , 5)) = 0.1
		_Normal_intensity("Normal_intensity", Range( 0 , 2)) = 1
		_Albedo_smoothness_map_input("Albedo_smoothness_map_input", 2D) = "white" {}
		_Detail_mask_1_map_input("Detail_mask_1_map_input", 2D) = "white" {}
		_Alpha_cutout_mask_map_input("Alpha_cutout_mask_map_input", 2D) = "white" {}
		_Normal_map_input("Normal_map_input", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard noforward 
		struct Input
		{
			fixed2 uv_texcoord;
		};

		uniform fixed _Normal_intensity;
		uniform sampler2D _Normal_map_input;
		uniform fixed _Tilling;
		uniform sampler2D _Albedo_smoothness_map_input;
		uniform fixed4 _Base_color;
		uniform fixed4 _Detail_color;
		uniform sampler2D _Detail_mask_1_map_input;
		uniform fixed _Detail_level;
		uniform sampler2D _Emmisvie_map_input;
		uniform float4 _Emmisvie_map_input_ST;
		uniform fixed4 _Emmisive_color;
		uniform fixed _Emmisive_intensity;
		uniform fixed _Smoothness_shift;
		uniform sampler2D _Alpha_cutout_mask_map_input;
		uniform float4 _Alpha_cutout_mask_map_input_ST;
		uniform fixed _Alpha_cutout_level;
		uniform float _Cutoff = 0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = UnpackScaleNormal( tex2D( _Normal_map_input, i.uv_texcoord ) ,_Normal_intensity );
			fixed4 albedoMap = tex2D( _Albedo_smoothness_map_input, i.uv_texcoord );
			float4 lerpResult46 = lerp(
				( 
					saturate( 
						(
							( _Base_color > 0.5 ) ? 
							( 1.0 - ( 1.0 - 2.0 * ( _Base_color - 0.5 ) ) * ( 1.0 - albedoMap ) ) 
							: 
							( 2.0 * _Base_color * albedoMap ) 
							) 
						)
					) , _Detail_color , ( tex2D( _Detail_mask_1_map_input, i.uv_texcoord ) * _Detail_level ).r);

			o.Albedo = lerpResult46.rgb;
			o.Smoothness = ( albedoMap.a * _Smoothness_shift );
			o.Alpha = 1;
			float2 uv_Alpha_cutout_mask_map_input = i.uv_texcoord * _Alpha_cutout_mask_map_input_ST.xy + _Alpha_cutout_mask_map_input_ST.zw;
			float4 _alpha114 = ( tex2D( _Alpha_cutout_mask_map_input, uv_Alpha_cutout_mask_map_input ) * _Alpha_cutout_level );
			clip( _alpha114.r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}