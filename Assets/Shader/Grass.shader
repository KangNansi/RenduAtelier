Shader "Custom/Grass" {
	Properties{
		_Scale("Scale", Range(0.0,100.0)) = 100.0
		_Color("Color", Color) = (1.0,1.0,1.0)
		_Scale2("Scale2", Range(0.0,5.0)) = 1.0
		_Scale3("Scale3", Range(0.0,5.0)) = 1.0
		_Texture("Texture", 2D) = "white"
		_Contrast("Contraste", Float) = 1.0
	}
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		
		float _Scale;
		float3 _Color;
		float _Scale2;
		float _Scale3;
		float _Contrast;
		sampler2D _Texture;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

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

		float rand(float3 co)
		{
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		float3 getNormal(float2 pn) {
			float mul = 20;
			float pm = frac(noise(float3(pn.x, pn.y, 0)*mul));
			float pl = frac(noise(float3(pn.x - 1, pn.y, 0)*mul));
			float pr = frac(noise(float3(pn.x + 1, pn.y, 0)*mul));
			float pu = frac(noise(float3(pn.x, pn.y + 1, 0)*mul));
			float pd = frac(noise(float3(pn.x, pn.y - 1, 0)*mul));
			return normalize(float3(pm - pl, 0, 1) + float3(pm - pr, 0, 1) + float3(0, pm - pu, 1) + float3(0, pm - pd, 1));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float nois, nois2;
			float alt;
			float alt2;
			float seed;
			float heigth = frac(noise(IN.worldPos)*0.5f) / 2;
			float hmod = 20;
			if (IN.worldPos.z/hmod + (frac(noise(float3(IN.worldPos.x, IN.worldPos.y, 0)*20))*0.05f-0.025f) > 0.75f) {
				nois = 1 - frac(noise(IN.worldPos * 2)) / 4;
				o.Albedo = 1 * nois;
			}
			else if (IN.worldPos.z/hmod + (frac(noise(float3(IN.worldPos.x, IN.worldPos.y, 0)*20))*0.3f-0.15f) > 0.3f) {
				// Albedo comes from a texture tinted by color
				alt = 1 - frac(noise(_Scale3*(IN.worldPos.z + noise(IN.worldPos * _Scale2)))) / 3;
				alt2 = 1 - frac(noise(float3(IN.worldPos.x*(_Scale / 3.), IN.worldPos.y*(_Scale / 3.), 0)));
				seed = 1 - frac(noise(float3(IN.worldPos.x*_Scale, IN.worldPos.y*_Scale, 0)))*alt2 / _Contrast;
				o.Albedo = tex2D(_Texture, IN.uv_MainTex).rgb*_Color*half3(seed, seed, seed)*alt;
			}
			else if (IN.worldPos.z / hmod> 0.0f){
				nois = 1 - frac(noise(float3(IN.worldPos.x, IN.worldPos.y,0)*20))/5.;
				nois2 = 1 - frac(noise(float3(IN.worldPos.x, IN.worldPos.y, 0) * 20)) / 5.;
				o.Albedo = float3(0, 0.2, 0)*nois+float3(0.2,0.1,0)*nois2;
			}
			else {
				// Albedo comes from a texture tinted by color
				alt = 1 - frac(noise(_Scale3*(IN.worldPos.z + noise(IN.worldPos * _Scale2)))) / 3;
				alt2 = 1 - frac(noise(float3(IN.worldPos.x*(_Scale / 3.), IN.worldPos.y*(_Scale / 3.), 0)));
				seed = 1 - frac(noise(float3(IN.worldPos.x*_Scale, IN.worldPos.y*_Scale, 0)))*alt2 / _Contrast;
				o.Albedo = tex2D(_Texture, IN.uv_MainTex).rgb*_Color*half3(seed, seed, seed)*alt;
			}
			
			// Metallic and smoothness come from slider variables
			o.Normal = getNormal(IN.worldPos);
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
