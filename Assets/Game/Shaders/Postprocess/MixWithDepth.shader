Shader "PostProcess/MixWithDepth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScreenWidth("Screen Width", Float) = 1024
		_ScreenHeight("Screen Height", Float) = 768
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			struct appdata
			{
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				float color = tex2D(_MainTex, i.uv);
				return pow(Linear01Depth(depth), 1000.1f);//(color);
			}
			ENDCG
		}
	}
}
