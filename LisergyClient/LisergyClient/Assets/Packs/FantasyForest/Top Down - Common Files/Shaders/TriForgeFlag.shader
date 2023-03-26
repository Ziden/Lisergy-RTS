// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/Top Down/Flag"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Color("Color", Color) = (0.08018869,0.3806623,1,0)
		_BaseColor("Base Color", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_MaskMap("Mask Map", 2D) = "black" {}
		_AnimationStrength("Animation Strength", Range( 0 , 3)) = 0
		_AnimationScale("Animation Scale", Range( 0 , 10)) = 0
		_AnimationSpeed("Animation Speed", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "DisableBatching" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _AnimationSpeed;
		uniform float _AnimationScale;
		uniform float _AnimationStrength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _BaseColor;
		uniform float4 _BaseColor_ST;
		uniform float4 _Color;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float _Cutoff = 0.5;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime7 = _Time.y * _AnimationSpeed;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 uv2_TexCoord27 = v.texcoord1.xy * float2( 2,-1.54 ) + ase_worldPos.xy;
			float2 panner18 = ( mulTime7 * float2( -1,0 ) + uv2_TexCoord27);
			float simplePerlin2D36 = snoise( panner18*( 0.2 * _AnimationScale ) );
			float2 uv2_TexCoord8 = v.texcoord1.xy * float2( 5,0.2 ) + ase_worldPos.xy;
			float2 panner4 = ( mulTime7 * float2( 0,-1 ) + uv2_TexCoord8);
			float simplePerlin2D50 = snoise( panner4*( 0.2 * _AnimationScale ) );
			float4 appendResult31 = (float4(( simplePerlin2D36 * 4.0 ) , ( simplePerlin2D50 + -5.0 ) , 0.0 , 0.0));
			v.vertex.xyz += ( ( appendResult31 * _AnimationStrength ) * v.texcoord1.xy.x ).xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_BaseColor = i.uv_texcoord * _BaseColor_ST.xy + _BaseColor_ST.zw;
			float4 tex2DNode51 = tex2D( _BaseColor, uv_BaseColor );
			o.Albedo = ( tex2DNode51 * _Color ).rgb;
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode54 = tex2D( _MaskMap, uv_MaskMap );
			o.Metallic = tex2DNode54.r;
			o.Smoothness = tex2DNode54.a;
			o.Occlusion = tex2DNode54.g;
			o.Alpha = 1;
			clip( tex2DNode51.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18933
-15;461;1920;673;3797.924;58.49799;1.268869;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;62;-3420.374,117.6592;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;63;-3176.34,216.9562;Inherit;False;Property;_AnimationSpeed;Animation Speed;7;0;Create;True;0;0;0;False;0;False;0;4.61;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;-2888.762,221.3501;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-2839.523,462.2871;Inherit;False;Property;_AnimationScale;Animation Scale;6;0;Create;True;0;0;0;False;0;False;0;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-2990.089,319.2125;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,-1.54;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;-2540.523,162.2871;Inherit;False;Constant;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-2985.956,53.3623;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,0.2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-2539.254,460.2116;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;4;-2701.87,51.06905;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;18;-2699.219,269.5649;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;-1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-2357.523,234.2871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-2374.523,532.2871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;36;-2179.477,398.8139;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;50;-2180.517,106.0622;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1574.803,467.4125;Inherit;False;Constant;_Float3;Float 3;8;0;Create;True;0;0;0;False;0;False;-5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2115.408,-20.82846;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-1363.609,315.489;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1776.635,207.5881;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-1172.78,178.1559;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1115.275,498.9709;Inherit;False;Property;_AnimationStrength;Animation Strength;5;0;Create;True;0;0;0;False;0;False;0;0.2;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-1601.959,-823.4784;Inherit;True;Property;_BaseColor;Base Color;2;0;Create;True;0;0;0;False;0;False;-1;None;e8fcab8d8e3409c439fcebf500601175;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-625.7503,483.4525;Inherit;True;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-752.6833,363.08;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;2;-1525.413,-587.6088;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;0.08018869,0.3806623,1,0;0.5471698,0.5471698,0.5471698,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-350.8218,302.5201;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1210.962,-734.829;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-2033.355,778.7857;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;54;-1602.602,-186.6748;Inherit;True;Property;_MaskMap;Mask Map;4;0;Create;True;0;0;0;False;0;False;-1;None;9a8e091abe1ea28469f11f21f38c054d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1786.693,832.0433;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1758.889,448.1528;Inherit;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;53;-1605.274,-393.5127;Inherit;True;Property;_Normal;Normal;3;0;Create;True;0;0;0;False;0;False;-1;None;44c70ba0ebf9bbd4e800d205cb0ff403;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;-1968.092,920.8625;Inherit;False;Constant;_Float5;Float 5;2;0;Create;True;0;0;0;False;0;False;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;46;-1555.34,643.4519;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TriForge/Top Down/Flag;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;63;0
WireConnection;27;1;62;0
WireConnection;8;1;62;0
WireConnection;4;0;8;0
WireConnection;4;1;7;0
WireConnection;18;0;27;0
WireConnection;18;1;7;0
WireConnection;60;0;58;0
WireConnection;60;1;61;0
WireConnection;59;0;57;0
WireConnection;59;1;61;0
WireConnection;36;0;18;0
WireConnection;36;1;59;0
WireConnection;50;0;4;0
WireConnection;50;1;60;0
WireConnection;65;0;50;0
WireConnection;65;1;64;0
WireConnection;28;0;36;0
WireConnection;28;1;29;0
WireConnection;31;0;28;0
WireConnection;31;1;65;0
WireConnection;55;0;31;0
WireConnection;55;1;56;0
WireConnection;32;0;55;0
WireConnection;32;1;17;1
WireConnection;52;0;51;0
WireConnection;52;1;2;0
WireConnection;48;0;42;2
WireConnection;48;1;49;0
WireConnection;46;0;50;0
WireConnection;46;1;47;0
WireConnection;46;2;48;0
WireConnection;0;0;52;0
WireConnection;0;1;53;0
WireConnection;0;3;54;1
WireConnection;0;4;54;4
WireConnection;0;5;54;2
WireConnection;0;10;51;4
WireConnection;0;11;32;0
ASEEND*/
//CHKSM=B78D00B5E23BB54E0BAD2E39BE3670FECBE7C109