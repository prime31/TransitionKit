Shader "prime[31]/Transitions/Blur"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_BlurSize ( "Blur Size", Range( 0.0, 0.01 ) ) = 0.05
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

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
uniform float _BlurSize;


fixed4 frag( v2f_img i ) : COLOR
{
	fixed4 sum = fixed4( 0.0, 0.0, 0.0, 0.0 );
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y - 4.0 * _BlurSize ) ) * 0.05;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y - 3.0 * _BlurSize ) ) * 0.09;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y - 2.0 * _BlurSize ) ) * 0.12;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y - _BlurSize ) ) * 0.15;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y ) ) * 0.16;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y + _BlurSize ) ) * 0.15;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y + 2.0 * _BlurSize ) ) * 0.12;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y + 3.0 * _BlurSize ) ) * 0.09;
	sum += tex2D( _MainTex, half2( i.uv.x, i.uv.y + 4.0 * _BlurSize ) ) * 0.05;

	return sum;
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
