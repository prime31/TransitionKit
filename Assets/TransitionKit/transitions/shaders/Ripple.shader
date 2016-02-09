Shader "prime[31]/Transitions/Ripple"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_Progress ( "Progress", Range( 0.0, 1.0 ) ) = 0.0
		_Amplitude ( "Amplitude", Float ) = 100.0
		_Speed ( "Speed", Float ) = 50.0
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
uniform float _Amplitude;
uniform float _Speed;


fixed4 frag( v2f_img i ) : COLOR
{
	half2 dir = i.uv - half2( 0.5, 0.5 );
	float dist = length( dir );
	half2 offset = dir * ( sin( _Time.x * dist * _Amplitude - _Progress * _Speed ) + 0.5 ) / 30.0;

	return lerp( tex2D( _MainTex, i.uv + offset ), fixed4( 0.0, 0.0, 0.0, 0.0 ), smoothstep( 0.5, 1.0, _Progress ) );
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
