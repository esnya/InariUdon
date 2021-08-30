// Upgrade NOTE: upgraded instancing buffer 'InariUdonSurfaceEmissive' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "InariUdon/Surface Emissive"
{
	Properties
	{
		_Color("Color", Color) = (0.8,0.8,0.8,1)
		[HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_OffsetFactor("Offset Factor", Float) = 0
		_OffsetUnit("Offset Unit", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Offset  [_OffsetFactor] , [_OffsetUnit]
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma only_renderers d3d11_9x d3d11 gles3 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		uniform float _OffsetFactor;
		uniform float _OffsetUnit;
		uniform float _Metallic;
		uniform float _Smoothness;

		UNITY_INSTANCING_BUFFER_START(InariUdonSurfaceEmissive)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr InariUdonSurfaceEmissive
			UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
#define _EmissionColor_arr InariUdonSurfaceEmissive
		UNITY_INSTANCING_BUFFER_END(InariUdonSurfaceEmissive)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			o.Albedo = _Color_Instance.rgb;
			float4 _EmissionColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_EmissionColor_arr, _EmissionColor);
			o.Emission = ( _EmissionColor_Instance * i.vertexColor ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
970;875;1920;1006;1998.587;678.2058;1.694437;True;True
Node;AmplifyShaderEditor.ColorNode;6;-512,448;Inherit;False;InstancedProperty;_EmissionColor;Emission Color;1;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;4,4,4,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;7;-496,640;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2;-512,64;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-512,160;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-512,256;Inherit;False;Property;_OffsetFactor;Offset Factor;4;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-512,352;Inherit;False;Property;_OffsetUnit;Offset Unit;5;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-513.5,-126;Inherit;False;InstancedProperty;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0.8,0.8,0.8,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-288,512;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;InariUdon/Surface Emissive;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;Back;0;False;-1;0;False;-1;True;0;True;4;0;True;5;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;3;d3d11_9x;d3d11;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;0;0;1;0
WireConnection;0;2;8;0
WireConnection;0;3;3;0
WireConnection;0;4;2;0
ASEEND*/
//CHKSM=3BACD1095A1C8E45D3E8E008B79C0E50628FDA9F