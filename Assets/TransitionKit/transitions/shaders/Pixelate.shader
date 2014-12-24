// good min 0.001f and good max 0.08f;
Shader "prime[31]/Transitions/Pixelate"
{
	Properties
	{
		_MainTex( "Base (RGB)", 2D ) = "white" {}
        _CellSize( "Cell Size", float ) = 0.02
        // this should be 1 / Screen.aspect. It is used to make the pixellation square
		_WidthAspectMultiplier( "Width Aspect Multiplier", float ) = 1.0
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }


CGPROGRAM

#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"


sampler2D _MainTex;
fixed _CellSize;
fixed _WidthAspectMultiplier;



fixed4 frag( v2f_img i ):COLOR
{
	float2 cellSize = float2( _CellSize * _WidthAspectMultiplier, _CellSize );
    float2 steppedUV = i.uv.xy;
    steppedUV /= cellSize;
    steppedUV = round( steppedUV );
    steppedUV *= cellSize;

    return tex2D( _MainTex, steppedUV );
}

ENDCG
		}
	}

	FallBack off
}
