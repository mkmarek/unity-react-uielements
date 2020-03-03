Shader "Unlit/Starfield"
{
	Properties
	{
		_resolutionX("Resolution X", Float) = 256
		_resolutionY("Resolution Y", Float) = 256
		_invResolutionX("Inverse Resolution X", Float) = 256
		_invResolutionY("Inverse Resolution Y", Float) = 256
		_zoom("Zoom", Float) = 1
		_volSteps("Vol steps", Int) = 1
		_originX("Origin X", Float) = 0
		_originY("Origin Y", Float) = 0
		_hexCount("Hex count", Float) = 15
		_hexFill("Hex fill", Range(0, 1)) = 1
		_hexAlpha("Hex alpha", Range(0, 1)) = 0.4
		_rim("Rim", Range(0, 1)) = 0
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

			float _resolutionX;
			float _resolutionY;
			float _invResolutionX;
			float _invResolutionY;
			float4 _color;
			float _zoom;
			int _volSteps;
			float _originX;
			float _originY;
			float _hexCount;
			float _hexFill;
			float _hexAlpha;
			float _rim;

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
				float param = (_ScreenParams.y / _ScreenParams.x) * 2;

				float rimSize = 75;
				float2 pos = i.uv;
				float2 p = pos * _hexCount;
				float  r = (1.0 - 0.7)*0.5;
				float4 hex = hexagon(p);
				fixed4 hexagons = (smoothstep(0.0, r + 0.05, hex));


				if (hex.x + hex.y + rand(hex.xyy) * _hexCount < _hexCount * 6 * _hexFill / param) {
					return (1, 1, 1, 0);
				}

				if (_rim != 0 && (i.uv.y < (rimSize / _ScreenParams.y) * _rim || i.uv.y > 1 - ((rimSize * _rim) / _ScreenParams.y))) {
					return (1, 1, 1, 1);
				}
				
				float2 resolution = (_resolutionX, _resolutionY);
				float2 invResolution = (_invResolutionX, _invResolutionY);
				float3 origin = (_originX, _originY, _Time.x / 100);

				float2 uv = i.uv * invResolution - 0.5;
				uv.y *= resolution.y * invResolution.x;

				float3 dir = float3(uv * _zoom, 1.0);

				float s = 0.1, fade = 0.01;
				_color = float4(0.0, 0.0, 0.0, 1.0);

				for (int r = 0; r < _volSteps; ++r) {
					float3 p = origin + dir * (s * 0.5);
					p = abs(float3(frequencyVariation, frequencyVariation, frequencyVariation) - 
						fmod(p, float3(frequencyVariation * 2.0, frequencyVariation * 2.0, frequencyVariation * 2.0)));

					float prevlen = 0.0, a = 0.0;
					for (int i = 0; i < iterations; ++i) {
						p = abs(p);
						p = p * (1.0 / dot(p, p)) + (-sparsity); // the magic formula            
						float len = length(p);
						a += abs(len - prevlen); // absolute sum of average change
						prevlen = len;
					}

					a *= a * a; // add contrast

					// coloring based on distance        
					if (fade < 0.001) {
						_color += float4((float3(s, s*s, s*s*s) * a * brightness + 1.0) * fade, 1);
					}

					fade *= distfading; // distance fading
					s += stepsize;
				}

				_color = _color + max((0, 0, 0), (1, 1, 1, _hexAlpha) - (hexagons.z, hexagons.z, hexagons.z) * 4);

				if (hex.x + hex.y + rand(hex.xyy) * _hexCount < _hexCount * 6.5 * _hexFill / param) {
					return _color + min((0, 0, 0, 0.8), max(
						(0,0,0,0.2),
						(0,0,0, 0.9 * rand(hex.xyy) + sin(_Time.x * 50 * rand(hex.xyy)) * (1 - rand(hex.xyy)))));
				}

				return _color;
			}
			ENDCG
		}
	}
}