Shader "prime[31]/Transitions/Squares"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_Color( "Fade to Color", Color ) = ( 0, 0, 0, 1 )
		_Progress ( "Progress", Range( 0.0, 1.0 ) ) = 0.0
		_Size ( "Size", Vector ) = ( 64.0, 45.0, 0, 0 )
		_Smoothness ( "Smoothness", float ) = 0.5
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off

		Pass
		{
CGPROGRAM
#pragma exclude_renderers ps3 xbox360
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"


// uniforms
uniform sampler2D _MainTex;
uniform fixed4 _Color;
uniform half _Progress;
uniform float2 _Size;
uniform float _Smoothness;



float rand( float2 co )
{
	float x = sin( dot( co.xy, float2( 12.9898, 78.233 ) ) ) * 43758.5453;
	return x - floor( x );
}



fixed4 frag( v2f_img i ) : COLOR
{
	float r = rand( floor( _Size.xy * i.uv ) );
	float m = smoothstep( 0.0, -_Smoothness, r - ( _Progress * ( 1.0 + _Smoothness ) ) );

	return lerp( tex2D( _MainTex, i.uv ), _Color, m );
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
