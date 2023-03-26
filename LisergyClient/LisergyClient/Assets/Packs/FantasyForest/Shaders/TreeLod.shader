// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/TreeLod"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex1("Base Color", 2D) = "white" {}
		_MainTextureBrightness("Main Texture Brightness", Range( 0 , 2)) = 0.5
		_TopBrightness("Top Brightness", Range( 0 , 10)) = 3
		_Color("Color", Color) = (1,1,1,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _MainTex1;
		uniform float4 _MainTex1_ST;
		uniform float4 _Color;
		uniform float _MainTextureBrightness;
		uniform float _TopBrightness;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_MainTex1 = i.uv_texcoord * _MainTex1_ST.xy + _MainTex1_ST.zw;
			float4 tex2DNode3 = tex2D( _MainTex1, uv_MainTex1 );
			float4 temp_output_4_0 = ( ( tex2DNode3 * _Color ) * _MainTextureBrightness );
			float3 desaturateInitialColor11 = temp_output_4_0.rgb;
			float desaturateDot11 = dot( desaturateInitialColor11, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar11 = lerp( desaturateInitialColor11, desaturateDot11.xxx, 0.25 );
			float4 lerpResult15 = lerp( temp_output_4_0 , float4( ( desaturateVar11 * _TopBrightness ) , 0.0 ) , saturate( pow( i.uv2_texcoord2.y , 2.93 ) ));
			o.Albedo = ( lerpResult15 * 1.0 ).rgb;
			float temp_output_19_0 = 0.0;
			float3 temp_cast_3 = (temp_output_19_0).xxx;
			o.Specular = temp_cast_3;
			o.Smoothness = temp_output_19_0;
			o.Alpha = 1;
			clip( tex2DNode3.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
1920;66;1920;952;1920.546;518.2039;1;True;False
Node;AmplifyShaderEditor.ColorNode;24;-1832.239,-436.7929;Inherit;False;Property;_Color;Color;4;0;Create;True;0;0;False;0;False;1,1,1,0;0.9365748,1,0.8915094,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1910.073,-633.4023;Inherit;True;Property;_MainTex1;Base Color;1;0;Create;False;0;0;False;0;False;-1;None;d51b16f134b4bc8459f27029f5841049;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1494.453,-723.7745;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1649.998,-275.9969;Inherit;False;Property;_MainTextureBrightness;Main Texture Brightness;2;0;Create;True;0;0;False;0;False;0.5;1.41;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1309.382,-402.1539;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;5;-1096.059,-512.5289;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1588.732,112.9377;Inherit;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;False;2.93;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1654.299,-160.3759;Inherit;True;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1144.34,-276.2228;Inherit;False;Property;_TopBrightness;Top Brightness;3;0;Create;True;0;0;False;0;False;3;4;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;8;-1345.191,-84.58684;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;11;-1049.811,-403.0039;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.25;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;10;-810.2794,-533.3179;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-800.5664,-353.7169;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;13;-1041.667,-85.95183;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;12;-533.4304,-513.517;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-352.9753,-210.2669;Inherit;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;15;-386.2194,-384.1109;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.7783207,1,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-103.0179,-296.4459;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-50.90825,-6.230708;Inherit;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;171.0413,-80.0285;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;TriForge/TreeLod;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;3;0
WireConnection;22;1;24;0
WireConnection;4;0;22;0
WireConnection;4;1;2;0
WireConnection;5;0;4;0
WireConnection;8;0;7;2
WireConnection;8;1;6;0
WireConnection;11;0;4;0
WireConnection;10;0;5;0
WireConnection;14;0;11;0
WireConnection;14;1;9;0
WireConnection;13;0;8;0
WireConnection;12;0;10;0
WireConnection;15;0;12;0
WireConnection;15;1;14;0
WireConnection;15;2;13;0
WireConnection;17;0;15;0
WireConnection;17;1;16;0
WireConnection;0;0;17;0
WireConnection;0;3;19;0
WireConnection;0;4;19;0
WireConnection;0;10;3;4
ASEEND*/
//CHKSM=FEA55FE1164C0A810D105EDEE32BBEC5658BDA7B