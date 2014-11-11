Shader "prime[31]/Transitions/Wind"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_Progress ( "Progress", Range( 0.0, 1.0 ) ) = 0.5
		_Size ( "Size", Range( 0.0, 2.0 ) ) = 0.3
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
CGPROGRAM
#pragma exclude_renderers ps3 xbox360
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"


// uniforms
sampler2D _MainTex;
uniform float _Progress;
uniform float _Size;


float rand( half2 co )
{
	float x = sin( dot( co.xy, half2( 12.9898,78.233 ) ) ) * 43758.5453;
	return x - floor( x );
}


fixed4 frag( v2f_img i ) : COLOR
{
	float r = rand( half2( 0, i.uv.y ) );
	float m = smoothstep( 0.0, -_Size, i.uv.x * ( 1.0 - _Size ) + _Size * r - ( _Progress * ( 1.0 + _Size ) ) );
	return lerp( tex2D( _MainTex, i.uv ), half4( 0.0 ), m );
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
