Shader "PostProcess/EdgyMode"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_VertexLit("Texture", 2D) = "white" {}
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _VertexLit;
			sampler2D _CameraDepthTexture;

			float _ScreenWidth;
			float _ScreenHeight;

			float3 rgb_to_hsv(float3 RGB)
			{
				float r = RGB.x;
				float g = RGB.y;
				float b = RGB.z;

				float minChannel = min(r, min(g, b));
				float maxChannel = max(r, max(g, b));

				float h = 0;
				float s = 0;
				float v = maxChannel;

				float delta = maxChannel - minChannel;

				if (delta != 0)
				{
					s = delta / v;

					if (r == v) h = (g - b) / delta;
					else if (g == v) h = 2 + (b - r) / delta;
					else if (b == v) h = 4 + (r - g) / delta;
				}

				return float3(h, s, v);
			}

			float3 hsv_to_rgb(float3 HSV)
			{
				float3 RGB = HSV.z;

				float h = HSV.x;
				float s = HSV.y;
				float v = HSV.z;

				float i = floor(h);
				float f = h - i;

				float p = (1.0 - s);
				float q = (1.0 - s * f);
				float t = (1.0 - s * (1 - f));

				if (i == 0) { RGB = float3(1, t, p); }
				else if (i == 1) { RGB = float3(q, 1, p); }
				else if (i == 2) { RGB = float3(p, 1, t); }
				else if (i == 3) { RGB = float3(p, q, 1); }
				else if (i == 4) { RGB = float3(t, p, 1); }
				else /* i == -1 */ { RGB = float3(1, p, q); }

				RGB *= v;

				return RGB;
			}

			inline float4 monochrome(float4 col)
			{
				float c = (col.r + col.b + col.g) / 3;
				return float4(c, c, c, 1);
			}

			inline float4 pick_red(float4 i, float4 col)
			{
				float red_intesity = max(min(col.r - col.b, col.r - col.g), 0);
				float v = (col.r + col.b + col.g) / 3 * i.r;

				// strenghten
				red_intesity = 1 - (1 - red_intesity) * (1 - red_intesity);
				v = v + 0.5 * red_intesity * (1 - v);

				return float4(hsv_to_rgb(float3(0, red_intesity, v)), 1);
			}

			inline float sample_edge(float2 texCoord)
			{
				//return tex2D(_VertexLit, texCoord);
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, texCoord));
				float color = tex2D(_VertexLit, texCoord);
				return pow(Linear01Depth(depth), color);
			}

			float4 sobelish(v2f i) : COLOR0
			{
				// TOP ROW
				float4 s11 = sample_edge(i.uv + float2(-1.0f / _ScreenWidth, -1.0f / _ScreenHeight));
				float4 s12 = sample_edge(i.uv + float2(0, -1.0f / _ScreenHeight));
				float4 s13 = sample_edge(i.uv + float2(1.0f / _ScreenWidth, -1.0f / _ScreenHeight));
				float4 s21 = sample_edge(i.uv + float2(-1.0f / _ScreenWidth, 0));
				float4 s23 = sample_edge(i.uv + float2(-1.0f / _ScreenWidth, 0));
				float4 s31 = sample_edge(i.uv + float2(-1.0f / _ScreenWidth, 1.0f / _ScreenHeight));
				float4 s32 = sample_edge(i.uv + float2(0, 1.0f / _ScreenHeight));
				float4 s33 = sample_edge(i.uv + float2(1.0f / _ScreenWidth, 1.0f / _ScreenHeight));

				float4 col = tex2D(_MainTex, i.uv);

				float4 sh = (s11 * -1.0f + s12 * -2.0f + s13 * -1.0f + s31 * 1.0f + s32 * 2.0f + s33 * 1.0f);
				float4 sv = (s11 * -1.0f + s21 * -2.0f + s31 * -1.0f + s13 * 1.0f + s23 * 2.0f + s33 * 1.0f);

				float4 eh = sh;//monochrome(sh);
				float4 ev = sv;//monochrome(sv);

				/*float4 col = float4(sqrt(horiz * horiz + vert * vert), 1);
				return col;*/

				return pick_red(1 - sqrt(eh * eh + ev * ev), col);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//return sample_edge(i.uv);
				return sobelish(i);

				//float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				//depth = pow(Linear01Depth(depth), 0.3f);
				//

				//return tex2D(_VertexLit, i.uv);
				//fixed4 col = fixed4(0,0,0,1);//tex2D(_CameraDepthTexture, i.uv);
				//col.r = Linear01Depth(i.depth);
				//float4 col = sobelish(i);
				//
			}


			ENDCG
		}
	}
}
