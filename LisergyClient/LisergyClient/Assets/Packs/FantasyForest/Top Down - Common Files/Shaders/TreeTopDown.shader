// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/Top Down/Tree"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Base Color", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_MaskMap("Mask Map", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_AOIntensity("AO Intensity", Range( 0 , 1)) = 1
		_MainTextureBrightness("Main Texture Brightness", Range( 0 , 2)) = 0.5
		[Toggle(_USEGRADIENT_ON)] _UseGradient("UseGradient", Float) = 0
		_DetailBendingScale("Detail Bending Scale", Float) = 5
		_GradientPosition("Gradient Position", Float) = 2.93
		_TopBrightness("Top Brightness", Range( 0 , 10)) = 3
		[HDR]_TopColor("Top Color", Color) = (0,0,0,0)
		[HDR]_Color("Color", Color) = (0,0,0,0)
		[KeywordEnum(UV2,UV3)] _GradientUV("Gradient UV", Float) = 0
		_NormalIntensity("Normal Intensity", Float) = 1
		_WindDirectionRandomness("Wind Direction Randomness", Range( 0 , 1)) = 0
		_WindMultiplier("Wind Multiplier", Float) = 1
		[Toggle(_USECOLORVARIANCE_ON)] _UseColorVariance("UseColorVariance", Float) = 0
		_ColorVarianceAmount("Color Variance Amount", Range( 0 , 1)) = 0.44
		_ColorVariationMask("Color Variation Mask", 2D) = "white" {}
		_DetailBendingIntensity("Detail Bending Intensity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "DisableBatching" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _USECOLORVARIANCE_ON
		#pragma shader_feature_local _USEGRADIENT_ON
		#pragma shader_feature_local _GRADIENTUV_UV2 _GRADIENTUV_UV3
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nolightmap  nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float2 uv3_texcoord3;
		};

		uniform float3 TD_WindDirection;
		uniform float _WindDirectionRandomness;
		uniform float _DetailBendingIntensity;
		uniform float _DetailBendingScale;
		uniform float TD_WindStrength;
		uniform float _WindMultiplier;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalIntensity;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float _MainTextureBrightness;
		uniform float4 _TopColor;
		uniform float _TopBrightness;
		uniform float _GradientPosition;
		uniform sampler2D _ColorVariationMask;
		uniform float _ColorVarianceAmount;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float _Smoothness;
		uniform float _AOIntensity;
		uniform float _Cutoff = 0.5;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float ifLocalVar183 = 0;
			if( TD_WindDirection.x == 0.0 )
				ifLocalVar183 = (float)1;
			float ifLocalVar186 = 0;
			if( TD_WindDirection.z == 0.0 )
				ifLocalVar186 = (float)1;
			float3 lerpResult187 = lerp( TD_WindDirection , float3(1,0,0) , ( ifLocalVar183 * ifLocalVar186 ));
			float3 worldToObjDir94 = normalize( mul( unity_WorldToObject, float4( lerpResult187, 0 ) ).xyz );
			float3 lerpResult173 = lerp( worldToObjDir94 , lerpResult187 , _WindDirectionRandomness);
			float3 objToWorld214 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float2 panner82 = ( ( _Time.y * 1.0 ) * float2( 0.12,0 ) + ( (objToWorld214).xz / 50.0 ));
			float simplePerlin3D80 = snoise( float3( panner82 ,  0.0 )*3.0 );
			simplePerlin3D80 = simplePerlin3D80*0.5 + 0.5;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner102 = ( ( _Time.y * 0.6 ) * float2( -0.1,0.1 ) + ( (ase_worldPos).xz / _DetailBendingScale ));
			float simplePerlin3D97 = snoise( float3( panner102 ,  0.0 )*10.0 );
			simplePerlin3D97 = simplePerlin3D97*0.5 + 0.5;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue59 = RotateAroundAxis( float3(0,0,0), ase_vertex3Pos, lerpResult173, radians( ( ( (-0.4 + (simplePerlin3D80 - 0.0) * (0.6 - -0.4) / (1.0 - 0.0)) + ( _DetailBendingIntensity * simplePerlin3D97 ) ) * 30.0 ) ) );
			v.vertex.xyz += ( ( v.texcoord1.xy.y * ( rotatedValue59 - ase_vertex3Pos ) ) * ( TD_WindStrength * _WindMultiplier ) );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalIntensity );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_155_0 = ( tex2DNode1 * _Color );
			float4 temp_output_47_0 = ( temp_output_155_0 * _MainTextureBrightness );
			float3 desaturateInitialColor46 = temp_output_47_0.rgb;
			float desaturateDot46 = dot( desaturateInitialColor46, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar46 = lerp( desaturateInitialColor46, desaturateDot46.xxx, 0.25 );
			#if defined(_GRADIENTUV_UV2)
				float staticSwitch166 = i.uv2_texcoord2.y;
			#elif defined(_GRADIENTUV_UV3)
				float staticSwitch166 = i.uv3_texcoord3.y;
			#else
				float staticSwitch166 = i.uv2_texcoord2.y;
			#endif
			float4 lerpResult32 = lerp( temp_output_47_0 , ( float4( desaturateVar46 , 0.0 ) * ( _TopColor * _TopBrightness ) ) , saturate( pow( staticSwitch166 , _GradientPosition ) ));
			float4 temp_output_42_0 = ( lerpResult32 * 1.0 );
			#ifdef _USEGRADIENT_ON
				float4 staticSwitch170 = temp_output_42_0;
			#else
				float4 staticSwitch170 = temp_output_155_0;
			#endif
			float dotResult205 = dot( temp_output_42_0 , float4( float3(0.55,0.55,0.55) , 0.0 ) );
			float3 objToWorld204 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float4 lerpResult210 = lerp( staticSwitch170 , ( ( dotResult205 * 5.0 ) * tex2D( _ColorVariationMask, ( (objToWorld204).xz / 50.0 ) ) ) , _ColorVarianceAmount);
			#ifdef _USECOLORVARIANCE_ON
				float4 staticSwitch212 = lerpResult210;
			#else
				float4 staticSwitch212 = staticSwitch170;
			#endif
			o.Albedo = staticSwitch212.rgb;
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode3 = tex2D( _MaskMap, uv_MaskMap );
			o.Smoothness = ( tex2DNode3.a * _Smoothness );
			float lerpResult162 = lerp( 1.0 , tex2DNode3.g , _AOIntensity);
			o.Occlusion = lerpResult162;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18933
2231;206;1439;820;5947.945;-676.4253;1.772041;True;False
Node;AmplifyShaderEditor.CommentaryNode;230;-5238.056,2801.559;Inherit;False;1444.798;504.833;Comment;11;98;103;101;104;99;105;100;102;97;228;225;Detail Bending;0,0.7498155,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;222;-5237.201,2076.563;Inherit;False;1480.792;513.0635;Comment;11;214;215;55;87;58;85;57;82;80;151;150;Main Bending;0,0.8396716,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;52;-3369.732,-1639.619;Inherit;False;1982.951;1064.909;Top-down gradient;23;43;42;31;20;21;7;51;50;49;32;37;44;46;47;48;1;154;166;167;170;171;172;155;Main Color Gradient;0.5744526,1,0,1;0;0
Node;AmplifyShaderEditor.ColorNode;154;-3334.261,-1357.855;Inherit;False;Property;_Color;Color;12;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.1123447,0.2735849,0.05032931,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;98;-5188.056,2972.349;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;1;-3333.033,-1555.294;Inherit;True;Property;_MainTex;Base Color;1;0;Create;False;0;0;0;False;0;False;-1;None;5e7bd8d93dba93d468c6a5f482759dbd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;214;-5119.097,2126.563;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;87;-4634.053,2134.309;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-2949.211,-1505.58;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;215;-4850.19,2243.841;Inherit;False;Constant;_Float9;Float 9;20;0;Create;True;0;0;0;False;0;False;50;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3332.247,-1174.743;Inherit;False;Property;_MainTextureBrightness;Main Texture Brightness;6;0;Create;True;0;0;0;False;0;False;0.5;0.63;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-4607.278,2466.097;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;55;-4631.156,2380.321;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-4956.615,3190.392;Inherit;False;Constant;_Float6;Float 6;7;0;Create;True;0;0;0;False;0;False;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-5156.843,3136.23;Inherit;False;Property;_DetailBendingScale;Detail Bending Scale;8;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;103;-4959.108,3104.615;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;101;-4966.201,2967.663;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;167;-3335.985,-942.8135;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-3334.847,-1078.022;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-4429.876,2383.807;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;85;-4531.122,2251.687;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;100;-4645.798,2915.908;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-4768.52,3105.428;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-2875.932,-1317.8;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;221;-5180.877,936.319;Inherit;False;1712.018;660.969;Check if WindDirection is being read from the script, if not use a default direction vector;10;177;185;187;65;192;186;183;173;94;174;Direction;1,0.08150674,0,1;0;0
Node;AmplifyShaderEditor.ColorNode;171;-2770.856,-1194.416;Inherit;False;Property;_TopColor;Top Color;11;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.9245283,0.7200845,0.2921858,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;185;-5091.628,1121.766;Inherit;False;Constant;_Int0;Int 0;18;0;Create;True;0;0;0;False;0;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.Vector3Node;177;-5154.974,1231.878;Inherit;False;Global;TD_WindDirection;TD_WindDirection;18;0;Create;True;0;0;0;False;0;False;0,0,0;-0.8013381,0,0.5982117;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;50;-2668.71,-1375.475;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2931.083,-702.0089;Inherit;False;Property;_GradientPosition;Gradient Position;9;0;Create;True;0;0;0;False;0;False;2.93;2.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;102;-4602.408,3044.49;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;166;-3037.987,-851.8135;Inherit;False;Property;_GradientUV;Gradient UV;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;UV2;UV3;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-2837.892,-1013.87;Inherit;False;Property;_TopBrightness;Top Brightness;10;0;Create;True;0;0;0;False;0;False;3;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;82;-4220.995,2336.233;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;0.12,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ConditionalIfNode;183;-4817.134,987.7911;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;51;-2382.028,-1390.565;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;20;-2675.341,-902.6335;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-2488.604,-1123.541;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;46;-2616.361,-1318.65;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.25;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;225;-4400.977,2851.559;Inherit;False;Property;_DetailBendingIntensity;Detail Bending Intensity;21;0;Create;True;0;0;0;False;0;False;0;0.2667871;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;97;-4359.452,3039.809;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;1,1,0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;80;-4020.408,2330.627;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;1,1,0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;186;-4816.074,1180.972;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;-4618.236,1145.204;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;65;-4581.937,989.1014;Inherit;False;Constant;_DefaultDirection;Default Direction;8;0;Create;True;0;0;0;False;0;False;1,0,0;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-2367.115,-1269.363;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;148;-2988.599,2280.67;Inherit;False;1656.133;780.9799;Wind;7;62;59;78;71;61;72;73;Wind;0.2795924,0.6454769,0.9716981,1;0;0
Node;AmplifyShaderEditor.SaturateNode;31;-2522.215,-905.5986;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;49;-2106.38,-1368.363;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-4028.254,2915.021;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;213;-3092.977,-461.4115;Inherit;False;1450.198;630.7034;Comment;12;204;206;198;196;197;209;199;205;208;207;211;210;Color Variance;1,0.3160377,0.3160377,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;106;-3641.997,2545.502;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.4;False;4;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;-4389.162,1150.9;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2788.837,2738.184;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;0;False;0;False;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;229;-3340.884,2758.029;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-2071.653,-1293.388;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.7783207,1,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformPositionNode;204;-3042.977,-52.59377;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;43;-2156.02,-1096.986;Inherit;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-4100.906,1435.215;Inherit;False;Property;_WindDirectionRandomness;Wind Direction Randomness;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;94;-4043.07,1069.084;Inherit;False;World;Object;True;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;196;-2821.826,-52.60707;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1928.616,-1179.764;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;206;-2892.452,-374.1135;Inherit;False;Constant;_Vector0;Vector 0;18;0;Create;True;0;0;0;False;0;False;0.55,0.55,0.55;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2581.408,2601.777;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;198;-2768.837,44.14272;Inherit;False;Constant;_Float1;Float 1;17;0;Create;True;0;0;0;False;0;False;50;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;173;-3795.252,1282.35;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-2767.98,-176.0633;Inherit;False;Constant;_Float8;Float 8;18;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;205;-2621.869,-387.0869;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;61;-2393.016,2834.162;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;71;-2346.581,2602.246;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;78;-2372.078,2680.2;Inherit;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;197;-2605.912,-32.53487;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;149;-974.6036,979.8713;Inherit;False;484.7685;409.0511;;2;53;143;Mask Wind by UV2;1,0.5394964,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;199;-2439.782,-60.70807;Inherit;True;Property;_ColorVariationMask;Color Variation Mask;20;0;Create;True;0;0;0;False;0;False;-1;c849c2a1b3558b14d8f1d7f81d5841cf;f4fe61ba46dee6840af24529c05d9609;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-2450.632,-388.3665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;59;-2095.981,2617.124;Inherit;False;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-952.7106,1031.04;Inherit;True;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;62;-1743.888,2683.958;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-369.8595,1279.368;Inherit;False;Property;_WindMultiplier;Wind Multiplier;17;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;170;-1610.777,-856.3892;Inherit;True;Property;_UseGradient;UseGradient;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-2229.479,-388.7148;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-2139.156,-229.6611;Inherit;False;Property;_ColorVarianceAmount;Color Variance Amount;19;0;Create;True;0;0;0;False;0;False;0.44;0.44;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-463.1819,1184.026;Inherit;False;Global;TD_WindStrength;TD_Wind Strength;8;0;Create;True;0;0;0;False;0;False;0.5;0.354;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-174.0103,1224.507;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-1473.205,27.25467;Inherit;False;Property;_NormalIntensity;Normal Intensity;14;0;Create;True;0;0;0;False;0;False;1;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;210;-1824.78,-411.4115;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1150.234,630.3875;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;157;-993.6711,250.6216;Inherit;False;Property;_AOIntensity;AO Intensity;5;0;Create;True;0;0;0;False;0;False;1;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-642.9866,1049.106;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;3;-1166.234,395.1477;Inherit;True;Property;_MaskMap;Mask Map;3;0;Create;True;0;0;0;False;0;False;-1;None;a52975599a7767f43a89f55de1c6fafb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;163;-857.2272,166.4388;Inherit;False;Constant;_Float7;Float 7;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-828.3535,558.8832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;212;-1235.073,-580.5687;Inherit;False;Property;_UseColorVariance;UseColorVariance;18;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-1200.27,-22.45811;Inherit;True;Property;_Normal;Normal;2;0;Create;True;0;0;0;False;0;False;-1;None;08a30fb1a7545064a9adc25fc25f1c04;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-4850.568,2323.232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-5187.201,2336.769;Inherit;False;Property;_WindNoiseScale;Wind Noise Scale;15;0;Create;True;0;0;0;False;0;False;1;1;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-31.84179,1042.851;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;162;-694.2272,173.4388;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TriForge/Top Down/Tree;False;False;False;False;False;False;True;False;True;False;False;False;True;True;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;214;0
WireConnection;155;0;1;0
WireConnection;155;1;154;0
WireConnection;101;0;98;0
WireConnection;57;0;55;0
WireConnection;57;1;58;0
WireConnection;85;0;87;0
WireConnection;85;1;215;0
WireConnection;100;0;101;0
WireConnection;100;1;99;0
WireConnection;105;0;103;0
WireConnection;105;1;104;0
WireConnection;47;0;155;0
WireConnection;47;1;48;0
WireConnection;50;0;47;0
WireConnection;102;0;100;0
WireConnection;102;1;105;0
WireConnection;166;1;7;2
WireConnection;166;0;167;2
WireConnection;82;0;85;0
WireConnection;82;1;57;0
WireConnection;183;0;177;1
WireConnection;183;3;185;0
WireConnection;51;0;50;0
WireConnection;20;0;166;0
WireConnection;20;1;21;0
WireConnection;172;0;171;0
WireConnection;172;1;44;0
WireConnection;46;0;47;0
WireConnection;97;0;102;0
WireConnection;80;0;82;0
WireConnection;186;0;177;3
WireConnection;186;3;185;0
WireConnection;192;0;183;0
WireConnection;192;1;186;0
WireConnection;37;0;46;0
WireConnection;37;1;172;0
WireConnection;31;0;20;0
WireConnection;49;0;51;0
WireConnection;228;0;225;0
WireConnection;228;1;97;0
WireConnection;106;0;80;0
WireConnection;187;0;177;0
WireConnection;187;1;65;0
WireConnection;187;2;192;0
WireConnection;229;0;106;0
WireConnection;229;1;228;0
WireConnection;32;0;49;0
WireConnection;32;1;37;0
WireConnection;32;2;31;0
WireConnection;94;0;187;0
WireConnection;196;0;204;0
WireConnection;42;0;32;0
WireConnection;42;1;43;0
WireConnection;72;0;229;0
WireConnection;72;1;73;0
WireConnection;173;0;94;0
WireConnection;173;1;187;0
WireConnection;173;2;174;0
WireConnection;205;0;42;0
WireConnection;205;1;206;0
WireConnection;71;0;72;0
WireConnection;197;0;196;0
WireConnection;197;1;198;0
WireConnection;199;1;197;0
WireConnection;208;0;205;0
WireConnection;208;1;209;0
WireConnection;59;0;173;0
WireConnection;59;1;71;0
WireConnection;59;2;78;0
WireConnection;59;3;61;0
WireConnection;62;0;59;0
WireConnection;62;1;61;0
WireConnection;170;1;155;0
WireConnection;170;0;42;0
WireConnection;207;0;208;0
WireConnection;207;1;199;0
WireConnection;175;0;146;0
WireConnection;175;1;176;0
WireConnection;210;0;170;0
WireConnection;210;1;207;0
WireConnection;210;2;211;0
WireConnection;143;0;53;2
WireConnection;143;1;62;0
WireConnection;4;0;3;4
WireConnection;4;1;5;0
WireConnection;212;1;170;0
WireConnection;212;0;210;0
WireConnection;2;5;168;0
WireConnection;150;1;151;0
WireConnection;145;0;143;0
WireConnection;145;1;175;0
WireConnection;162;0;163;0
WireConnection;162;1;3;2
WireConnection;162;2;157;0
WireConnection;0;0;212;0
WireConnection;0;1;2;0
WireConnection;0;4;4;0
WireConnection;0;5;162;0
WireConnection;0;10;1;4
WireConnection;0;11;145;0
ASEEND*/
//CHKSM=CB4DF6B748A2EC9FE8E062CC29C5408B9BFB4216