Shader "prime[31]/Transitions/Texture With Alpha"
{
	Properties
	{
		_MainTex( "Base (RGB)", 2D ) = "white" {}
		_Color( "Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off

		Pass
		{
CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"


	uniform sampler2D _MainTex;
	uniform float _Progress;
	uniform fixed4 _Color;


	fixed4 frag( v2f_img i ):COLOR
	{
	    return tex2D( _MainTex, i.uv ) * _Color;
	}

ENDCG
		}
	}

	FallBack off
}