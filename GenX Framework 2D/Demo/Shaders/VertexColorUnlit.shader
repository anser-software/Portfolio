Shader "MapGen2D/Unlit Transparent Vertex Colored"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

		Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		//Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha
		Fog{ Color(0,0,0,0) }
		Lighting Off
		Cull Off //we can turn backface culling off because we know nothing will be facing backwards

		BindChannels
	{
		Bind "Vertex", vertex
		Bind "texcoord", texcoord
		Bind "Color", color
	}

		SubShader
	{
		Pass
	{
		SetTexture[_MainTex]
	{
		Combine texture * primary
	}
	}
	}
	}
}

