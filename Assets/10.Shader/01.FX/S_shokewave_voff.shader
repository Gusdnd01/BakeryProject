// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "1_fx/shokewave_voff"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 0
		[HDR]_TintColor("Tint Color", Color) = (1,1,1,0)
		_Main_Texture("Main_Texture", 2D) = "white" {}
		_main_step("main_step", Float) = 0.5
		_main_scale("main_scale", Vector) = (1,1,0,0)
		_Main_UPanner("Main_UPanner", Float) = 0
		_Main_Ins("Main_Ins", Range( 1 , 10)) = 1
		_Main_Pow("Main_Pow", Range( 0 , 10)) = 1
		_Opacity("Opacity", Range( 0 , 10)) = 1
		_Fade_Distance("Fade_Distance", Float) = 0
		_Noise_Texture("Noise_Texture", 2D) = "bump" {}
		_Noise_Str("Noise_Str", Range( 0 , 0.3)) = 0.06793601
		_Noise_Tex_UPanner("Noise_Tex_UPanner", Float) = 0
		_Noise_Tex_VPanner("Noise_Tex_VPanner", Float) = 0
		_Diss_Texture("Diss_Texture", 2D) = "white" {}
		_Diss_Tex_UPanner("Diss_Tex_UPanner", Float) = 0
		_Diss_Tex_VPanner("Diss_Tex_VPanner", Float) = 0
		_Dissolve("Dissolve", Range( -1 , 1)) = 0
		[Toggle(USE_CUSTOM_ON)] Use_Custom("Use_Custom", Float) = 0
		_Sub_Texture("Sub_Texture", 2D) = "white" {}
		[HDR]_SubTex_Color("SubTex_Color", Color) = (0,0,0,0)
		_SubTex_Ins("SubTex_Ins", Range( 0 , 10)) = 0
		_SubTex_Pow("SubTex_Pow", Range( 1 , 10)) = 0
		_SubTex_UPanner("SubTex_UPanner", Float) = 0
		_SubTex_VPanner("SubTex_VPanner", Float) = 0
		_Mask_Texture("Mask_Texture", 2D) = "white" {}
		_Mask_Pow("Mask_Pow", Range( 1 , 50)) = 5.346326
		_Ceil("Ceil", Float) = 0
		[Toggle(_USE_MASKTEX_ON)] _Use_MaskTex("Use_MaskTex", Float) = 0
		[Toggle(_MASK_1X_ON)] _Mask_1x("Mask_-1x", Float) = 0
		[Toggle(_MASK_MIRROR_ON)] _Mask_Mirror("Mask_Mirror", Float) = 1
		[Toggle(_USE_ADDMULITYMASK_ON)] _Use_AddMulityMask("Use_AddMulityMask", Float) = 0
		[Toggle(_USE_CEIL_ON)] _Use_Ceil("Use_Ceil", Float) = 0
		[ASEEnd][Toggle(_USE_MAIN_STEP_ON)] _Use_main_step("Use_main_step", Float) = 0


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

		Cull [_CullMode]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.5
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

			Blend [_Src] [_Dst], [_Src] [_Dst]
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
			#pragma shader_feature_local _USE_CEIL_ON
			#pragma shader_feature_local _USE_MAIN_STEP_ON
			#pragma shader_feature_local USE_CUSTOM_ON
			#pragma shader_feature_local _USE_ADDMULITYMASK_ON
			#pragma shader_feature_local _USE_MASKTEX_ON
			#pragma shader_feature_local _MASK_MIRROR_ON
			#pragma shader_feature_local _MASK_1X_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
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
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Diss_Texture_ST;
			float4 _Sub_Texture_ST;
			float4 _SubTex_Color;
			float4 _TintColor;
			float4 _Main_Texture_ST;
			float4 _Noise_Texture_ST;
			float2 _main_scale;
			float _Src;
			float _Dissolve;
			float _Diss_Tex_VPanner;
			float _Diss_Tex_UPanner;
			float _Opacity;
			float _Main_Ins;
			float _Main_Pow;
			float _Ceil;
			float _Main_UPanner;
			float _Noise_Tex_UPanner;
			float _Noise_Tex_VPanner;
			float _Mask_Pow;
			float _main_step;
			float _SubTex_Ins;
			float _SubTex_Pow;
			float _SubTex_VPanner;
			float _SubTex_UPanner;
			float _CullMode;
			float _Dst;
			float _Noise_Str;
			float _Fade_Distance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Sub_Texture;
			sampler2D _Main_Texture;
			sampler2D _Noise_Texture;
			sampler2D _Diss_Texture;
			sampler2D _Mask_Texture;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord2;
				o.ase_texcoord5 = v.ase_texcoord1;
				
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
				float4 ase_texcoord2 : TEXCOORD2;
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
				o.ase_texcoord2 = v.ase_texcoord2;
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
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
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

				float4 vert_color_ref165 = IN.ase_color;
				float2 appendResult97 = (float2(_SubTex_UPanner , _SubTex_VPanner));
				float2 uv_Sub_Texture = IN.ase_texcoord3.xy * _Sub_Texture_ST.xy + _Sub_Texture_ST.zw;
				float2 panner92 = ( 1.0 * _Time.y * appendResult97 + uv_Sub_Texture);
				float2 uv_Main_Texture = IN.ase_texcoord3.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 uv_Noise_Texture = IN.ase_texcoord3.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
				float2 appendResult32 = (float2(_Noise_Tex_UPanner , _Noise_Tex_VPanner));
				float vert_uv2_u_ref144 = IN.ase_texcoord4.x;
				float vert_uv2_v_ref145 = IN.ase_texcoord4.y;
				float2 appendResult66 = (float2(vert_uv2_u_ref144 , vert_uv2_v_ref145));
				#ifdef USE_CUSTOM_ON
				float2 staticSwitch67 = appendResult66;
				#else
				float2 staticSwitch67 = _main_scale;
				#endif
				float vert__uv2_w_ref146 = IN.ase_texcoord4.z;
				float2 appendResult7 = (float2(_Main_UPanner , vert__uv2_w_ref146));
				float4 tex2DNode1 = tex2D( _Main_Texture, (( uv_Main_Texture + ( (UnpackNormalScale( tex2D( _Noise_Texture, (uv_Noise_Texture*1.0 + ( appendResult32 * _TimeParameters.x )) ), 1.0f )).xy * _Noise_Str ) )*staticSwitch67 + appendResult7) );
				#ifdef _USE_MAIN_STEP_ON
				float staticSwitch174 = tex2DNode1.r;
				#else
				float staticSwitch174 = step( _main_step , tex2DNode1.r );
				#endif
				float main_r_ref156 = staticSwitch174;
				float2 temp_cast_0 = (main_r_ref156).xx;
				float temp_output_2_0_g1 = _Ceil;
				#ifdef _USE_CEIL_ON
				float staticSwitch131 = (( ceil( ( temp_cast_0 * temp_output_2_0_g1 ) ) / temp_output_2_0_g1 )).x;
				#else
				float staticSwitch131 = main_r_ref156;
				#endif
				float vert_uv1_u_ref153 = IN.ase_texcoord5.x;
				#ifdef USE_CUSTOM_ON
				float staticSwitch61 = vert_uv1_u_ref153;
				#else
				float staticSwitch61 = _Main_Ins;
				#endif
				
				float vert_alpha_ref166 = IN.ase_color.a;
				float2 appendResult53 = (float2(_Diss_Tex_UPanner , _Diss_Tex_VPanner));
				float2 uv_Diss_Texture = IN.ase_texcoord3.xy * _Diss_Texture_ST.xy + _Diss_Texture_ST.zw;
				float2 panner54 = ( 1.0 * _Time.y * appendResult53 + uv_Diss_Texture);
				float vert_uv1_v_ref152 = IN.ase_texcoord5.y;
				#ifdef USE_CUSTOM_ON
				float staticSwitch62 = vert_uv1_v_ref152;
				#else
				float staticSwitch62 = _Dissolve;
				#endif
				float2 texCoord8 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float uv_v_ref136 = texCoord8.y;
				#ifdef _MASK_1X_ON
				float staticSwitch81 = ( 1.0 - uv_v_ref136 );
				#else
				float staticSwitch81 = uv_v_ref136;
				#endif
				#ifdef _MASK_MIRROR_ON
				float staticSwitch82 = ( saturate( ( uv_v_ref136 * ( 1.0 - uv_v_ref136 ) ) ) * 4.0 );
				#else
				float staticSwitch82 = staticSwitch81;
				#endif
				float mask_pow_ref141 = _Mask_Pow;
				float2 texCoord83 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_MASKTEX_ON
				float staticSwitch89 = saturate( pow( tex2D( _Mask_Texture, texCoord83 ).r , mask_pow_ref141 ) );
				#else
				float staticSwitch89 = saturate( pow( staticSwitch82 , mask_pow_ref141 ) );
				#endif
				float mask_ref139 = staticSwitch89;
				#ifdef _USE_ADDMULITYMASK_ON
				float staticSwitch109 = ( ( main_r_ref156 + mask_ref139 ) * mask_ref139 );
				#else
				float staticSwitch109 = ( main_r_ref156 * mask_ref139 );
				#endif
				float4 screenPos = IN.ase_texcoord6;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth69 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth69 = abs( ( screenDepth69 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Distance ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( vert_color_ref165 * ( ( ( saturate( pow( tex2D( _Sub_Texture, panner92 ).r , _SubTex_Pow ) ) * _SubTex_Ins ) * _SubTex_Color ) + ( _TintColor * ( pow( staticSwitch131 , _Main_Pow ) * staticSwitch61 ) ) ) ).rgb;
				float Alpha = ( vert_alpha_ref166 * ( saturate( ( _Opacity * ( saturate( ( tex2D( _Diss_Texture, panner54 ).r + staticSwitch62 ) ) * staticSwitch109 ) ) ) * saturate( distanceDepth69 ) ) );
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
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 140008
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#pragma shader_feature_local USE_CUSTOM_ON
			#pragma shader_feature_local _USE_ADDMULITYMASK_ON
			#pragma shader_feature_local _USE_MAIN_STEP_ON
			#pragma shader_feature_local _USE_MASKTEX_ON
			#pragma shader_feature_local _MASK_MIRROR_ON
			#pragma shader_feature_local _MASK_1X_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
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
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Diss_Texture_ST;
			float4 _Sub_Texture_ST;
			float4 _SubTex_Color;
			float4 _TintColor;
			float4 _Main_Texture_ST;
			float4 _Noise_Texture_ST;
			float2 _main_scale;
			float _Src;
			float _Dissolve;
			float _Diss_Tex_VPanner;
			float _Diss_Tex_UPanner;
			float _Opacity;
			float _Main_Ins;
			float _Main_Pow;
			float _Ceil;
			float _Main_UPanner;
			float _Noise_Tex_UPanner;
			float _Noise_Tex_VPanner;
			float _Mask_Pow;
			float _main_step;
			float _SubTex_Ins;
			float _SubTex_Pow;
			float _SubTex_VPanner;
			float _SubTex_UPanner;
			float _CullMode;
			float _Dst;
			float _Noise_Str;
			float _Fade_Distance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Diss_Texture;
			sampler2D _Main_Texture;
			sampler2D _Noise_Texture;
			sampler2D _Mask_Texture;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_texcoord4 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

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

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

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
				float4 ase_texcoord2 : TEXCOORD2;

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
				o.ase_texcoord2 = v.ase_texcoord2;
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
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
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

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
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

				float vert_alpha_ref166 = IN.ase_color.a;
				float2 appendResult53 = (float2(_Diss_Tex_UPanner , _Diss_Tex_VPanner));
				float2 uv_Diss_Texture = IN.ase_texcoord2.xy * _Diss_Texture_ST.xy + _Diss_Texture_ST.zw;
				float2 panner54 = ( 1.0 * _Time.y * appendResult53 + uv_Diss_Texture);
				float vert_uv1_v_ref152 = IN.ase_texcoord3.y;
				#ifdef USE_CUSTOM_ON
				float staticSwitch62 = vert_uv1_v_ref152;
				#else
				float staticSwitch62 = _Dissolve;
				#endif
				float2 uv_Main_Texture = IN.ase_texcoord2.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 uv_Noise_Texture = IN.ase_texcoord2.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
				float2 appendResult32 = (float2(_Noise_Tex_UPanner , _Noise_Tex_VPanner));
				float vert_uv2_u_ref144 = IN.ase_texcoord4.x;
				float vert_uv2_v_ref145 = IN.ase_texcoord4.y;
				float2 appendResult66 = (float2(vert_uv2_u_ref144 , vert_uv2_v_ref145));
				#ifdef USE_CUSTOM_ON
				float2 staticSwitch67 = appendResult66;
				#else
				float2 staticSwitch67 = _main_scale;
				#endif
				float vert__uv2_w_ref146 = IN.ase_texcoord4.z;
				float2 appendResult7 = (float2(_Main_UPanner , vert__uv2_w_ref146));
				float4 tex2DNode1 = tex2D( _Main_Texture, (( uv_Main_Texture + ( (UnpackNormalScale( tex2D( _Noise_Texture, (uv_Noise_Texture*1.0 + ( appendResult32 * _TimeParameters.x )) ), 1.0f )).xy * _Noise_Str ) )*staticSwitch67 + appendResult7) );
				#ifdef _USE_MAIN_STEP_ON
				float staticSwitch174 = tex2DNode1.r;
				#else
				float staticSwitch174 = step( _main_step , tex2DNode1.r );
				#endif
				float main_r_ref156 = staticSwitch174;
				float2 texCoord8 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float uv_v_ref136 = texCoord8.y;
				#ifdef _MASK_1X_ON
				float staticSwitch81 = ( 1.0 - uv_v_ref136 );
				#else
				float staticSwitch81 = uv_v_ref136;
				#endif
				#ifdef _MASK_MIRROR_ON
				float staticSwitch82 = ( saturate( ( uv_v_ref136 * ( 1.0 - uv_v_ref136 ) ) ) * 4.0 );
				#else
				float staticSwitch82 = staticSwitch81;
				#endif
				float mask_pow_ref141 = _Mask_Pow;
				float2 texCoord83 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_MASKTEX_ON
				float staticSwitch89 = saturate( pow( tex2D( _Mask_Texture, texCoord83 ).r , mask_pow_ref141 ) );
				#else
				float staticSwitch89 = saturate( pow( staticSwitch82 , mask_pow_ref141 ) );
				#endif
				float mask_ref139 = staticSwitch89;
				#ifdef _USE_ADDMULITYMASK_ON
				float staticSwitch109 = ( ( main_r_ref156 + mask_ref139 ) * mask_ref139 );
				#else
				float staticSwitch109 = ( main_r_ref156 * mask_ref139 );
				#endif
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth69 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth69 = abs( ( screenDepth69 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Distance ) );
				

				float Alpha = ( vert_alpha_ref166 * ( saturate( ( _Opacity * ( saturate( ( tex2D( _Diss_Texture, panner54 ).r + staticSwitch62 ) ) * staticSwitch109 ) ) ) * saturate( distanceDepth69 ) ) );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif
				return 0;
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

			#pragma shader_feature_local USE_CUSTOM_ON
			#pragma shader_feature_local _USE_ADDMULITYMASK_ON
			#pragma shader_feature_local _USE_MAIN_STEP_ON
			#pragma shader_feature_local _USE_MASKTEX_ON
			#pragma shader_feature_local _MASK_MIRROR_ON
			#pragma shader_feature_local _MASK_1X_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Diss_Texture_ST;
			float4 _Sub_Texture_ST;
			float4 _SubTex_Color;
			float4 _TintColor;
			float4 _Main_Texture_ST;
			float4 _Noise_Texture_ST;
			float2 _main_scale;
			float _Src;
			float _Dissolve;
			float _Diss_Tex_VPanner;
			float _Diss_Tex_UPanner;
			float _Opacity;
			float _Main_Ins;
			float _Main_Pow;
			float _Ceil;
			float _Main_UPanner;
			float _Noise_Tex_UPanner;
			float _Noise_Tex_VPanner;
			float _Mask_Pow;
			float _main_step;
			float _SubTex_Ins;
			float _SubTex_Pow;
			float _SubTex_VPanner;
			float _SubTex_UPanner;
			float _CullMode;
			float _Dst;
			float _Noise_Str;
			float _Fade_Distance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Diss_Texture;
			sampler2D _Main_Texture;
			sampler2D _Noise_Texture;
			sampler2D _Mask_Texture;
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
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				
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
				float4 ase_texcoord2 : TEXCOORD2;

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
				o.ase_texcoord2 = v.ase_texcoord2;
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
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
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

				float vert_alpha_ref166 = IN.ase_color.a;
				float2 appendResult53 = (float2(_Diss_Tex_UPanner , _Diss_Tex_VPanner));
				float2 uv_Diss_Texture = IN.ase_texcoord.xy * _Diss_Texture_ST.xy + _Diss_Texture_ST.zw;
				float2 panner54 = ( 1.0 * _Time.y * appendResult53 + uv_Diss_Texture);
				float vert_uv1_v_ref152 = IN.ase_texcoord1.y;
				#ifdef USE_CUSTOM_ON
				float staticSwitch62 = vert_uv1_v_ref152;
				#else
				float staticSwitch62 = _Dissolve;
				#endif
				float2 uv_Main_Texture = IN.ase_texcoord.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 uv_Noise_Texture = IN.ase_texcoord.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
				float2 appendResult32 = (float2(_Noise_Tex_UPanner , _Noise_Tex_VPanner));
				float vert_uv2_u_ref144 = IN.ase_texcoord2.x;
				float vert_uv2_v_ref145 = IN.ase_texcoord2.y;
				float2 appendResult66 = (float2(vert_uv2_u_ref144 , vert_uv2_v_ref145));
				#ifdef USE_CUSTOM_ON
				float2 staticSwitch67 = appendResult66;
				#else
				float2 staticSwitch67 = _main_scale;
				#endif
				float vert__uv2_w_ref146 = IN.ase_texcoord2.z;
				float2 appendResult7 = (float2(_Main_UPanner , vert__uv2_w_ref146));
				float4 tex2DNode1 = tex2D( _Main_Texture, (( uv_Main_Texture + ( (UnpackNormalScale( tex2D( _Noise_Texture, (uv_Noise_Texture*1.0 + ( appendResult32 * _TimeParameters.x )) ), 1.0f )).xy * _Noise_Str ) )*staticSwitch67 + appendResult7) );
				#ifdef _USE_MAIN_STEP_ON
				float staticSwitch174 = tex2DNode1.r;
				#else
				float staticSwitch174 = step( _main_step , tex2DNode1.r );
				#endif
				float main_r_ref156 = staticSwitch174;
				float2 texCoord8 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float uv_v_ref136 = texCoord8.y;
				#ifdef _MASK_1X_ON
				float staticSwitch81 = ( 1.0 - uv_v_ref136 );
				#else
				float staticSwitch81 = uv_v_ref136;
				#endif
				#ifdef _MASK_MIRROR_ON
				float staticSwitch82 = ( saturate( ( uv_v_ref136 * ( 1.0 - uv_v_ref136 ) ) ) * 4.0 );
				#else
				float staticSwitch82 = staticSwitch81;
				#endif
				float mask_pow_ref141 = _Mask_Pow;
				float2 texCoord83 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_MASKTEX_ON
				float staticSwitch89 = saturate( pow( tex2D( _Mask_Texture, texCoord83 ).r , mask_pow_ref141 ) );
				#else
				float staticSwitch89 = saturate( pow( staticSwitch82 , mask_pow_ref141 ) );
				#endif
				float mask_ref139 = staticSwitch89;
				#ifdef _USE_ADDMULITYMASK_ON
				float staticSwitch109 = ( ( main_r_ref156 + mask_ref139 ) * mask_ref139 );
				#else
				float staticSwitch109 = ( main_r_ref156 * mask_ref139 );
				#endif
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth69 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth69 = abs( ( screenDepth69 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Distance ) );
				

				surfaceDescription.Alpha = ( vert_alpha_ref166 * ( saturate( ( _Opacity * ( saturate( ( tex2D( _Diss_Texture, panner54 ).r + staticSwitch62 ) ) * staticSwitch109 ) ) ) * saturate( distanceDepth69 ) ) );
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

			#pragma shader_feature_local USE_CUSTOM_ON
			#pragma shader_feature_local _USE_ADDMULITYMASK_ON
			#pragma shader_feature_local _USE_MAIN_STEP_ON
			#pragma shader_feature_local _USE_MASKTEX_ON
			#pragma shader_feature_local _MASK_MIRROR_ON
			#pragma shader_feature_local _MASK_1X_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Diss_Texture_ST;
			float4 _Sub_Texture_ST;
			float4 _SubTex_Color;
			float4 _TintColor;
			float4 _Main_Texture_ST;
			float4 _Noise_Texture_ST;
			float2 _main_scale;
			float _Src;
			float _Dissolve;
			float _Diss_Tex_VPanner;
			float _Diss_Tex_UPanner;
			float _Opacity;
			float _Main_Ins;
			float _Main_Pow;
			float _Ceil;
			float _Main_UPanner;
			float _Noise_Tex_UPanner;
			float _Noise_Tex_VPanner;
			float _Mask_Pow;
			float _main_step;
			float _SubTex_Ins;
			float _SubTex_Pow;
			float _SubTex_VPanner;
			float _SubTex_UPanner;
			float _CullMode;
			float _Dst;
			float _Noise_Str;
			float _Fade_Distance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Diss_Texture;
			sampler2D _Main_Texture;
			sampler2D _Noise_Texture;
			sampler2D _Mask_Texture;
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
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				
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
				float4 ase_texcoord2 : TEXCOORD2;

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
				o.ase_texcoord2 = v.ase_texcoord2;
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
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
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

				float vert_alpha_ref166 = IN.ase_color.a;
				float2 appendResult53 = (float2(_Diss_Tex_UPanner , _Diss_Tex_VPanner));
				float2 uv_Diss_Texture = IN.ase_texcoord.xy * _Diss_Texture_ST.xy + _Diss_Texture_ST.zw;
				float2 panner54 = ( 1.0 * _Time.y * appendResult53 + uv_Diss_Texture);
				float vert_uv1_v_ref152 = IN.ase_texcoord1.y;
				#ifdef USE_CUSTOM_ON
				float staticSwitch62 = vert_uv1_v_ref152;
				#else
				float staticSwitch62 = _Dissolve;
				#endif
				float2 uv_Main_Texture = IN.ase_texcoord.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 uv_Noise_Texture = IN.ase_texcoord.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
				float2 appendResult32 = (float2(_Noise_Tex_UPanner , _Noise_Tex_VPanner));
				float vert_uv2_u_ref144 = IN.ase_texcoord2.x;
				float vert_uv2_v_ref145 = IN.ase_texcoord2.y;
				float2 appendResult66 = (float2(vert_uv2_u_ref144 , vert_uv2_v_ref145));
				#ifdef USE_CUSTOM_ON
				float2 staticSwitch67 = appendResult66;
				#else
				float2 staticSwitch67 = _main_scale;
				#endif
				float vert__uv2_w_ref146 = IN.ase_texcoord2.z;
				float2 appendResult7 = (float2(_Main_UPanner , vert__uv2_w_ref146));
				float4 tex2DNode1 = tex2D( _Main_Texture, (( uv_Main_Texture + ( (UnpackNormalScale( tex2D( _Noise_Texture, (uv_Noise_Texture*1.0 + ( appendResult32 * _TimeParameters.x )) ), 1.0f )).xy * _Noise_Str ) )*staticSwitch67 + appendResult7) );
				#ifdef _USE_MAIN_STEP_ON
				float staticSwitch174 = tex2DNode1.r;
				#else
				float staticSwitch174 = step( _main_step , tex2DNode1.r );
				#endif
				float main_r_ref156 = staticSwitch174;
				float2 texCoord8 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float uv_v_ref136 = texCoord8.y;
				#ifdef _MASK_1X_ON
				float staticSwitch81 = ( 1.0 - uv_v_ref136 );
				#else
				float staticSwitch81 = uv_v_ref136;
				#endif
				#ifdef _MASK_MIRROR_ON
				float staticSwitch82 = ( saturate( ( uv_v_ref136 * ( 1.0 - uv_v_ref136 ) ) ) * 4.0 );
				#else
				float staticSwitch82 = staticSwitch81;
				#endif
				float mask_pow_ref141 = _Mask_Pow;
				float2 texCoord83 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_MASKTEX_ON
				float staticSwitch89 = saturate( pow( tex2D( _Mask_Texture, texCoord83 ).r , mask_pow_ref141 ) );
				#else
				float staticSwitch89 = saturate( pow( staticSwitch82 , mask_pow_ref141 ) );
				#endif
				float mask_ref139 = staticSwitch89;
				#ifdef _USE_ADDMULITYMASK_ON
				float staticSwitch109 = ( ( main_r_ref156 + mask_ref139 ) * mask_ref139 );
				#else
				float staticSwitch109 = ( main_r_ref156 * mask_ref139 );
				#endif
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth69 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth69 = abs( ( screenDepth69 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Distance ) );
				

				surfaceDescription.Alpha = ( vert_alpha_ref166 * ( saturate( ( _Opacity * ( saturate( ( tex2D( _Diss_Texture, panner54 ).r + staticSwitch62 ) ) * staticSwitch109 ) ) ) * saturate( distanceDepth69 ) ) );
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

			#pragma shader_feature_local USE_CUSTOM_ON
			#pragma shader_feature_local _USE_ADDMULITYMASK_ON
			#pragma shader_feature_local _USE_MAIN_STEP_ON
			#pragma shader_feature_local _USE_MASKTEX_ON
			#pragma shader_feature_local _MASK_MIRROR_ON
			#pragma shader_feature_local _MASK_1X_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
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
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Diss_Texture_ST;
			float4 _Sub_Texture_ST;
			float4 _SubTex_Color;
			float4 _TintColor;
			float4 _Main_Texture_ST;
			float4 _Noise_Texture_ST;
			float2 _main_scale;
			float _Src;
			float _Dissolve;
			float _Diss_Tex_VPanner;
			float _Diss_Tex_UPanner;
			float _Opacity;
			float _Main_Ins;
			float _Main_Pow;
			float _Ceil;
			float _Main_UPanner;
			float _Noise_Tex_UPanner;
			float _Noise_Tex_VPanner;
			float _Mask_Pow;
			float _main_step;
			float _SubTex_Ins;
			float _SubTex_Pow;
			float _SubTex_VPanner;
			float _SubTex_UPanner;
			float _CullMode;
			float _Dst;
			float _Noise_Str;
			float _Fade_Distance;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Diss_Texture;
			sampler2D _Main_Texture;
			sampler2D _Noise_Texture;
			sampler2D _Mask_Texture;
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
				o.ase_texcoord4 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2 = v.ase_texcoord1;
				o.ase_texcoord3 = v.ase_texcoord2;
				
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
				float4 ase_texcoord2 : TEXCOORD2;

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
				o.ase_texcoord2 = v.ase_texcoord2;
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
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
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

				float vert_alpha_ref166 = IN.ase_color.a;
				float2 appendResult53 = (float2(_Diss_Tex_UPanner , _Diss_Tex_VPanner));
				float2 uv_Diss_Texture = IN.ase_texcoord1.xy * _Diss_Texture_ST.xy + _Diss_Texture_ST.zw;
				float2 panner54 = ( 1.0 * _Time.y * appendResult53 + uv_Diss_Texture);
				float vert_uv1_v_ref152 = IN.ase_texcoord2.y;
				#ifdef USE_CUSTOM_ON
				float staticSwitch62 = vert_uv1_v_ref152;
				#else
				float staticSwitch62 = _Dissolve;
				#endif
				float2 uv_Main_Texture = IN.ase_texcoord1.xy * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
				float2 uv_Noise_Texture = IN.ase_texcoord1.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
				float2 appendResult32 = (float2(_Noise_Tex_UPanner , _Noise_Tex_VPanner));
				float vert_uv2_u_ref144 = IN.ase_texcoord3.x;
				float vert_uv2_v_ref145 = IN.ase_texcoord3.y;
				float2 appendResult66 = (float2(vert_uv2_u_ref144 , vert_uv2_v_ref145));
				#ifdef USE_CUSTOM_ON
				float2 staticSwitch67 = appendResult66;
				#else
				float2 staticSwitch67 = _main_scale;
				#endif
				float vert__uv2_w_ref146 = IN.ase_texcoord3.z;
				float2 appendResult7 = (float2(_Main_UPanner , vert__uv2_w_ref146));
				float4 tex2DNode1 = tex2D( _Main_Texture, (( uv_Main_Texture + ( (UnpackNormalScale( tex2D( _Noise_Texture, (uv_Noise_Texture*1.0 + ( appendResult32 * _TimeParameters.x )) ), 1.0f )).xy * _Noise_Str ) )*staticSwitch67 + appendResult7) );
				#ifdef _USE_MAIN_STEP_ON
				float staticSwitch174 = tex2DNode1.r;
				#else
				float staticSwitch174 = step( _main_step , tex2DNode1.r );
				#endif
				float main_r_ref156 = staticSwitch174;
				float2 texCoord8 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float uv_v_ref136 = texCoord8.y;
				#ifdef _MASK_1X_ON
				float staticSwitch81 = ( 1.0 - uv_v_ref136 );
				#else
				float staticSwitch81 = uv_v_ref136;
				#endif
				#ifdef _MASK_MIRROR_ON
				float staticSwitch82 = ( saturate( ( uv_v_ref136 * ( 1.0 - uv_v_ref136 ) ) ) * 4.0 );
				#else
				float staticSwitch82 = staticSwitch81;
				#endif
				float mask_pow_ref141 = _Mask_Pow;
				float2 texCoord83 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _USE_MASKTEX_ON
				float staticSwitch89 = saturate( pow( tex2D( _Mask_Texture, texCoord83 ).r , mask_pow_ref141 ) );
				#else
				float staticSwitch89 = saturate( pow( staticSwitch82 , mask_pow_ref141 ) );
				#endif
				float mask_ref139 = staticSwitch89;
				#ifdef _USE_ADDMULITYMASK_ON
				float staticSwitch109 = ( ( main_r_ref156 + mask_ref139 ) * mask_ref139 );
				#else
				float staticSwitch109 = ( main_r_ref156 * mask_ref139 );
				#endif
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth69 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth69 = abs( ( screenDepth69 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Fade_Distance ) );
				

				surfaceDescription.Alpha = ( vert_alpha_ref166 * ( saturate( ( _Opacity * ( saturate( ( tex2D( _Diss_Texture, panner54 ).r + staticSwitch62 ) ) * staticSwitch109 ) ) ) * saturate( distanceDepth69 ) ) );
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
Node;AmplifyShaderEditor.CommentaryNode;171;-2269.902,-704.1467;Inherit;False;3334.423;758.4979;;40;156;59;167;105;23;2;17;154;19;61;16;18;131;163;162;132;157;1;7;150;5;68;148;147;66;67;65;28;4;121;120;119;118;117;116;115;114;172;173;174;Main;0.2122642,1,0.4933898,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;170;-62.80074,96.63489;Inherit;False;1185.532;505.1816;;8;60;168;71;22;20;21;56;107;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;151;-3848.811,-50.56902;Inherit;False;1274.378;305.3345;;10;58;166;165;153;152;63;146;145;144;64;Vertex Data;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;35;-3850.724,-626.2464;Inherit;False;1516.7;459;;12;26;27;25;24;123;30;124;127;126;33;34;32;Normal;0.8918172,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;15;-2526.43,767.914;Inherit;False;2481.993;1008.156;;31;136;8;109;161;158;14;110;108;139;140;160;143;87;86;141;85;84;83;142;89;13;12;82;81;79;138;11;45;137;9;10;Mask;1,0.0518868,0.9405553,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;57;-1728.904,191.0442;Inherit;False;1654.966;556.1593;;11;155;46;49;47;62;48;54;53;50;51;52;Dissolve;0,0.1466861,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;106;-1351.096,-1231.218;Inherit;False;1538.653;484.4775;;14;164;102;103;100;101;111;98;99;96;95;97;93;92;90;Sub Texture;0.8605148,1,0,1;0;0
Node;AmplifyShaderEditor.PannerNode;54;-1190.843,281.0443;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;90;-749.3624,-1181.218;Inherit;True;Property;_Sub_Texture;Sub_Texture;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;107;126.7364,382.2562;Inherit;False;675.7916;190.0119;DepthFade;3;69;70;72;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;76;1366.23,-419.3442;Inherit;False;204;332;;3;73;75;74;Enum;0,0,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-3603.724,-407.2458;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-2129.675,1255.495;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-2288.675,1311.494;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-2463.372,1248.26;Inherit;False;136;uv_v_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;-2011.222,1255.655;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1888.476,1256.194;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-2456.581,1126.337;Inherit;False;136;uv_v_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;79;-2283.805,1194.493;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;81;-2133.204,1126.792;Inherit;False;Property;_Mask_1x;Mask_-1x;31;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;82;-1716.705,1124.292;Inherit;False;Property;_Mask_Mirror;Mask_Mirror;32;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;12;-1464.359,1128.71;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;13;-1330.796,1128.763;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;89;-1142.543,1122.223;Inherit;False;Property;_Use_MaskTex;Use_MaskTex;30;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;-1683.872,1219.829;Inherit;False;141;mask_pow_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;83;-2467.66,1499.902;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;84;-2262.204,1469.034;Inherit;True;Property;_Mask_Texture;Mask_Texture;26;0;Create;True;0;0;0;False;0;False;-1;None;df9e34652b2692b4daf5cea2ebeee232;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;85;-2428.32,1655.659;Inherit;False;Property;_Mask_Pow;Mask_Pow;27;0;Create;True;0;0;0;False;0;False;5.346326;1;1;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;141;-2164.547,1654.579;Inherit;False;mask_pow_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;86;-1898.773,1497.511;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;87;-1768.868,1496.998;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;143;-1264.703,1437.442;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-3829.724,-412.2458;Inherit;False;Property;_Noise_Tex_UPanner;Noise_Tex_UPanner;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-3829.724,-342.2458;Inherit;False;Property;_Noise_Tex_VPanner;Noise_Tex_VPanner;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;126;-3714.628,-267.3362;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-3829.628,-270.3362;Inherit;False;Constant;_Float0;Float 0;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-3424.513,-290.7978;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-3827.724,-529.246;Inherit;False;0;24;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;123;-3253.513,-521.7978;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;24;-3075.724,-552.2458;Inherit;True;Property;_Noise_Texture;Noise_Texture;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;25;-2800.723,-553.2458;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2799.723,-478.2458;Inherit;False;Property;_Noise_Str;Noise_Str;13;0;Create;True;0;0;0;False;0;False;0.06793601;0;0;0.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2495.723,-547.2458;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;64;-3798.811,-0.5690207;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-3549.695,15.73883;Inherit;False;vert_uv2_u_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;145;-3548.695,84.7388;Inherit;False;vert_uv2_v_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;-3546.695,149.7388;Inherit;False;vert__uv2_w_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;63;-3351.296,9.622869;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-3160.871,98.11903;Inherit;False;vert_uv1_v_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-3163.471,26.62468;Inherit;False;vert_uv1_u_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;-1091.208,981.9459;Inherit;False;156;main_r_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-1086.381,1052.12;Inherit;False;139;mask_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;139;-930.8084,1122.489;Inherit;False;mask_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;108;-856.7808,1033.898;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-703.9854,1104.373;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-515.5653,968.8134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;-707.2082,961.9459;Inherit;False;156;main_r_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-704.2082,1031.946;Inherit;False;139;mask_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;109;-323.6926,1074.369;Inherit;False;Property;_Use_AddMulityMask;Use_AddMulityMask;33;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-2457.379,853.6682;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-2262.375,898.8345;Inherit;False;uv_v_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-1360.843,402.0443;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1567.843,398.0443;Inherit;False;Property;_Diss_Tex_UPanner;Diss_Tex_UPanner;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1567.843,475.0443;Inherit;False;Property;_Diss_Tex_VPanner;Diss_Tex_VPanner;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;-1026.942,252.1764;Inherit;True;Property;_Diss_Texture;Diss_Texture;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-686.152,282.0368;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;49;-579.8756,283.1169;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;62;-944.2194,535.9821;Inherit;False;Property;Use_Custom;Use_Custom;29;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1247.7,536.4635;Inherit;False;Property;_Dissolve;Dissolve;19;0;Create;True;0;0;0;False;0;False;0;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-1165.994,607.4144;Inherit;False;152;vert_uv1_v_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;52;-1444.955,277.5331;Inherit;False;0;46;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;92;-943.3579,-1152.739;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;93;-1201.358,-1156.739;Inherit;False;0;90;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;97;-1122.602,-1035.391;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-1323.602,-1039.391;Inherit;False;Property;_SubTex_UPanner;SubTex_UPanner;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1321.602,-970.3903;Inherit;False;Property;_SubTex_VPanner;SubTex_VPanner;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-730.6024,-999.3904;Inherit;False;Property;_SubTex_Pow;SubTex_Pow;23;0;Create;True;0;0;0;False;0;False;0;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;98;-408.6026,-1152.391;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;111;-278.8634,-1152.49;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-407.4509,-1061.675;Inherit;False;Property;_SubTex_Ins;SubTex_Ins;22;0;Create;True;0;0;0;False;0;False;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-116.4508,-1152.675;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;103;-340.4509,-989.6743;Inherit;False;Property;_SubTex_Color;SubTex_Color;21;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;31.54933,-1151.675;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;164;-16.52329,-998.4885;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;165;-2818.101,15.97486;Inherit;False;vert_color_ref;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-2813.566,101.0507;Inherit;False;vert_alpha_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;58;-2973.459,19.4724;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;112;1894.28,-553.3848;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;114;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;115;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;116;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;117;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;2;5;True;_Src;10;True;_Dst;3;1;True;_Src;10;True;_Dst;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;118;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;119;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;120;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;121;1058.258,-529.7874;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;74;1409.153,-273.4828;Inherit;False;Property;_Src;Src;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;1409.153,-180.4828;Inherit;False;Property;_Dst;Dst;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;1405.153,-360.4828;Inherit;False;Property;_CullMode;CullMode;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;113;1123.337,-420.4637;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;1_fx/shokewave_voff;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_CullMode;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;True;True;2;5;True;_Src;10;True;_Dst;3;1;True;_Src;10;True;_Dst;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;1;638387539112172886;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;0;638387539142243526;  Use Shadow Threshold;0;0;Receive Shadows;0;638387539148933291;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.DepthFade;69;382.6844,437.2682;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;125.9243,285.0859;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;222.7364,455.5418;Inherit;False;Property;_Fade_Distance;Fade_Distance;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;70;607.5276,434.2563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-12.80074,214.7262;Inherit;False;Property;_Opacity;Opacity;10;0;Create;True;0;0;0;False;0;False;1;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;267.5573,218.4636;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;383.8939,215.2482;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;798.118,216.5416;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;739.037,146.6349;Inherit;False;166;vert_alpha_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;941.0321,152.0286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2173.541,-654.1467;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-1932.446,-573.0547;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;65;-1468.33,-569.4441;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;67;-1771.061,-384.1566;Inherit;False;Property;Use_Custom;Use_Custom;19;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;-2013.332,-354.7067;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-2216.902,-359.4445;Inherit;False;144;vert_uv2_u_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;-2219.902,-289.4445;Inherit;False;145;vert_uv2_v_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;68;-1959.532,-475.7446;Inherit;False;Property;_main_scale;main_scale;6;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;5;-1887.822,-125.5775;Inherit;False;Property;_Main_UPanner;Main_UPanner;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-1910.364,-55.09318;Inherit;False;146;vert__uv2_w_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-1690.822,-119.5775;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1299.095,-597.5952;Inherit;True;Property;_Main_Texture;Main_Texture;4;0;Create;True;0;0;0;False;0;False;-1;07454a717364c364b9bc4becd33778da;aaf570bac628fa54c9d0892eda2e8614;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;157;-1018.185,-492.8623;Inherit;False;156;main_r_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-999.5313,-422.1104;Inherit;False;Property;_Ceil;Ceil;28;0;Create;True;0;0;0;False;0;False;0;0.56;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;162;-840.5642,-488.1483;Inherit;False;F_Toon;-1;;1;611313da66c9ce042a4ae9bf3e5290ab;0;2;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;163;-674.8557,-494.4307;Inherit;False;True;False;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;16;-181.6326,-563.8167;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;61;-243.8936,-329.7109;Inherit;False;Property;Use_Custom;Use_Custom;16;0;Create;False;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-558.478,-329.6097;Inherit;False;Property;_Main_Ins;Main_Ins;8;0;Create;True;0;0;0;False;0;False;1;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-478.6737,-258.9673;Inherit;False;153;vert_uv1_u_ref;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;26.3279,-349.149;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;27.90454,-519.5952;Inherit;False;Property;_TintColor;Tint Color;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0.5894622,0.6520594,0.4398325,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;288.7679,-374.2334;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;105;507.0149,-397.2177;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;167;509.037,-470.3651;Inherit;False;165;vert_color_ref;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;716.778,-420.5812;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;169;375.4846,-1035.132;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;173;-1154.59,-668.1628;Inherit;False;Property;_main_step;main_step;5;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;172;-993.5899,-663.1628;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;174;-864.8218,-598.2371;Inherit;False;Property;_Use_main_step;Use_main_step;35;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-650.1851,-600.4622;Inherit;False;main_r_ref;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;131;-447.6854,-601.0103;Inherit;False;Property;_Use_Ceil;Use_Ceil;34;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-469.5322,-472.7413;Inherit;False;Property;_Main_Pow;Main_Pow;9;0;Create;True;0;0;0;False;0;False;1;0;0;10;0;1;FLOAT;0
WireConnection;54;0;52;0
WireConnection;54;2;53;0
WireConnection;90;1;92;0
WireConnection;32;0;34;0
WireConnection;32;1;33;0
WireConnection;10;0;137;0
WireConnection;10;1;9;0
WireConnection;9;0;137;0
WireConnection;45;0;10;0
WireConnection;11;0;45;0
WireConnection;79;0;138;0
WireConnection;81;1;138;0
WireConnection;81;0;79;0
WireConnection;82;1;81;0
WireConnection;82;0;11;0
WireConnection;12;0;82;0
WireConnection;12;1;142;0
WireConnection;13;0;12;0
WireConnection;89;1;13;0
WireConnection;89;0;143;0
WireConnection;84;1;83;0
WireConnection;141;0;85;0
WireConnection;86;0;84;1
WireConnection;86;1;141;0
WireConnection;87;0;86;0
WireConnection;143;0;87;0
WireConnection;126;0;127;0
WireConnection;124;0;32;0
WireConnection;124;1;126;0
WireConnection;123;0;30;0
WireConnection;123;2;124;0
WireConnection;24;1;123;0
WireConnection;25;0;24;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;144;0;64;1
WireConnection;145;0;64;2
WireConnection;146;0;64;3
WireConnection;152;0;63;2
WireConnection;153;0;63;1
WireConnection;139;0;89;0
WireConnection;108;0;160;0
WireConnection;108;1;140;0
WireConnection;110;0;108;0
WireConnection;110;1;139;0
WireConnection;14;0;158;0
WireConnection;14;1;161;0
WireConnection;109;1;14;0
WireConnection;109;0;110;0
WireConnection;136;0;8;2
WireConnection;53;0;51;0
WireConnection;53;1;50;0
WireConnection;46;1;54;0
WireConnection;47;0;46;1
WireConnection;47;1;62;0
WireConnection;49;0;47;0
WireConnection;62;1;48;0
WireConnection;62;0;155;0
WireConnection;92;0;93;0
WireConnection;92;2;97;0
WireConnection;97;0;95;0
WireConnection;97;1;96;0
WireConnection;98;0;90;1
WireConnection;98;1;99;0
WireConnection;111;0;98;0
WireConnection;100;0;111;0
WireConnection;100;1;101;0
WireConnection;102;0;100;0
WireConnection;102;1;164;0
WireConnection;164;0;103;0
WireConnection;165;0;58;0
WireConnection;166;0;58;4
WireConnection;113;2;59;0
WireConnection;113;3;60;0
WireConnection;69;0;72;0
WireConnection;56;0;49;0
WireConnection;56;1;109;0
WireConnection;70;0;69;0
WireConnection;20;0;21;0
WireConnection;20;1;56;0
WireConnection;22;0;20;0
WireConnection;71;0;22;0
WireConnection;71;1;70;0
WireConnection;60;0;168;0
WireConnection;60;1;71;0
WireConnection;28;0;4;0
WireConnection;28;1;26;0
WireConnection;65;0;28;0
WireConnection;65;1;67;0
WireConnection;65;2;7;0
WireConnection;67;1;68;0
WireConnection;67;0;66;0
WireConnection;66;0;147;0
WireConnection;66;1;148;0
WireConnection;7;0;5;0
WireConnection;7;1;150;0
WireConnection;1;1;65;0
WireConnection;162;1;157;0
WireConnection;162;2;132;0
WireConnection;163;0;162;0
WireConnection;16;0;131;0
WireConnection;16;1;18;0
WireConnection;61;1;19;0
WireConnection;61;0;154;0
WireConnection;17;0;16;0
WireConnection;17;1;61;0
WireConnection;23;0;2;0
WireConnection;23;1;17;0
WireConnection;105;0;169;0
WireConnection;105;1;23;0
WireConnection;59;0;167;0
WireConnection;59;1;105;0
WireConnection;169;0;102;0
WireConnection;172;0;173;0
WireConnection;172;1;1;1
WireConnection;174;1;172;0
WireConnection;174;0;1;1
WireConnection;156;0;174;0
WireConnection;131;1;156;0
WireConnection;131;0;163;0
ASEEND*/
//CHKSM=FE26409CCDE5EE519CC036BB2B0DB26AE7506138