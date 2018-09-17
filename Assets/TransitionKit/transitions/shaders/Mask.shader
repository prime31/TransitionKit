// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// where _MaskTex is
Shader "prime[31]/Transitions/Mask"
{
	Properties
	{
		_MainTex( "Base (RGB)", 2D ) = "white" {}
		_MaskTex( "Mask Texture", 2D ) = "black" {}
		_Progress( "Progress", Range( 0.0, 1.0 ) ) = 0.0
		_Color( "Background Color", Color ) = ( 0, 0, 0, 1 )
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
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"


	uniform sampler2D _MainTex;
	uniform sampler2D _MaskTex;
	uniform float4 _MaskTex_ST;
	uniform float _Progress;
	uniform fixed4 _Color;


	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 uvMask : TEXCOORD1;
	};



	float mapper( float value, float min, float max )
	{
		return min + value * ( max - min );
	}


	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos( v.vertex );
		o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );

		// _ST.xy is scale and _ST.zw is offset
		float aspectRatio = _ScreenParams.x / _ScreenParams.y;
		float tiling = mapper( _Progress, 0, 25.0 );

		// our offset maps to our tiling in the form of -0.5 * tiling + 0.5. we have to take into account the aspectRatio scaling
		// on the x axis though so we need to calcuate everything separately for x and y
		float offsetX =  -0.5 * tiling * aspectRatio + 0.5;
		float offsetY = -0.5 * tiling + 0.5;
		o.uvMask.x = v.texcoord.x * ( _MaskTex_ST.x * tiling * aspectRatio ) + offsetX;
		o.uvMask.y = v.texcoord.y * ( _MaskTex_ST.y * tiling ) + offsetY;

		return o;
	}


	fixed4 frag( v2f i ):COLOR
	{
	    fixed4 texColor = tex2D( _MainTex, i.uv );
	    fixed4 maskColor = tex2D( _MaskTex, i.uvMask );

		// we use a non-zero as the cutoff because when the screen grab is blended with the mask for small sizes it will average the alpha
		if( maskColor.a <= 0.3 )
			return texColor;

		return _Color;
	}

ENDCG
		}
	}

	FallBack off
}
