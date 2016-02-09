Shader "prime[31]/Transitions/Doorway"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_Progress ( "Progress", Range( 0.0, 1.0 ) ) = 0.0
		_Perspective ( "Perspective", Range( 0.0, 2.0 ) ) = 1.5
		_Depth ( "Depth", Range( 0.0, 5.0 ) ) = 3.0
		_Direction ( "Direction", int ) = 1
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
uniform float _Perspective;
uniform float _Depth;
uniform int _Direction;


bool inBounds( half2 p )
{
	if( p.x > 0.0 && p.y > 0.0 && p.x < 1.0 && p.y < 1.0 )
		return true;
	return false;
}


fixed4 frag( v2f_img i ) : COLOR
{
	if( _Progress == 0.0 )
		return tex2D( _MainTex, i.uv );

	// we can either go from 0 to -1 or from 0 to -1. normalize here
	if( _Direction == 1 )
		_Progress = _Progress * -1.0;

	half2 pfr = half2( -1.0, -1.0 );
	half2 pto = half2( -1.0, -1.0 );

	float middleSlit = 2.0 * abs( i.uv.x - 0.5 ) - _Progress;
	if( middleSlit > 0.0 )
	{
		pfr = i.uv + ( i.uv.x > 0.5 ? -1.0 : 1.0 ) * half2( 0.5 * _Progress, 0.0 );
		float d = 1.0 / ( 1.0 + _Perspective * _Progress * ( 1.0 - middleSlit ) );
		pfr.y -= d / 2.;
		pfr.y *= d;
		pfr.y += d / 2.;
	}

  float size = lerp( 1.0, _Depth, 1.0 - _Progress );
  pto = ( i.uv + half2( -0.5, -0.5 ) ) * half2( size, size ) + half2( 0.5, 0.5 );

  if( inBounds( pfr ))
    return tex2D( _MainTex, pfr );
  else
    return fixed4( 0.0, 0.0, 0.0, 0.0 );
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
