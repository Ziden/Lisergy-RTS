// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Flame"
{
	Properties
	{
		_MainTex("Particle Texture", 2D) = "white" {}
		[HDR]_TintColor("Tint Color", Color) = (0.5294118,0.5294118,0.5294118,0)
		_DistortionStrength("Distortion Strength", Range( 0 , 1)) = 0.1126335
		_DissolveStrength("Dissolve Strength", Range( 0 , 10)) = 1
		_DissolveScale("Dissolve Scale", Range( 0 , 10)) = 2
		_DistortionScale("Distortion Scale", Range( 0 , 10)) = 2
		_EffectSpeed("Effect Speed", Range( 0.1 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform sampler2D _MainTex;
		uniform float _EffectSpeed;
		uniform float _DistortionScale;
		uniform float _DistortionStrength;
		uniform float _DissolveScale;
		uniform float _DissolveStrength;
		uniform float4 _TintColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float2 voronoihash23( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi23( float2 v, float time, inout float2 id, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash23( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime13 = _Time.y * _EffectSpeed;
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_50_0 = ( mulTime13 + ( ase_worldPos * 2.0 ) );
			float2 uv_TexCoord11 = i.uv_texcoord + ( (( float3( float2( 0,-0.3 ) ,  0.0 ) * temp_output_50_0 )*1.0 + 0.0) + float3( 0,0,0 ) ).xy;
			float simplePerlin2D10 = snoise( uv_TexCoord11*_DistortionScale );
			simplePerlin2D10 = simplePerlin2D10*0.5 + 0.5;
			float2 temp_cast_2 = (simplePerlin2D10).xx;
			float2 lerpResult17 = lerp( i.uv_texcoord , temp_cast_2 , _DistortionStrength);
			float4 tex2DNode1 = tex2D( _MainTex, lerpResult17 );
			float time23 = 0.0;
			float2 temp_cast_5 = (( (0.0*1.0 + ( float3( float2( 0,-0.8 ) ,  0.0 ) * temp_output_50_0 ).x) + 0.0 )).xx;
			float2 uv_TexCoord24 = i.uv_texcoord + temp_cast_5;
			float2 coords23 = uv_TexCoord24 * _DissolveScale;
			float2 id23 = 0;
			float voroi23 = voronoi23( coords23, time23,id23, 0 );
			float temp_output_27_0 = ( simplePerlin2D10 * pow( voroi23 , _DissolveStrength ) );
			float4 temp_output_8_0 = ( ( ( tex2DNode1 * temp_output_27_0 ) * _TintColor ) * i.vertexColor );
			o.Albedo = temp_output_8_0.rgb;
			o.Emission = temp_output_8_0.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth55 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth55 = abs( ( screenDepth55 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 3.0 ) );
			o.Alpha = saturate( ( ( ( tex2DNode1.a * temp_output_27_0 ) * i.vertexColor.a ) * distanceDepth55 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
1920;0;1920;1018;3515.369;358.3296;1.683267;True;False
Node;AmplifyShaderEditor.RangedFloatNode;47;-5266.844,-196.0866;Inherit;False;Property;_EffectSpeed;Effect Speed;6;0;Create;True;0;0;False;0;False;1;2;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-5090.014,132.2609;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;33;-5137.557,-39.89352;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-4845.414,42.76078;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;13;-4943.818,-192.4239;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;14;-4584.27,-357.9478;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;False;0;False;0,-0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;22;-4596.723,-6.711517;Inherit;False;Constant;_Vector1;Vector 1;2;0;Create;True;0;0;False;0;False;0,-0.8;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-4714.414,-147.2392;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-4297.808,-282.4091;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-4299.251,-2.334686;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;12;-4134.375,-286.6099;Inherit;False;3;0;FLOAT3;1,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;20;-4132.273,-7.716888;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-3852.566,-178.7775;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-3840.466,85.95154;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-3569.528,161.652;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-3570.075,318.5755;Inherit;False;Property;_DissolveScale;Dissolve Scale;4;0;Create;True;0;0;False;0;False;2;3.53;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3582.173,-199.0606;Inherit;False;Property;_DistortionScale;Distortion Scale;5;0;Create;True;0;0;False;0;False;2;3.83;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-3573.09,-112.954;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-2852.644,777.7291;Inherit;False;Property;_DissolveStrength;Dissolve Strength;3;0;Create;True;0;0;False;0;False;1;0.44;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-3582.18,-277.5841;Inherit;False;Property;_DistortionStrength;Distortion Strength;2;0;Create;True;0;0;False;0;False;0.1126335;0.191;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-3543.199,-415.0712;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;10;-3301.58,-117.927;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;23;-3296.622,159.2891;Inherit;True;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3;False;3;FLOAT;0;False;2;FLOAT;0;FLOAT2;1
Node;AmplifyShaderEditor.LerpOp;17;-2501.407,-353.0669;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;25;-2511.934,323.3083;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2245.672,-351.0429;Inherit;True;Property;_MainTex;Particle Texture;0;0;Create;False;0;0;False;0;False;-1;5228a04ef529d2641937cab585cc1a02;95103f0f012cb1d488c1fdec2ea9f4be;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2242.828,245.1489;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;7;-1382.718,504.3184;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1413.115,254.8296;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1983.217,159.5697;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-1514.083,-274.0204;Inherit;False;Property;_TintColor;Tint Color;1;1;[HDR];Create;True;0;0;False;0;False;0.5294118,0.5294118,0.5294118,0;178.2618,22.39939,0,0.5;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-864.5623,370.844;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;55;-905.9954,574.6116;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-563.3408,456.2129;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1198.145,-131.2498;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;58;-342.4095,454.6884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-867.3324,150.6742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4.342565,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Flame;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;8;5;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;33;0
WireConnection;53;1;54;0
WireConnection;13;0;47;0
WireConnection;50;0;13;0
WireConnection;50;1;53;0
WireConnection;15;0;14;0
WireConnection;15;1;50;0
WireConnection;19;0;22;0
WireConnection;19;1;50;0
WireConnection;12;0;15;0
WireConnection;20;2;19;0
WireConnection;44;0;12;0
WireConnection;43;0;20;0
WireConnection;24;1;43;0
WireConnection;11;1;44;0
WireConnection;10;0;11;0
WireConnection;10;1;45;0
WireConnection;23;0;24;0
WireConnection;23;2;46;0
WireConnection;17;0;16;0
WireConnection;17;1;10;0
WireConnection;17;2;18;0
WireConnection;25;0;23;0
WireConnection;25;1;26;0
WireConnection;1;1;17;0
WireConnection;27;0;10;0
WireConnection;27;1;25;0
WireConnection;49;0;1;4
WireConnection;49;1;27;0
WireConnection;28;0;1;0
WireConnection;28;1;27;0
WireConnection;9;0;49;0
WireConnection;9;1;7;4
WireConnection;57;0;9;0
WireConnection;57;1;55;0
WireConnection;6;0;28;0
WireConnection;6;1;3;0
WireConnection;58;0;57;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;0;0;8;0
WireConnection;0;2;8;0
WireConnection;0;9;58;0
ASEEND*/
//CHKSM=715F1C1CA0E53CFE28CA95C1CF86AED756991A8A