Shader "prime[31]/Transitions/Fish Eye"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_Progress ( "Progress", Range( 0.0, 1.0 ) ) = 0.0
		_Size ( "Size", Range( 0.0, 0.4 ) ) = 0.2
		_Zoom ( "Zoom", Range( 0.0, 150.0 ) ) = 100.0
		_ColorSeparation ( "Color Separation", Range( 0.0, 5.0 ) ) = 0.2
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
uniform float _Zoom;
uniform float _ColorSeparation;


fixed4 frag( v2f_img i ) : COLOR
{
	float inv = 1.0 - _Progress;
	float2 disp = _Size * half2( cos( _Zoom * i.uv.x ), sin( _Zoom * i.uv.y ) );
	half4 texTo = half4( 0.0, 0.0, 0.0, 0.0 );

	half4 texFrom = half4
	(
		tex2D( _MainTex, i.uv + _Progress * disp * ( 1.0 - _ColorSeparation ) ).r,
		tex2D( _MainTex, i.uv + _Progress * disp ).g,
		tex2D( _MainTex, i.uv + _Progress * disp * ( 1.0 + _ColorSeparation ) ).b,
	1.0 );

	return texTo * _Progress + texFrom * inv;
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
