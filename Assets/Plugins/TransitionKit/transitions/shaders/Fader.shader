Shader "prime[31]/Transitions/Fader"
{
	Properties
	{
		_MainTex( "Base (RGB)", 2D ) = "white" {}
		_Progress( "Fade Amount", Range( 0.0, 1.0 ) ) = 0.0
		_Color( "Fade to Color", Color ) = ( 0, 0, 0, 1 )
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off

		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }


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
	    fixed4 texColor = tex2D( _MainTex, i.uv );
	    return lerp( texColor, _Color, _Progress );
	}

ENDCG
		}
	}

	FallBack off
}
