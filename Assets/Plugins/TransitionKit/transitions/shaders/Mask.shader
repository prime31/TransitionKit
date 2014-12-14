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
		Tags { "RenderType" = "Opaque" }
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



	float map01( float value, float rightMin, float rightMax )
	{
		return rightMin + value * ( rightMax - rightMin );
	}


	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
		o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );

		// _ST.xy is scale and _ST.zw is offset
		float tiling = map01( _Progress, 0.5, 13.0 );
		float offset = map01( _Progress, 0.25, -6.0 );
		o.uvMask = v.texcoord.xy * ( _MaskTex_ST.xy * tiling ) + ( half2( offset ) );

		return o;
	}


	fixed4 frag( v2f i ):COLOR
	{
	    fixed4 texColor = tex2D( _MainTex, i.uv );
	    fixed4 maskColor = tex2D( _MaskTex, i.uvMask );

		// we use 0.5 as the cutoff because when the screen grab is blended with the mask for small sizes it will average the alpha
		if( maskColor.a <= 0.5 )
			return texColor;

		return _Color;
	}

ENDCG
		}
	}

	FallBack off
}
