Shader "Voxel Mesh" {
	Properties{
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Side("Side", 2D) = "white" {}
	_Top("Top", 2D) = "white" {}
	_Bottom("Bottom", 2D) = "white" {}
	_SideScale("Side Scale", Float) = 1
		_TopScale("Top Scale", Float) = 1
		_BottomScale("Bottom Scale", Float) = 1
	}

		SubShader{
		Tags{
		"Queue" = "Geometry"
		"IgnoreProjector" = "False"
		"RenderType" = "Opaque"
	}

		LOD 200

		Cull Back
		ZWrite On

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma exclude_renderers flash
#pragma target 3.0

		sampler2D _Side, _Top, _Bottom;
	float _SideScale, _TopScale, _BottomScale;

	struct Input {
		float3 worldPos;
		float3 worldNormal;
	};

	half _Glossiness;
	half _Metallic;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));

		// SIDE X
		float4 x = tex2D(_Side, frac(IN.worldPos.zy * _SideScale)) * abs(IN.worldNormal.x);

		// TOP / BOTTOM
		float4 y = 0;
		if (IN.worldNormal.y > 0) {
			y = tex2D(_Top, frac(IN.worldPos.zx * _TopScale)) * abs(IN.worldNormal.y);
		}
		else {
			y = tex2D(_Bottom, frac(IN.worldPos.zx * _BottomScale)) * abs(IN.worldNormal.y);
		}

		// SIDE Z	
		float4 z = tex2D(_Side, frac(IN.worldPos.xy * _SideScale)) * abs(IN.worldNormal.z);

		o.Albedo = z;
		o.Albedo = lerp(o.Albedo, x, projNormal.x);
		o.Albedo = lerp(o.Albedo, y, projNormal.y);

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		//o.Alpha = c.a;
	}
	ENDCG
	}
		Fallback "Diffuse"
}