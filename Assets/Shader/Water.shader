Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Time("Time", Float) = 0.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float hash(float n)
		{
			return frac(sin(n)*43758.5453);
		}

		float noise(float3 x)
		{
			// The noise function returns a value in the range -1.0f -> 1.0f

			float3 p = floor(x);
			float3 f = frac(x);

			f = f*f*(3.0 - 2.0*f);
			float n = p.x + p.y*57.0 + 113.0*p.z;

			return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
				lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
				lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
					lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
		}

		float3 getNormal(float2 pn) {
			float mul = 1;
			float pm = frac(noise((float3(pn.x, pn.y, 0)+_Time)*mul));
			float pl = frac(noise((float3(pn.x - 1, pn.y, 0) + _Time)*mul));
			float pr = frac(noise((float3(pn.x + 1, pn.y, 0) + _Time)*mul));
			float pu = frac(noise((float3(pn.x, pn.y + 1, 0) + _Time)*mul));
			float pd = frac(noise((float3(pn.x, pn.y - 1, 0) + _Time)*mul));
			return normalize(float3(pm - pl, 0, 1) + float3(pm - pr, 0, 1) + float3(0, pm - pu, 1) + float3(0, pm - pd, 1));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color * (1-frac(noise(IN.worldPos+_Time))/5.);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Normal = getNormal(IN.worldPos)/4.+float3(0,0,0.75);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
