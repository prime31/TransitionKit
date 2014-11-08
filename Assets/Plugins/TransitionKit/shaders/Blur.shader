Shader "prime[31]/Transitions/Blur"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_BlurAmount ( "Blur Amount", Range( 0.0, 1.0 ) ) = 0.0005
	}

	SubShader
	{
		//Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
uniform half _BlurAmount;



fixed4 frag( v2f_img i ) : COLOR
{
    half4 texcol = half4( 0.0 );
    float remaining = 1.0f;
    float coef = 1.0;
    float fI = 0;

    for( int j = 0; j < 3; j++ )
    {
    	fI++;
    	coef *= 0.32;
    	texcol += tex2D( _MainTex, float2( i.uv.x, i.uv.y - fI * _BlurAmount ) ) * coef;
    	texcol += tex2D( _MainTex, float2( i.uv.x - fI * _BlurAmount, i.uv.y ) ) * coef;
    	texcol += tex2D( _MainTex, float2( i.uv.x + fI * _BlurAmount, i.uv.y ) ) * coef;
    	texcol += tex2D( _MainTex, float2( i.uv.x, i.uv.y + fI * _BlurAmount ) ) * coef;

    	remaining -= 4 * coef;
    }
    texcol += tex2D( _MainTex, float2( i.uv.x, i.uv.y ) ) * remaining;

    return texcol;
}

ENDCG
		} // end Pass
	} // end SubShader

	FallBack "Diffuse"
}
