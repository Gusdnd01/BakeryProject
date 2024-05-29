// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "1_fx/post_screen"
{
	Properties
	{
		_ucenter("ucenter", Float) = 0
		_vcenter("vcenter", Float) = 0
		_radial("radial", Float) = 0
		_length("length", Float) = 0
		_hole_rad("hole_rad", Float) = 0
		_hole_power("hole_power", Float) = 0
		_noise_upanner("noise_upanner", Float) = 0
		_noise_vpanner("noise_vpanner", Float) = 0
		_main_tex("main_tex", 2D) = "white" {}
		_noise_step("noise_step", Float) = 0
		_range("range", Float) = 0
		_color_a("color_a", Color) = (0,0,0,0)
		_color_b("color_b", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float4 _color_a;
			uniform float4 _color_b;
			uniform float _range;
			uniform sampler2D _main_tex;
			uniform float _noise_upanner;
			uniform float _noise_vpanner;
			uniform float _ucenter;
			uniform float _vcenter;
			uniform float _radial;
			uniform float _length;
			uniform float _hole_rad;
			uniform float _hole_power;
			uniform float _noise_step;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float temp_output_35_0 = ( 1.0 - _range );
				float3 temp_cast_0 = (temp_output_35_0).xxx;
				float3 temp_cast_1 = (( temp_output_35_0 + 0.0 )).xxx;
				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 desaturateInitialColor31 = tex2D( _MainTex, uv_MainTex ).rgb;
				float desaturateDot31 = dot( desaturateInitialColor31, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar31 = lerp( desaturateInitialColor31, desaturateDot31.xxx, 1.0 );
				float3 smoothstepResult37 = smoothstep( temp_cast_0 , temp_cast_1 , desaturateVar31);
				float4 color45 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float4 color46 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float2 appendResult22 = (float2(_noise_upanner , _noise_vpanner));
				float2 appendResult3 = (float2(_ucenter , _vcenter));
				float2 CenteredUV15_g1 = ( i.texcoord.xy - appendResult3 );
				float2 break17_g1 = CenteredUV15_g1;
				float2 appendResult23_g1 = (float2(( length( CenteredUV15_g1 ) * _radial * 2.0 ) , ( atan2( break17_g1.x , break17_g1.y ) * ( 1.0 / 6.28318548202515 ) * _length )));
				float2 panner23 = ( 1.0 * _Time.y * appendResult22 + appendResult23_g1);
				float2 texCoord7 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_9_0 = ( ( texCoord7 - appendResult3 ) * 2.0 );
				float2 temp_output_11_0 = ( temp_output_9_0 * temp_output_9_0 );
				float4 lerpResult41 = lerp( color45 , color46 , step( ( tex2D( _main_tex, panner23 ).r * pow( saturate( ( ( (temp_output_11_0).x + (temp_output_11_0).y ) - _hole_rad ) ) , _hole_power ) ) , _noise_step ));
				float4 lerpResult44 = lerp( _color_a , _color_b , ( float4( smoothstepResult37 , 0.0 ) + lerpResult41 ));
				

				float4 color = lerpResult44;
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.FunctionNode;4;-1739.723,-146.3908;Inherit;True;Polar Coordinates;-1;;1;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1922.723,-43.39077;Inherit;False;Property;_radial;radial;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1921.723,26.60923;Inherit;False;Property;_length;length;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-2512.723,-122.3908;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2680.723,-52.39077;Inherit;False;Property;_vcenter;vcenter;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-2680.723,-126.3908;Inherit;False;Property;_ucenter;ucenter;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-2686.723,269.6093;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;8;-2320.723,268.6093;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2172.723,269.6093;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2316.723,360.6093;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-2035.794,272.111;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;12;-1887.794,267.111;Inherit;False;True;False;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;13;-1889.794,337.111;Inherit;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1676.794,271.111;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1606.794,480.111;Inherit;False;Property;_hole_rad;hole_rad;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;-1419.794,270.111;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;17;-1292.794,269.111;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;-1157.794,269.111;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1354.794,385.111;Inherit;False;Property;_hole_power;hole_power;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1594.046,74.49121;Inherit;False;Property;_noise_upanner;noise_upanner;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1595.555,151.4514;Inherit;False;Property;_noise_vpanner;noise_vpanner;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1413.025,78.86166;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;23;-1225.025,-144.1383;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;24;-1039.025,-173.1383;Inherit;True;Property;_main_tex;main_tex;8;0;Create;True;0;0;0;False;0;False;-1;74325abcea916354f85d4f10c238e82f;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-662.2534,-145.9155;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-680.2534,-51.91553;Inherit;False;Property;_noise_step;noise_step;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;26;-474.2534,-144.9155;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-563.4969,-666.9969;Inherit;False;Property;_range;range;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-562.7729,-592.72;Inherit;False;Constant;_smooth;smooth;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;35;-440.7917,-663.2554;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-286.6055,-612.632;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;37;-159.8564,-687.381;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-564.1385,-742.8184;Inherit;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;29;-858.1392,-919.8184;Inherit;False;0;0;_MainTex;Pass;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-718.139,-924.8184;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;31;-394.1382,-918.8184;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;59.88226,-686.2465;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;41;-279.0712,-469.4274;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;44;258.1164,-1012.394;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;45;-556.9048,-479.5324;Inherit;False;Constant;_Color0;Color 0;13;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;-556.9048,-312.5324;Inherit;False;Constant;_Color1;Color 1;13;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;414.0515,-1012.664;Float;False;True;-1;2;ASEMaterialInspector;0;8;1_fx/post_screen;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;True;7;False;;False;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.ColorNode;42;-18.37275,-1021.055;Inherit;False;Property;_color_a;color_a;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;43;-22.34156,-854.9673;Inherit;False;Property;_color_b;color_b;12;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;4;2;3;0
WireConnection;4;3;5;0
WireConnection;4;4;6;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;8;0;7;0
WireConnection;8;1;3;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;11;0;9;0
WireConnection;11;1;9;0
WireConnection;12;0;11;0
WireConnection;13;0;11;0
WireConnection;14;0;12;0
WireConnection;14;1;13;0
WireConnection;16;0;14;0
WireConnection;16;1;15;0
WireConnection;17;0;16;0
WireConnection;18;0;17;0
WireConnection;18;1;19;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;23;0;4;0
WireConnection;23;2;22;0
WireConnection;24;1;23;0
WireConnection;25;0;24;1
WireConnection;25;1;18;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;35;0;33;0
WireConnection;36;0;35;0
WireConnection;36;1;34;0
WireConnection;37;0;31;0
WireConnection;37;1;35;0
WireConnection;37;2;36;0
WireConnection;30;0;29;0
WireConnection;31;0;30;0
WireConnection;31;1;32;0
WireConnection;38;0;37;0
WireConnection;38;1;41;0
WireConnection;41;0;45;0
WireConnection;41;1;46;0
WireConnection;41;2;26;0
WireConnection;44;0;42;0
WireConnection;44;1;43;0
WireConnection;44;2;38;0
WireConnection;0;0;44;0
ASEEND*/
//CHKSM=B0B26FEE20A8DE965E2671741126F4B0686DAAD3