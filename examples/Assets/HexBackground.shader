Shader "Unlit/Hex background"
{
	Properties
	{
		_hexCount("Hex count", Float) = 15
		_hexAlpha("Hex alpha", Range(0, 1)) = 0.4
	}

	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off

		Pass
		{
			CGPROGRAM
			#pragma fragment frag
			#pragma vertex vert

			#define iterations 20

			#define volsteps 8

			#define sparsity 0.51  // .4 to .5 (sparse)
			#define stepsize 0.21

			#define frequencyVariation   0.5 // 0.5 to 2.0

			#define brightness 0.0018
			#define distfading 0.700

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

			float4 _color;
			float _hexCount;
			float _hexAlpha;

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos((v.vertex));
				o.uv = v.uv;

				return o;
			}

			float4 hexagon(float2 p)
			{
				float ratio = _ScreenParams.x / _ScreenParams.y;

				float2 q = float2(p.x*2.0*ratio, p.y * ratio + p.x*ratio);

				float2 pi = floor(q);
				float2 pf = frac(q);

				float v = fmod(pi.x + pi.y, 3.0);

				float ca = step(1.0, v);
				float cb = step(2.0, v);
				float2  ma = step(pf.xy, pf.yx);

				// distance to borders
				float e = dot(ma, 1.0 - pf.yx + ca * (pf.x + pf.y - 1.0) + cb * (pf.yx - 2.0*pf.xy));

				// distance to center	
				p = float2(q.x + floor(0.5 + p.y / 1.5), 4.0*p.y / 3.0)*0.5 + 0.5;
				float f = length((frac(p) - 0.5)*float2(1.0, 0.85));

				return float4(pi + ca - cb * ma, e, f);
			}

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float rimSize = 75;
				float2 pos = i.uv;
				float2 p = pos * _hexCount;
				float  r = (1.0 - 0.7)*0.5;
				float4 hex = hexagon(p);
				fixed4 hexagons = (smoothstep(0.0, r + 0.05, hex));
	

				_color = max((0, 0, 0), (1, 1, 1, _hexAlpha * (0.1 + i.uv.x)) - (hexagons.x, hexagons.y, hexagons.z) * 4);

				if (rand(hex.xyy) < 0.6) {
					return _color + 
						(0, 0, 0, (_hexAlpha * rand(hex.xyy) + sin(_Time.x * 50 * rand(hex.xyy)) * (1 - rand(hex.xyy)) / 20) * (0.1 + i.uv.x));
				}

				return _color;
			}
			ENDCG
		}
	}
}