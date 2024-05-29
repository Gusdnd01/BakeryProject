// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "1_fx/alphablended_mask"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_Main_Texture("Main_Texture", 2D) = "white" {}
		_main_upanner("main_upanner", Float) = 0
		_main_vpanner("main_vpanner", Float) = 0
		_Main_Pow("Main_Pow", Range( 1 , 10)) = 1.952941
		_Main_Ins("Main_Ins", Range( 1 , 10)) = 1
		[HDR]_main_color_b("main_color_b", Color) = (1,1,1,0)
		[HDR]_main_color_a("main_color_a", Color) = (1,1,1,0)
		_Opacity("Opacity", Range( 0 , 10)) = 1
		[Toggle(_USE_CUSTOMDATA_ON)] _Use_Customdata("Use_Customdata", Float) = 0
		_Dissolve_Texture("Dissolve_Texture", 2D) = "white" {}
		_Dissolve("Dissolve", Range( -1 , 1)) = 1
		_Fade_Ditance("Fade_Ditance", Float) = 0
		_normal_tex("normal_tex", 2D) = "bump" {}
		_normal_radial("normal_radial", Float) = 0
		_normal_upanner("normal_upanner", Float) = 1
		_normal_vpanner("normal_vpanner", Float) = 1
		_normal_str("normal_str", Range( 0 , 1)) = 0
		_diss_normal_tex("diss_normal_tex", 2D) = "bump" {}
		_diss_normal_upanner("diss_normal_upanner", Float) = 0
		_diss_normal_vpanner("diss_normal_vpanner", Float) = 0
		_diss_normal_power("diss_normal_power", Range( 0 , 1)) = 0
		[Toggle(_USE_TOONDISSOLVE_ON)] _Use_ToonDissolve("Use_ToonDissolve", Float) = 0
		[Toggle(_USE_LERP_ON)] _Use_lerp("Use_lerp", Float) = 0
		[Toggle(_USE_POLAR_ON)] _use_polar("use_polar", Float) = 0
		[Toggle(_USE_OTHER_DISS_ON)] _use_other_diss("use_other_diss", Float) = 0
		_edge_width("edge_width", Float) = 0.06
		[HDR]_edge_color("edge_color", Color) = (0,0,0,0)
		[Toggle(USE_MASK_Y_DIR_ON)] use_mask_y_dir("use_mask_y_dir", Float) = 0
		[ASEEnd][Toggle(USE_MASK_ONE_MINUS_ON)] use_mask_one_minus("use_mask_one_minus", Float) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull Off
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 140008
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma instancing_options renderinglayer

			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma shader_feature _ _SAMPLE_GI
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        	#pragma multi_compile_fragment _ DEBUG_DISPLAY
        	#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        	#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USE_LERP_ON
			#pragma shader_feature_local _USE_POLAR_ON
			#pragma shader_feature_local _USE_CUSTOMDATA_ON
			#pragma shader_feature_local _USE_TOONDISSOLVE_ON
			#pragma shader_feature_local _USE_OTHER_DISS_ON
			#pragma shader_feature_local USE_MASK_ONE_MINUS_ON
			#pragma shader_feature_local USE_MASK_Y_DIR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _main_color_a;
			float4 _main_color_b;
			float4 _Dissolve_Texture_ST;
			float4 _normal_tex_ST;
			float4 _Main_Texture_ST;
			float4 _edge_color;
			float _edge_width;
			float _diss_normal_power;
			float _diss_normal_vpanner;
			float _diss_normal_upanner;
			float _Dissolve;
			float _Main_Pow;
			float _Opacity;
			float _normal_str;
			float _normal_radial;
			float _normal_vpanner;
			float _normal_upanner;
			float _main_vpanner;
			float _main_upanner;
			float _Main_Ins;
			float _Fade_Ditance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_Texture;
			sampler2D _normal_tex;
			sampler2D _diss_normal_tex;
			sampler2D _Dissolve_Texture;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN
				#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 vert_color119 = IN.ase_color;
				float2 texCoord66 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 lerpResult64 = lerp( _main_color_a , _main_color_b , texCoord66.x);
				#ifdef _USE_LERP_ON
				float4 staticSwitch70 = lerpResult64;
				#else
				float4 staticSwitch70 = _main_color_a;
				#endif
				float2 appendResult62 = (float2(_main_upanner , _main_vpanner));
				float2 appendResult49 = (float2(_normal_upanner , _normal_vpanner));
				float2 uv_normal_tex = IN.ase_texcoord3.xy * _normal_tex_ST.xy + _normal_tex_ST.zw;
				float2 CenteredUV15_g3 = ( uv_normal_tex - float2( 0.5,0.5 ) );
				float2 break17_g3 = CenteredUV15_g3;
				float2 appendResult23_g3 = (float2(( length( CenteredUV15_g3 ) * _normal_radial * 2.0 ) , ( atan2( break17_g3.x , break17_g3.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				#ifdef _USE_POLAR_ON
				float2 staticSwitch57 = appendResult23_g3;
				#else
				float2 staticSwitch57 = uv_normal_tex;
				#endif
				float2 panner53 = ( 1.0 * _Time.y * appendResult49 + staticSwitch57);
				float2 normalRef126 = ( (UnpackNormalScale( tex2D( _normal_tex, panner53 ), 1.0f )).xy * _normal_str );
				float2 uv_Main_Texture = IN.ase_texcoord3.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 panner59 = ( 1.0 * _Time.y * appendResult62 + ( normalRef126 + uv_Main_Texture ));
				float mainTexRef105 = tex2D( _Main_Texture, panner59 ).r;
				float vert_ref_w108 = IN.ase_texcoord4.z;
				#ifdef _USE_CUSTOMDATA_ON
				float staticSwitch21 = vert_ref_w108;
				#else
				float staticSwitch21 = _Main_Ins;
				#endif
				float vert_ref_t109 = IN.ase_texcoord4.w;
				#ifdef _USE_CUSTOMDATA_ON
				float staticSwitch23 = vert_ref_t109;
				#else
				float staticSwitch23 = _Dissolve;
				#endif
				float2 texCoord88 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult97 = (float2(_diss_normal_upanner , _diss_normal_vpanner));
				float2 texCoord90 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner94 = ( 1.0 * _Time.y * appendResult97 + texCoord90);
				float temp_output_98_0 = ( ( staticSwitch23 + ( 1.0 - texCoord88.y ) ) + ( (UnpackNormalScale( tex2D( _diss_normal_tex, panner94 ), 1.0f )).y * _diss_normal_power ) );
				float temp_output_103_0 = ( 1.0 - step( temp_output_98_0 , 0.48 ) );
				float4 dissolve_color134 = ( _edge_color * ( temp_output_103_0 - step( 0.48 , ( temp_output_98_0 - _edge_width ) ) ) );
				
				float vert_alpha120 = IN.ase_color.a;
				float2 uv_Dissolve_Texture = IN.ase_texcoord3.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw;
				float dissolve_1113 = saturate( ( tex2D( _Dissolve_Texture, uv_Dissolve_Texture ).r + staticSwitch23 ) );
				float dissolve_2114 = temp_output_103_0;
				#ifdef _USE_OTHER_DISS_ON
				float staticSwitch115 = dissolve_2114;
				#else
				float staticSwitch115 = dissolve_1113;
				#endif
				float temp_output_20_0 = ( mainTexRef105 * staticSwitch115 );
				#ifdef _USE_TOONDISSOLVE_ON
				float staticSwitch31 = saturate( step( 0.5 , temp_output_20_0 ) );
				#else
				float staticSwitch31 = temp_output_20_0;
				#endif
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth25 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth25 = abs( ( screenDepth25 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Ditance ) );
				float2 texCoord140 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_u_ref159 = IN.ase_texcoord4.x;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch145 = ( vert_u_ref159 + texCoord140.y );
				#else
				float staticSwitch145 = ( texCoord140.x + vert_u_ref159 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch153 = ( 1.0 - staticSwitch145 );
				#else
				float staticSwitch153 = staticSwitch145;
				#endif
				float mask_ref155 = step( 0.5 , staticSwitch153 );
				float2 texCoord169 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_v_ref161 = IN.ase_texcoord4.y;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch168 = ( vert_v_ref161 + texCoord169.y );
				#else
				float staticSwitch168 = ( texCoord169.x + vert_v_ref161 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch172 = staticSwitch168;
				#else
				float staticSwitch172 = ( 1.0 - staticSwitch168 );
				#endif
				float mask_minus_ref173 = step( 0.5 , staticSwitch172 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( vert_color119 * ( staticSwitch70 * saturate( ( saturate( pow( mainTexRef105 , _Main_Pow ) ) * staticSwitch21 ) ) ) ) + dissolve_color134 ).rgb;
				float Alpha = ( ( saturate( ( vert_alpha120 * saturate( ( staticSwitch31 * _Opacity ) ) ) ) * saturate( distanceDepth25 ) ) * mask_ref155 * mask_minus_ref173 );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.clipPos, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 140008
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma shader_feature_local _USE_TOONDISSOLVE_ON
			#pragma shader_feature_local _USE_POLAR_ON
			#pragma shader_feature_local _USE_OTHER_DISS_ON
			#pragma shader_feature_local _USE_CUSTOMDATA_ON
			#pragma shader_feature_local USE_MASK_ONE_MINUS_ON
			#pragma shader_feature_local USE_MASK_Y_DIR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _main_color_a;
			float4 _main_color_b;
			float4 _Dissolve_Texture_ST;
			float4 _normal_tex_ST;
			float4 _Main_Texture_ST;
			float4 _edge_color;
			float _edge_width;
			float _diss_normal_power;
			float _diss_normal_vpanner;
			float _diss_normal_upanner;
			float _Dissolve;
			float _Main_Pow;
			float _Opacity;
			float _normal_str;
			float _normal_radial;
			float _normal_vpanner;
			float _normal_upanner;
			float _main_vpanner;
			float _main_upanner;
			float _Main_Ins;
			float _Fade_Ditance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_Texture;
			sampler2D _normal_tex;
			sampler2D _Dissolve_Texture;
			sampler2D _diss_normal_tex;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_texcoord1 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float vert_alpha120 = IN.ase_color.a;
				float2 appendResult62 = (float2(_main_upanner , _main_vpanner));
				float2 appendResult49 = (float2(_normal_upanner , _normal_vpanner));
				float2 uv_normal_tex = IN.ase_texcoord.xy * _normal_tex_ST.xy + _normal_tex_ST.zw;
				float2 CenteredUV15_g3 = ( uv_normal_tex - float2( 0.5,0.5 ) );
				float2 break17_g3 = CenteredUV15_g3;
				float2 appendResult23_g3 = (float2(( length( CenteredUV15_g3 ) * _normal_radial * 2.0 ) , ( atan2( break17_g3.x , break17_g3.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				#ifdef _USE_POLAR_ON
				float2 staticSwitch57 = appendResult23_g3;
				#else
				float2 staticSwitch57 = uv_normal_tex;
				#endif
				float2 panner53 = ( 1.0 * _Time.y * appendResult49 + staticSwitch57);
				float2 normalRef126 = ( (UnpackNormalScale( tex2D( _normal_tex, panner53 ), 1.0f )).xy * _normal_str );
				float2 uv_Main_Texture = IN.ase_texcoord.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 panner59 = ( 1.0 * _Time.y * appendResult62 + ( normalRef126 + uv_Main_Texture ));
				float mainTexRef105 = tex2D( _Main_Texture, panner59 ).r;
				float2 uv_Dissolve_Texture = IN.ase_texcoord.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw;
				float vert_ref_t109 = IN.ase_texcoord1.w;
				#ifdef _USE_CUSTOMDATA_ON
				float staticSwitch23 = vert_ref_t109;
				#else
				float staticSwitch23 = _Dissolve;
				#endif
				float dissolve_1113 = saturate( ( tex2D( _Dissolve_Texture, uv_Dissolve_Texture ).r + staticSwitch23 ) );
				float2 texCoord88 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult97 = (float2(_diss_normal_upanner , _diss_normal_vpanner));
				float2 texCoord90 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner94 = ( 1.0 * _Time.y * appendResult97 + texCoord90);
				float temp_output_98_0 = ( ( staticSwitch23 + ( 1.0 - texCoord88.y ) ) + ( (UnpackNormalScale( tex2D( _diss_normal_tex, panner94 ), 1.0f )).y * _diss_normal_power ) );
				float temp_output_103_0 = ( 1.0 - step( temp_output_98_0 , 0.48 ) );
				float dissolve_2114 = temp_output_103_0;
				#ifdef _USE_OTHER_DISS_ON
				float staticSwitch115 = dissolve_2114;
				#else
				float staticSwitch115 = dissolve_1113;
				#endif
				float temp_output_20_0 = ( mainTexRef105 * staticSwitch115 );
				#ifdef _USE_TOONDISSOLVE_ON
				float staticSwitch31 = saturate( step( 0.5 , temp_output_20_0 ) );
				#else
				float staticSwitch31 = temp_output_20_0;
				#endif
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth25 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth25 = abs( ( screenDepth25 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Ditance ) );
				float2 texCoord140 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_u_ref159 = IN.ase_texcoord1.x;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch145 = ( vert_u_ref159 + texCoord140.y );
				#else
				float staticSwitch145 = ( texCoord140.x + vert_u_ref159 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch153 = ( 1.0 - staticSwitch145 );
				#else
				float staticSwitch153 = staticSwitch145;
				#endif
				float mask_ref155 = step( 0.5 , staticSwitch153 );
				float2 texCoord169 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_v_ref161 = IN.ase_texcoord1.y;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch168 = ( vert_v_ref161 + texCoord169.y );
				#else
				float staticSwitch168 = ( texCoord169.x + vert_v_ref161 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch172 = staticSwitch168;
				#else
				float staticSwitch172 = ( 1.0 - staticSwitch168 );
				#endif
				float mask_minus_ref173 = step( 0.5 , staticSwitch172 );
				

				surfaceDescription.Alpha = ( ( saturate( ( vert_alpha120 * saturate( ( staticSwitch31 * _Opacity ) ) ) ) * saturate( distanceDepth25 ) ) * mask_ref155 * mask_minus_ref173 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 140008
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma shader_feature_local _USE_TOONDISSOLVE_ON
			#pragma shader_feature_local _USE_POLAR_ON
			#pragma shader_feature_local _USE_OTHER_DISS_ON
			#pragma shader_feature_local _USE_CUSTOMDATA_ON
			#pragma shader_feature_local USE_MASK_ONE_MINUS_ON
			#pragma shader_feature_local USE_MASK_Y_DIR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _main_color_a;
			float4 _main_color_b;
			float4 _Dissolve_Texture_ST;
			float4 _normal_tex_ST;
			float4 _Main_Texture_ST;
			float4 _edge_color;
			float _edge_width;
			float _diss_normal_power;
			float _diss_normal_vpanner;
			float _diss_normal_upanner;
			float _Dissolve;
			float _Main_Pow;
			float _Opacity;
			float _normal_str;
			float _normal_radial;
			float _normal_vpanner;
			float _normal_upanner;
			float _main_vpanner;
			float _main_upanner;
			float _Main_Ins;
			float _Fade_Ditance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_Texture;
			sampler2D _normal_tex;
			sampler2D _Dissolve_Texture;
			sampler2D _diss_normal_tex;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float4 _SelectionID;


			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_texcoord1 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float vert_alpha120 = IN.ase_color.a;
				float2 appendResult62 = (float2(_main_upanner , _main_vpanner));
				float2 appendResult49 = (float2(_normal_upanner , _normal_vpanner));
				float2 uv_normal_tex = IN.ase_texcoord.xy * _normal_tex_ST.xy + _normal_tex_ST.zw;
				float2 CenteredUV15_g3 = ( uv_normal_tex - float2( 0.5,0.5 ) );
				float2 break17_g3 = CenteredUV15_g3;
				float2 appendResult23_g3 = (float2(( length( CenteredUV15_g3 ) * _normal_radial * 2.0 ) , ( atan2( break17_g3.x , break17_g3.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				#ifdef _USE_POLAR_ON
				float2 staticSwitch57 = appendResult23_g3;
				#else
				float2 staticSwitch57 = uv_normal_tex;
				#endif
				float2 panner53 = ( 1.0 * _Time.y * appendResult49 + staticSwitch57);
				float2 normalRef126 = ( (UnpackNormalScale( tex2D( _normal_tex, panner53 ), 1.0f )).xy * _normal_str );
				float2 uv_Main_Texture = IN.ase_texcoord.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 panner59 = ( 1.0 * _Time.y * appendResult62 + ( normalRef126 + uv_Main_Texture ));
				float mainTexRef105 = tex2D( _Main_Texture, panner59 ).r;
				float2 uv_Dissolve_Texture = IN.ase_texcoord.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw;
				float vert_ref_t109 = IN.ase_texcoord1.w;
				#ifdef _USE_CUSTOMDATA_ON
				float staticSwitch23 = vert_ref_t109;
				#else
				float staticSwitch23 = _Dissolve;
				#endif
				float dissolve_1113 = saturate( ( tex2D( _Dissolve_Texture, uv_Dissolve_Texture ).r + staticSwitch23 ) );
				float2 texCoord88 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult97 = (float2(_diss_normal_upanner , _diss_normal_vpanner));
				float2 texCoord90 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner94 = ( 1.0 * _Time.y * appendResult97 + texCoord90);
				float temp_output_98_0 = ( ( staticSwitch23 + ( 1.0 - texCoord88.y ) ) + ( (UnpackNormalScale( tex2D( _diss_normal_tex, panner94 ), 1.0f )).y * _diss_normal_power ) );
				float temp_output_103_0 = ( 1.0 - step( temp_output_98_0 , 0.48 ) );
				float dissolve_2114 = temp_output_103_0;
				#ifdef _USE_OTHER_DISS_ON
				float staticSwitch115 = dissolve_2114;
				#else
				float staticSwitch115 = dissolve_1113;
				#endif
				float temp_output_20_0 = ( mainTexRef105 * staticSwitch115 );
				#ifdef _USE_TOONDISSOLVE_ON
				float staticSwitch31 = saturate( step( 0.5 , temp_output_20_0 ) );
				#else
				float staticSwitch31 = temp_output_20_0;
				#endif
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth25 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth25 = abs( ( screenDepth25 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Ditance ) );
				float2 texCoord140 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_u_ref159 = IN.ase_texcoord1.x;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch145 = ( vert_u_ref159 + texCoord140.y );
				#else
				float staticSwitch145 = ( texCoord140.x + vert_u_ref159 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch153 = ( 1.0 - staticSwitch145 );
				#else
				float staticSwitch153 = staticSwitch145;
				#endif
				float mask_ref155 = step( 0.5 , staticSwitch153 );
				float2 texCoord169 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_v_ref161 = IN.ase_texcoord1.y;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch168 = ( vert_v_ref161 + texCoord169.y );
				#else
				float staticSwitch168 = ( texCoord169.x + vert_v_ref161 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch172 = staticSwitch168;
				#else
				float staticSwitch172 = ( 1.0 - staticSwitch168 );
				#endif
				float mask_minus_ref173 = step( 0.5 , staticSwitch172 );
				

				surfaceDescription.Alpha = ( ( saturate( ( vert_alpha120 * saturate( ( staticSwitch31 * _Opacity ) ) ) ) * saturate( distanceDepth25 ) ) * mask_ref155 * mask_minus_ref173 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 140008
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#pragma shader_feature_local _USE_TOONDISSOLVE_ON
			#pragma shader_feature_local _USE_POLAR_ON
			#pragma shader_feature_local _USE_OTHER_DISS_ON
			#pragma shader_feature_local _USE_CUSTOMDATA_ON
			#pragma shader_feature_local USE_MASK_ONE_MINUS_ON
			#pragma shader_feature_local USE_MASK_Y_DIR_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _main_color_a;
			float4 _main_color_b;
			float4 _Dissolve_Texture_ST;
			float4 _normal_tex_ST;
			float4 _Main_Texture_ST;
			float4 _edge_color;
			float _edge_width;
			float _diss_normal_power;
			float _diss_normal_vpanner;
			float _diss_normal_upanner;
			float _Dissolve;
			float _Main_Pow;
			float _Opacity;
			float _normal_str;
			float _normal_radial;
			float _normal_vpanner;
			float _normal_upanner;
			float _main_vpanner;
			float _main_upanner;
			float _Main_Ins;
			float _Fade_Ditance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_Texture;
			sampler2D _normal_tex;
			sampler2D _Dissolve_Texture;
			sampler2D _diss_normal_tex;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

				o.clipPos = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void frag( VertexOutput IN
				, out half4 outNormalWS : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
				 )
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float vert_alpha120 = IN.ase_color.a;
				float2 appendResult62 = (float2(_main_upanner , _main_vpanner));
				float2 appendResult49 = (float2(_normal_upanner , _normal_vpanner));
				float2 uv_normal_tex = IN.ase_texcoord1.xy * _normal_tex_ST.xy + _normal_tex_ST.zw;
				float2 CenteredUV15_g3 = ( uv_normal_tex - float2( 0.5,0.5 ) );
				float2 break17_g3 = CenteredUV15_g3;
				float2 appendResult23_g3 = (float2(( length( CenteredUV15_g3 ) * _normal_radial * 2.0 ) , ( atan2( break17_g3.x , break17_g3.y ) * ( 1.0 / TWO_PI ) * 1.0 )));
				#ifdef _USE_POLAR_ON
				float2 staticSwitch57 = appendResult23_g3;
				#else
				float2 staticSwitch57 = uv_normal_tex;
				#endif
				float2 panner53 = ( 1.0 * _Time.y * appendResult49 + staticSwitch57);
				float2 normalRef126 = ( (UnpackNormalScale( tex2D( _normal_tex, panner53 ), 1.0f )).xy * _normal_str );
				float2 uv_Main_Texture = IN.ase_texcoord1.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 panner59 = ( 1.0 * _Time.y * appendResult62 + ( normalRef126 + uv_Main_Texture ));
				float mainTexRef105 = tex2D( _Main_Texture, panner59 ).r;
				float2 uv_Dissolve_Texture = IN.ase_texcoord1.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw;
				float vert_ref_t109 = IN.ase_texcoord2.w;
				#ifdef _USE_CUSTOMDATA_ON
				float staticSwitch23 = vert_ref_t109;
				#else
				float staticSwitch23 = _Dissolve;
				#endif
				float dissolve_1113 = saturate( ( tex2D( _Dissolve_Texture, uv_Dissolve_Texture ).r + staticSwitch23 ) );
				float2 texCoord88 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult97 = (float2(_diss_normal_upanner , _diss_normal_vpanner));
				float2 texCoord90 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner94 = ( 1.0 * _Time.y * appendResult97 + texCoord90);
				float temp_output_98_0 = ( ( staticSwitch23 + ( 1.0 - texCoord88.y ) ) + ( (UnpackNormalScale( tex2D( _diss_normal_tex, panner94 ), 1.0f )).y * _diss_normal_power ) );
				float temp_output_103_0 = ( 1.0 - step( temp_output_98_0 , 0.48 ) );
				float dissolve_2114 = temp_output_103_0;
				#ifdef _USE_OTHER_DISS_ON
				float staticSwitch115 = dissolve_2114;
				#else
				float staticSwitch115 = dissolve_1113;
				#endif
				float temp_output_20_0 = ( mainTexRef105 * staticSwitch115 );
				#ifdef _USE_TOONDISSOLVE_ON
				float staticSwitch31 = saturate( step( 0.5 , temp_output_20_0 ) );
				#else
				float staticSwitch31 = temp_output_20_0;
				#endif
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth25 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth25 = abs( ( screenDepth25 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Ditance ) );
				float2 texCoord140 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_u_ref159 = IN.ase_texcoord2.x;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch145 = ( vert_u_ref159 + texCoord140.y );
				#else
				float staticSwitch145 = ( texCoord140.x + vert_u_ref159 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch153 = ( 1.0 - staticSwitch145 );
				#else
				float staticSwitch153 = staticSwitch145;
				#endif
				float mask_ref155 = step( 0.5 , staticSwitch153 );
				float2 texCoord169 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float vert_v_ref161 = IN.ase_texcoord2.y;
				#ifdef USE_MASK_Y_DIR_ON
				float staticSwitch168 = ( vert_v_ref161 + texCoord169.y );
				#else
				float staticSwitch168 = ( texCoord169.x + vert_v_ref161 );
				#endif
				#ifdef USE_MASK_ONE_MINUS_ON
				float staticSwitch172 = staticSwitch168;
				#else
				float staticSwitch172 = ( 1.0 - staticSwitch168 );
				#endif
				float mask_minus_ref173 = step( 0.5 , staticSwitch172 );
				

				surfaceDescription.Alpha = ( ( saturate( ( vert_alpha120 * saturate( ( staticSwitch31 * _Opacity ) ) ) ) * saturate( distanceDepth25 ) ) * mask_ref155 * mask_minus_ref173 );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(IN.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = IN.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;174;-1258.758,1924.083;Inherit;False;1888.596;628.1702;;22;148;150;160;144;152;143;145;165;167;168;140;169;162;166;151;170;154;153;171;172;155;173;Mask;0.8665271,0.9716981,0.371262,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;128;-1588.988,-939.9872;Inherit;False;2413.769;940.7214;;27;127;10;60;61;62;59;56;9;121;64;66;65;70;7;110;67;74;21;5;3;71;107;2;105;4;1;135;Main;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;125;-1446.245,75.85016;Inherit;False;2286.741;567.9067;;23;27;122;11;14;117;116;20;106;115;29;30;123;31;72;12;13;124;73;25;26;28;156;175;Alpha;0.9768651,1,0.3349057,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;118;-1439.899,758.0268;Inherit;False;2463.757;1033.936;;32;101;130;98;131;132;114;103;102;100;87;89;88;99;93;97;94;90;91;96;95;113;18;16;19;15;111;23;17;134;138;137;139;Dissolve;0.0518868,0.2646385,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;112;1343.598,-477.1847;Inherit;False;1334.059;297.5014;;8;120;119;8;159;109;108;24;161;Vertex Data;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;58;-3658.648,-774.1893;Inherit;False;1941.987;641.3805;;14;54;55;44;43;53;57;49;50;42;51;52;48;47;126;Normal;0,1,0.4410219,1;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;32;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;34;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;35;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;36;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;37;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;2;5;False;;10;False;;2;5;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;38;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;39;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;40;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;41;2291,-510;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3602.013,-306.3752;Inherit;False;Property;_normal_upanner;normal_upanner;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3599.013,-235.3748;Inherit;False;Property;_normal_vpanner;normal_vpanner;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3590.835,-393.2982;Inherit;False;Constant;_normal_length;normal_length;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-3582.835,-462.2983;Inherit;False;Property;_normal_radial;normal_radial;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;50;-3358.836,-506.2982;Inherit;False;Polar Coordinates;-1;;3;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;49;-3356.013,-301.3751;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;57;-3083.244,-725.0526;Inherit;False;Property;_use_polar;use_polar;23;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;53;-2820.013,-326.3752;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;43;-2660.333,-356.4933;Inherit;True;Property;_normal_tex;normal_tex;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;44;-2370.833,-357.3932;Inherit;False;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2371.611,-279.6722;Inherit;False;Property;_normal_str;normal_str;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2083.611,-353.6723;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1389.899,1019.011;Inherit;False;Property;_Dissolve;Dissolve;9;0;Create;True;0;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;23;-1084.175,1019.414;Inherit;True;Property;_Use_Customdata;Use_Customdata;19;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-1281.32,1085.649;Inherit;False;109;vert_ref_t;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-1123.583,808.0268;Inherit;True;Property;_Dissolve_Texture;Dissolve_Texture;8;0;Create;True;0;0;0;False;0;False;-1;None;145143eb93006454baed654b5669d0c3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-758.4188,836.0218;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;18;-649.5543,837.063;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-510.3392,830.596;Inherit;False;dissolve_1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-1341.21,1600.162;Inherit;False;Property;_diss_normal_upanner;diss_normal_upanner;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1343.21,1670.162;Inherit;False;Property;_diss_normal_vpanner;diss_normal_vpanner;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;-758.2101,1452.163;Inherit;True;Property;_diss_normal_tex;diss_normal_tex;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;90;-1213.21,1476.163;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;94;-959.2101,1481.163;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;97;-1122.21,1604.162;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;93;-471.2103,1452.163;Inherit;True;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-194.2101,1454.163;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;88;-1211.074,1357.231;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;89;-1004.422,1405.538;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-755.6986,1237.29;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-500.2103,1638.162;Inherit;False;Property;_diss_normal_power;diss_normal_power;20;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;678.4963,133.7092;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;14;477.2126,132.1439;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;-1396.245,327.8454;Inherit;False;114;dissolve_2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-957.487,194.2103;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;115;-1212.224,259.6642;Inherit;False;Property;_use_other_diss;use_other_diss;24;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;29;-467.0208,434.842;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-600.7322,431.4056;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;123;-773.2899,433.024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-468.8702,527.7568;Inherit;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;0;False;0;False;1;2.38;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;124;15.93601,470.3066;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;73;216.4157,195.8707;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;25;256.9944,286.9999;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;26;482.5041,283.45;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;106.504,305.4499;Inherit;False;Property;_Fade_Ditance;Fade_Ditance;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-782.584,-466.9095;Inherit;True;Property;_Main_Texture;Main_Texture;0;0;Create;True;0;0;0;False;0;False;-1;07454a717364c364b9bc4becd33778da;870ae2d60d57cd04fba95274a1472f67;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-759.584,-282.9095;Inherit;False;Property;_Main_Pow;Main_Pow;3;0;Create;True;0;0;0;False;0;False;1.952941;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;2;-303.7198,-436.5569;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;107;-358.6768,-284.6322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-169.2451,-436.2943;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;2.280277,-437.5569;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-755.7198,-207.557;Inherit;False;Property;_Main_Ins;Main_Ins;4;0;Create;True;0;0;0;False;0;False;1;2.37;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;21;-432.6608,-206.3352;Inherit;False;Property;_Use_Customdata;Use_Customdata;7;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;74;124.0218,-438.3029;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;471.525,-460.6469;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-650.9861,-127.8332;Inherit;False;108;vert_ref_w;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-501.6769,-889.9874;Inherit;False;Property;_main_color_a;main_color_a;6;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;14.92853,6.368605,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;70;14.10069,-888.191;Inherit;False;Property;_Use_lerp;Use_lerp;22;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;65;-500.1015,-727.5577;Inherit;False;Property;_main_color_b;main_color_b;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;64;-204.1013,-588.9581;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;474.3683,-530.9991;Inherit;False;119;vert_color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;662.7802,-484.657;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;31;-188.5323,192.9057;Inherit;False;Property;_Use_ToonDissolve;Use_ToonDissolve;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;-1140.913,-226.0804;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1312.513,-170.3803;Inherit;False;Property;_main_vpanner;main_vpanner;2;0;Create;True;0;0;0;False;0;False;0;0.66;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1312.113,-241.0802;Inherit;False;Property;_main_upanner;main_upanner;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;59;-960.073,-437.5432;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-1955.944,-354.7365;Inherit;False;normalRef;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;127;-1346.097,-444.2928;Inherit;False;126;normalRef;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1139.78,-440.2825;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-507.8015,-564.2581;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;122;59.07833,125.8502;Inherit;False;120;vert_alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;357.571,132.3746;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;-358.4566,435.209;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;98.75429,197.3237;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-1160.699,189.0311;Inherit;False;105;mainTexRef;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-498.0145,-443.0383;Inherit;False;mainTexRef;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;506.6936,1240.317;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;642.7347,1236.024;Inherit;False;dissolve_2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;794.361,-482.3582;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;608.8733,-387.1522;Inherit;False;134;dissolve_color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;678.9167,1095.764;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;0.7900238,1233.163;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;1.220337,1320.505;Inherit;False;Constant;_Float4;Float 4;26;0;Create;True;0;0;0;False;0;False;0.48;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;139;136.6969,1161.77;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-30.85999,1135.006;Inherit;False;Property;_edge_width;edge_width;25;0;Create;True;0;0;0;False;0;False;0.06;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;858.8054,1090.497;Inherit;False;dissolve_color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;132;677.7838,973.7841;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;101;310.1201,1240.805;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;131;313.3838,1134.885;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;138;289.9167,964.7635;Inherit;False;Property;_edge_color;edge_color;26;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;116;-1396.245,260.5942;Inherit;False;113;dissolve_1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1326.583,831.0268;Inherit;False;0;15;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-3632.598,-724.8463;Inherit;True;0;43;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1390.398,-371.1333;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;33;1085.959,-478.2495;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;1_fx/alphablended_mask;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;2;True;12;all;0;True;True;2;5;False;;10;False;;2;5;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;1;638225913306764441;  Blend;0;0;Two Sided;0;638488295391129580;Forward Only;0;0;Cast Shadows;0;638386622455905752;  Use Shadow Threshold;0;0;Receive Shadows;0;638386622462027313;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;False;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.VertexColorNode;8;2267.977,-394.4091;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;2447.821,-399.8992;Inherit;False;vert_color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;2448.821,-326.8993;Inherit;False;vert_alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;24;1393.598,-426.1846;Inherit;True;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;1676.422,-194.1945;Inherit;False;vert_ref_t;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;1673.422,-271.1946;Inherit;False;vert_ref_w;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;1680.161,-349.8716;Inherit;False;vert_v_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;1679.18,-414.4341;Inherit;False;vert_u_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;148;-988.8402,2067.124;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;150;-988.8402,2187.124;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;-963.6107,2111.211;Inherit;False;159;vert_u_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-761.8402,2136.124;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-525.4507,1974.083;Inherit;False;Constant;_Float1;Float 1;30;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-760.8402,2051.124;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;-757.668,2417.253;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;167;-756.668,2332.253;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;140;-1208.758,2061.591;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;169;-1149.769,2382.022;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;162;-924.3387,2398.362;Inherit;False;161;vert_v_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;166;-521.2785,2255.212;Inherit;False;Constant;_Float5;Float 1;30;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;151;149.5493,1983.083;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;170;186.2832,2269.317;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;154;-321.4507,2146.083;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;153;-159.4507,2099.083;Inherit;False;Property;use_mask_one_minus;use_mask_one_minus;29;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;171;-332.9654,2440.046;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;172;-155.4236,2344.298;Inherit;False;Property;use_mask_one_minus;use_mask_one_minus;30;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;374.8748,1977.19;Inherit;False;mask_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;399.8387,2265.305;Inherit;False;mask_minus_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;676.754,265.4233;Inherit;False;155;mask_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;643.7249,333.0243;Inherit;False;173;mask_minus_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;897.754,243.4233;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;145;-610.8402,2049.124;Inherit;False;Property;use_mask_y_dir;use_mask_y_dir;28;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;168;-606.668,2330.253;Inherit;False;Property;use_mask_y_dir;use_mask_y_dir;27;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
WireConnection;50;1;42;0
WireConnection;50;3;51;0
WireConnection;50;4;52;0
WireConnection;49;0;47;0
WireConnection;49;1;48;0
WireConnection;57;1;42;0
WireConnection;57;0;50;0
WireConnection;53;0;57;0
WireConnection;53;2;49;0
WireConnection;43;1;53;0
WireConnection;44;0;43;0
WireConnection;54;0;44;0
WireConnection;54;1;55;0
WireConnection;23;1;17;0
WireConnection;23;0;111;0
WireConnection;15;1;19;0
WireConnection;16;0;15;1
WireConnection;16;1;23;0
WireConnection;18;0;16;0
WireConnection;113;0;18;0
WireConnection;91;1;94;0
WireConnection;94;0;90;0
WireConnection;94;2;97;0
WireConnection;97;0;95;0
WireConnection;97;1;96;0
WireConnection;93;0;91;0
WireConnection;99;0;93;0
WireConnection;99;1;100;0
WireConnection;89;0;88;2
WireConnection;87;0;23;0
WireConnection;87;1;89;0
WireConnection;27;0;14;0
WireConnection;27;1;26;0
WireConnection;14;0;11;0
WireConnection;20;0;106;0
WireConnection;20;1;115;0
WireConnection;115;1;116;0
WireConnection;115;0;117;0
WireConnection;29;0;30;0
WireConnection;29;1;123;0
WireConnection;123;0;20;0
WireConnection;124;0;13;0
WireConnection;73;0;12;0
WireConnection;25;0;28;0
WireConnection;26;0;25;0
WireConnection;1;1;59;0
WireConnection;2;0;105;0
WireConnection;2;1;107;0
WireConnection;107;0;4;0
WireConnection;71;0;2;0
WireConnection;3;0;71;0
WireConnection;3;1;21;0
WireConnection;21;1;5;0
WireConnection;21;0;110;0
WireConnection;74;0;3;0
WireConnection;67;0;70;0
WireConnection;67;1;74;0
WireConnection;70;1;7;0
WireConnection;70;0;64;0
WireConnection;64;0;7;0
WireConnection;64;1;65;0
WireConnection;64;2;66;1
WireConnection;9;0;121;0
WireConnection;9;1;67;0
WireConnection;31;1;20;0
WireConnection;31;0;72;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;59;0;56;0
WireConnection;59;2;62;0
WireConnection;126;0;54;0
WireConnection;56;0;127;0
WireConnection;56;1;10;0
WireConnection;11;0;122;0
WireConnection;11;1;73;0
WireConnection;72;0;29;0
WireConnection;12;0;31;0
WireConnection;12;1;124;0
WireConnection;105;0;1;1
WireConnection;103;0;101;0
WireConnection;114;0;103;0
WireConnection;136;0;9;0
WireConnection;136;1;135;0
WireConnection;137;0;138;0
WireConnection;137;1;132;0
WireConnection;98;0;87;0
WireConnection;98;1;99;0
WireConnection;139;0;98;0
WireConnection;139;1;130;0
WireConnection;134;0;137;0
WireConnection;132;0;103;0
WireConnection;132;1;131;0
WireConnection;101;0;98;0
WireConnection;101;1;102;0
WireConnection;131;0;102;0
WireConnection;131;1;139;0
WireConnection;33;2;136;0
WireConnection;33;3;157;0
WireConnection;119;0;8;0
WireConnection;120;0;8;4
WireConnection;109;0;24;4
WireConnection;108;0;24;3
WireConnection;161;0;24;2
WireConnection;159;0;24;1
WireConnection;148;0;140;1
WireConnection;150;0;140;2
WireConnection;144;0;160;0
WireConnection;144;1;150;0
WireConnection;143;0;148;0
WireConnection;143;1;160;0
WireConnection;165;0;162;0
WireConnection;165;1;169;2
WireConnection;167;0;169;1
WireConnection;167;1;162;0
WireConnection;151;0;152;0
WireConnection;151;1;153;0
WireConnection;170;0;166;0
WireConnection;170;1;172;0
WireConnection;154;0;145;0
WireConnection;153;1;145;0
WireConnection;153;0;154;0
WireConnection;171;0;168;0
WireConnection;172;1;171;0
WireConnection;172;0;168;0
WireConnection;155;0;151;0
WireConnection;173;0;170;0
WireConnection;157;0;27;0
WireConnection;157;1;156;0
WireConnection;157;2;175;0
WireConnection;145;1;143;0
WireConnection;145;0;144;0
WireConnection;168;1;167;0
WireConnection;168;0;165;0
ASEEND*/
//CHKSM=E73B5CDA8B2DF463C99F7B104B71C1F338133330