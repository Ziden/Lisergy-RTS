// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/SimpleWater"
{
	Properties
	{
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapScale("Normal Map Scale", Range( 0 , 10)) = 1
		_NormalIntensity("Normal Intensity", Range( 0 , 5)) = 0
		_WaterColorBlend("Water Color Blend", Float) = 0
		_WaterDeepColor("Water Deep Color", Color) = (0.1320755,0.1320755,0.1320755,0)
		_WaterShallowColor("Water Shallow Color", Color) = (0.1320755,0.1320755,0.1320755,0)
		_MainOpacity("Main Opacity", Range( 0 , 1)) = 0.2
		_WaterOpacityBlend("Water Opacity Blend", Float) = 0
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0.95
		_WaterSpecularity("Water Specularity", Range( 0 , 1)) = 1
		_FoamColor("Foam Color", Color) = (0.9150943,0.9150943,0.9150943,0)
		_FoamAmount("Foam Amount", Range( 0 , 2)) = 0
		_FoamContrast("Foam Contrast", Range( 0 , 5)) = 1.5
		_FoamTexture("Foam Texture", 2D) = "white" {}
		_FoamScale("Foam Scale", Float) = 3
		_RefractionIntensity("Refraction Intensity", Range( 0 , 5)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf StandardSpecular alpha:fade keepalpha exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform sampler2D _NormalMap;
		uniform float _NormalMapScale;
		uniform float _NormalIntensity;
		uniform float4 _WaterShallowColor;
		uniform float4 _WaterDeepColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _WaterColorBlend;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionIntensity;
		uniform float _MainOpacity;
		uniform float _WaterOpacityBlend;
		uniform float _FoamAmount;
		uniform float _FoamContrast;
		uniform sampler2D _FoamTexture;
		uniform float _FoamScale;
		uniform float4 _FoamColor;
		uniform float _WaterSpecularity;
		uniform float _WaterSmoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float temp_output_11_0_g2 = ( 1.0 * _Time.y );
			float3 ase_worldPos = i.worldPos;
			float2 panner44 = ( 1.0 * _Time.y * float2( 0.02,0.1 ) + (( ase_worldPos / ( 3.0 * _NormalMapScale ) )).xz);
			float2 temp_output_32_0_g2 = panner44;
			float2 panner5_g2 = ( temp_output_11_0_g2 * float2( 0.1,0.1 ) + temp_output_32_0_g2);
			float temp_output_33_0_g2 = _NormalIntensity;
			float2 panner16_g2 = ( temp_output_11_0_g2 * float2( -0.1,-0.1 ) + ( temp_output_32_0_g2 + float2( 0.418,0.355 ) ));
			float2 panner19_g2 = ( temp_output_11_0_g2 * float2( -0.1,0.1 ) + ( temp_output_32_0_g2 + float2( 0.865,0.148 ) ));
			float2 panner23_g2 = ( temp_output_11_0_g2 * float2( 0.1,-0.1 ) + ( temp_output_32_0_g2 + float2( 0.651,0.752 ) ));
			float temp_output_11_0_g3 = ( 0.6 * _Time.y );
			float2 panner50 = ( 1.0 * _Time.y * float2( 0.1,0.06 ) + (( ase_worldPos / ( 8.0 * _NormalMapScale ) )).xz);
			float2 temp_output_32_0_g3 = panner50;
			float2 panner5_g3 = ( temp_output_11_0_g3 * float2( 0.1,0.1 ) + temp_output_32_0_g3);
			float temp_output_33_0_g3 = _NormalIntensity;
			float2 panner16_g3 = ( temp_output_11_0_g3 * float2( -0.1,-0.1 ) + ( temp_output_32_0_g3 + float2( 0.418,0.355 ) ));
			float2 panner19_g3 = ( temp_output_11_0_g3 * float2( -0.1,0.1 ) + ( temp_output_32_0_g3 + float2( 0.865,0.148 ) ));
			float2 panner23_g3 = ( temp_output_11_0_g3 * float2( 0.1,-0.1 ) + ( temp_output_32_0_g3 + float2( 0.651,0.752 ) ));
			float3 WaterNormal274 = BlendNormals( ( ( ( UnpackScaleNormal( tex2D( _NormalMap, panner5_g2 ), temp_output_33_0_g2 ) + UnpackScaleNormal( tex2D( _NormalMap, panner16_g2 ), temp_output_33_0_g2 ) ) + ( UnpackScaleNormal( tex2D( _NormalMap, panner19_g2 ), temp_output_33_0_g2 ) + UnpackScaleNormal( tex2D( _NormalMap, panner23_g2 ), temp_output_33_0_g2 ) ) ) * 1.0 ) , ( ( ( UnpackScaleNormal( tex2D( _NormalMap, panner5_g3 ), temp_output_33_0_g3 ) + UnpackScaleNormal( tex2D( _NormalMap, panner16_g3 ), temp_output_33_0_g3 ) ) + ( UnpackScaleNormal( tex2D( _NormalMap, panner19_g3 ), temp_output_33_0_g3 ) + UnpackScaleNormal( tex2D( _NormalMap, panner23_g3 ), temp_output_33_0_g3 ) ) ) * 1.0 ) );
			o.Normal = WaterNormal274;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth154 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth154 = saturate( abs( ( screenDepth154 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterColorBlend ) ) );
			float4 lerpResult159 = lerp( _WaterShallowColor , _WaterDeepColor , distanceDepth154);
			float4 screenColor271 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_screenPos + float4( ( WaterNormal274 * _RefractionIntensity ) , 0.0 ) ).xy/( ase_screenPos + float4( ( WaterNormal274 * _RefractionIntensity ) , 0.0 ) ).w);
			float screenDepth161 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth161 = saturate( abs( ( screenDepth161 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterOpacityBlend ) ) );
			float screenDepth182 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth182 = saturate( abs( ( screenDepth182 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _FoamAmount ) ) );
			float saferPower188 = abs( ( 1.0 - distanceDepth182 ) );
			float2 panner197 = ( 1.0 * _Time.y * float2( 0.2,0 ) + (( ase_worldPos / _FoamScale )).xz);
			float2 panner194 = ( 1.0 * _Time.y * float2( 0.3,0 ) + (( ase_worldPos / _FoamScale )).xz);
			float temp_output_209_0 = saturate( ( ( saturate( pow( saferPower188 , _FoamContrast ) ) - tex2D( _FoamTexture, panner197 ).r ) - saturate( pow( ( tex2D( _FoamTexture, panner194 ).g * 0.88 ) , 0.65 ) ) ) );
			float lerpResult191 = lerp( ( _MainOpacity * distanceDepth161 ) , 1.0 , temp_output_209_0);
			float Opacity298 = lerpResult191;
			float4 Refraction281 = ( ( screenColor271 * 0.5 ) * ( 1.0 - Opacity298 ) );
			float4 lerpResult184 = lerp( ( lerpResult159 + Refraction281 ) , _FoamColor , temp_output_209_0);
			o.Albedo = lerpResult184.rgb;
			float lerpResult246 = lerp( 1.0 , 0.45 , temp_output_209_0);
			float3 temp_cast_2 = (( _WaterSpecularity * lerpResult246 )).xxx;
			o.Specular = temp_cast_2;
			o.Smoothness = _WaterSmoothness;
			float screenDepth256 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth256 = saturate( abs( ( screenDepth256 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 0.11 ) ) );
			float saferPower258 = abs( distanceDepth256 );
			o.Alpha = saturate( pow( saferPower258 , 1.5 ) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-36;97;1920;1019;357.9913;1443.263;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;263;-2471.11,-1696.94;Inherit;False;2396.528;848.5557;Comment;15;213;214;198;200;215;195;194;199;202;196;197;193;253;252;250;Foam Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-2383.078,-1273.643;Inherit;False;Property;_FoamScale;Foam Scale;14;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;213;-2418.824,-1141.513;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;168;-61.4026,1290.114;Inherit;False;2801.553;1359.117;Comment;23;175;266;267;100;50;103;44;12;11;53;6;49;43;41;48;71;40;47;73;46;72;42;274;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;262;-1263.695,-751.9589;Inherit;False;1190.497;301.6274;Comment;6;182;210;188;190;187;189;Foam Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;214;-2141.793,-1091.385;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;42;694.9869,1621.842;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;448.704,2217.752;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;195.5693,1800.886;Inherit;False;Property;_NormalMapScale;Normal Map Scale;1;0;Create;True;0;0;0;False;0;False;1;2.6;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-1215.695,-660.8573;Inherit;False;Property;_FoamAmount;Foam Amount;11;0;Create;True;0;0;0;False;0;False;0;1.15;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;198;-2421.11,-1450.773;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;215;-1980.652,-1096.095;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;47;665.0825,2011.682;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;701.113,2191.43;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;861.8522,1691.362;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;40;697.9939,1340.114;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DepthFade;182;-929.8906,-701.9066;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;194;-1585.807,-1236.586;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.3,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;195;-1626.807,-1645.586;Inherit;True;Property;_FoamTexture;Foam Texture;13;0;Create;True;0;0;0;False;0;False;None;ac84d875cec63494b9e75d69325908c3;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleDivideOpNode;41;970.0322,1482.429;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;199;-2144.079,-1400.644;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;948.8112,2141.007;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;189;-1214.858,-544.3316;Inherit;False;Property;_FoamContrast;Foam Contrast;12;0;Create;True;0;0;0;False;0;False;1.5;3.15;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;196;-1221.808,-1419.586;Inherit;True;Property;_TextureSample1;Texture Sample 1;24;0;Create;True;0;0;0;False;0;False;-1;None;323f9644f30289e48b27a7e3fd40be60;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;202;-1982.937,-1405.353;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;49;1142.763,2163.677;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;1163.985,1505.099;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;210;-649.1445,-701.9589;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;44;1426.707,1448.529;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.02,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;50;1405.485,2107.106;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.06;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;1390.822,1946.592;Inherit;False;Constant;_WaterSpeed;Water Speed;4;0;Create;True;0;0;0;False;0;False;1;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;1439.625,2314.317;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;1383.828,1843.591;Inherit;False;Constant;_Float;Float;6;0;Create;True;0;0;0;False;0;False;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;1297.826,2021.782;Inherit;False;Property;_NormalIntensity;Normal Intensity;2;0;Create;True;0;0;0;False;0;False;0;0.9;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1431.343,2422.831;Inherit;False;Constant;_Float4;Float 4;4;0;Create;True;0;0;0;False;0;False;0.6;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;188;-430.3454,-645.6374;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;197;-1586.708,-1397.685;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;6;154.9976,1951.057;Inherit;True;Property;_NormalMap;Normal Map;0;1;[Normal];Create;True;0;0;0;False;0;False;104c20515b2912c40b3086db07672866;104c20515b2912c40b3086db07672866;True;bump;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;-799.5098,-1364.25;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0.88;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;252;-516.6389,-1361.63;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;190;-238.1969,-670.1245;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;266;1706.942,1896.367;Inherit;True;4WayChaos;-1;;3;39eb4b53527d6704289fdaf682a3d49e;0;5;32;FLOAT2;0,0;False;31;SAMPLER2D;0,0,0,0;False;29;FLOAT;0.2;False;33;FLOAT;1;False;10;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;267;1703.164,1650.791;Inherit;True;4WayChaos;-1;;2;39eb4b53527d6704289fdaf682a3d49e;0;5;32;FLOAT2;0,0;False;31;SAMPLER2D;0,0,0,0;False;29;FLOAT;0.2;False;33;FLOAT;1;False;10;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;193;-1221.321,-1646.94;Inherit;True;Property;_texture1;texture 1;24;0;Create;True;0;0;0;False;0;False;-1;None;323f9644f30289e48b27a7e3fd40be60;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;268;1499.914,-356.5967;Inherit;False;1046.668;419.9775;Comment;7;162;160;161;163;192;191;298;Opacity;0.2216981,0.3279938,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;162;1549.914,-53.3231;Inherit;False;Property;_WaterOpacityBlend;Water Opacity Blend;7;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;175;2102.741,1770.344;Inherit;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;250;-239.582,-1359.552;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;235;86.57323,-989.6245;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;161;1796.754,-71.61922;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;254;405.2165,-984.129;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;1556.808,-192.8324;Inherit;False;Property;_MainOpacity;Main Opacity;6;0;Create;True;0;0;0;False;0;False;0.2;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;292;1269.85,573.028;Inherit;False;1468.98;485.3479;Comment;12;281;299;287;301;300;288;271;270;269;293;275;294;Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;274;2448.189,1764.108;Inherit;False;WaterNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;294;1309.967,959.7166;Inherit;False;Property;_RefractionIntensity;Refraction Intensity;15;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;209;687.1531,-984.9767;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;2022.732,-287.4006;Inherit;False;Constant;_FoamOpacity;Foam Opacity;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;1319.85,822.1155;Inherit;False;274;WaterNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;2066.053,-185.2936;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;1575.359,831.7374;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;269;1320.627,623.028;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;191;2237.099,-293.0757;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;298;2341.197,-53.72485;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;270;1638.553,684.7803;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;300;1912.632,972.2975;Inherit;False;298;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;271;1812.614,678.446;Inherit;False;Global;_GrabScreen0;Grab Screen 0;17;0;Create;True;0;0;0;False;0;False;Object;-1;False;True;False;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;288;1844.819,865.4358;Inherit;False;Constant;_Float5;Float 5;17;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;2021.113,776.3759;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;301;2091.743,976.0032;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;264;1666.058,-2631.542;Inherit;False;1637.172;924.0256;Comment;9;184;185;159;154;157;156;155;284;290;Color;1,0.6486545,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;155;1721.146,-2180.897;Inherit;False;Property;_WaterColorBlend;Water Color Blend;3;0;Create;True;0;0;0;False;0;False;0;1.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;2194.269,775.8919;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;156;1719.058,-2403.542;Inherit;False;Property;_WaterDeepColor;Water Deep Color;4;0;Create;True;0;0;0;False;0;False;0.1320755,0.1320755,0.1320755,0;0.05490194,0.1977434,0.2078431,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;157;1716.058,-2581.542;Inherit;False;Property;_WaterShallowColor;Water Shallow Color;5;0;Create;True;0;0;0;False;0;False;0.1320755,0.1320755,0.1320755,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;291;2371.381,-1151.601;Inherit;False;912.8281;415.9839;Comment;6;247;248;166;246;165;245;Specularity & Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;281;2364.418,630.5789;Inherit;False;Refraction;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;261;1779.165,218.3072;Inherit;False;751.4045;186.9951;Water Edge;3;256;258;259;Water Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.DepthFade;154;2032.184,-2199.805;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;247;2421.381,-934.6172;Inherit;False;Constant;_Float12;Float 12;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;284;2390.77,-2230.555;Inherit;False;281;Refraction;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;248;2423.381,-853.6172;Inherit;False;Constant;_Float13;Float 13;25;0;Create;True;0;0;0;False;0;False;0.45;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;256;1829.165,270.3022;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.11;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;159;2355.817,-2532.189;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;258;2142.065,269.7776;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;166;2501.209,-1101.601;Inherit;False;Property;_WaterSpecularity;Water Specularity;9;0;Create;True;0;0;0;False;0;False;1;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;246;2599.381,-894.6172;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;185;2758.113,-1930.782;Inherit;False;Property;_FoamColor;Foam Color;10;0;Create;True;0;0;0;False;0;False;0.9150943,0.9150943,0.9150943,0;0.8277411,0.8326671,0.8396226,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;290;2811.833,-2251.842;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;276;4317.678,-812.3165;Inherit;False;274;WaterNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;165;2857.426,-850.7075;Inherit;False;Property;_WaterSmoothness;Water Smoothness;8;0;Create;True;0;0;0;False;0;False;0.95;0.82;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;259;2365.57,268.3072;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;2832.398,-982.8174;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;184;3041.657,-1958.203;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4642.718,-817.496;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;TriForge/SimpleWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;True;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;214;0;213;0
WireConnection;214;1;200;0
WireConnection;215;0;214;0
WireConnection;73;0;46;0
WireConnection;73;1;72;0
WireConnection;71;0;42;0
WireConnection;71;1;72;0
WireConnection;182;0;187;0
WireConnection;194;0;215;0
WireConnection;41;0;40;0
WireConnection;41;1;71;0
WireConnection;199;0;198;0
WireConnection;199;1;200;0
WireConnection;48;0;47;0
WireConnection;48;1;73;0
WireConnection;196;0;195;0
WireConnection;196;1;194;0
WireConnection;202;0;199;0
WireConnection;49;0;48;0
WireConnection;43;0;41;0
WireConnection;210;0;182;0
WireConnection;44;0;43;0
WireConnection;50;0;49;0
WireConnection;188;0;210;0
WireConnection;188;1;189;0
WireConnection;197;0;202;0
WireConnection;253;0;196;2
WireConnection;252;0;253;0
WireConnection;190;0;188;0
WireConnection;266;32;50;0
WireConnection;266;31;6;0
WireConnection;266;29;100;0
WireConnection;266;33;103;0
WireConnection;266;10;53;0
WireConnection;267;32;44;0
WireConnection;267;31;6;0
WireConnection;267;29;11;0
WireConnection;267;33;103;0
WireConnection;267;10;12;0
WireConnection;193;0;195;0
WireConnection;193;1;197;0
WireConnection;175;0;267;0
WireConnection;175;1;266;0
WireConnection;250;0;252;0
WireConnection;235;0;190;0
WireConnection;235;1;193;1
WireConnection;161;0;162;0
WireConnection;254;0;235;0
WireConnection;254;1;250;0
WireConnection;274;0;175;0
WireConnection;209;0;254;0
WireConnection;163;0;160;0
WireConnection;163;1;161;0
WireConnection;293;0;275;0
WireConnection;293;1;294;0
WireConnection;191;0;163;0
WireConnection;191;1;192;0
WireConnection;191;2;209;0
WireConnection;298;0;191;0
WireConnection;270;0;269;0
WireConnection;270;1;293;0
WireConnection;271;0;270;0
WireConnection;287;0;271;0
WireConnection;287;1;288;0
WireConnection;301;0;300;0
WireConnection;299;0;287;0
WireConnection;299;1;301;0
WireConnection;281;0;299;0
WireConnection;154;0;155;0
WireConnection;159;0;157;0
WireConnection;159;1;156;0
WireConnection;159;2;154;0
WireConnection;258;0;256;0
WireConnection;246;0;247;0
WireConnection;246;1;248;0
WireConnection;246;2;209;0
WireConnection;290;0;159;0
WireConnection;290;1;284;0
WireConnection;259;0;258;0
WireConnection;245;0;166;0
WireConnection;245;1;246;0
WireConnection;184;0;290;0
WireConnection;184;1;185;0
WireConnection;184;2;209;0
WireConnection;0;0;184;0
WireConnection;0;1;276;0
WireConnection;0;3;245;0
WireConnection;0;4;165;0
WireConnection;0;9;259;0
ASEEND*/
//CHKSM=B726FB7A9694E29B287F4301DC865AF55E0A7A6F