Shader "MapGen2D/TextureColorUnlit"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		Lighting Off
		SetTexture[_MainTex]{
		constantColor[_Color]

		combine constant * texture
	}
		}
	}
}
