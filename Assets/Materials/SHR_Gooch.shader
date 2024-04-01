//stolen from https://github.com/GarrettGunnell/Gooch-Shading/blob/main/Assets/Gooch.shader
//stolen part of https://catlikecoding.com/unity/tutorials/rendering/part-6/ for normal implementation

Shader "Custom/Gooch" {

	Properties {
		// _Albedo ("Albedo", Color) = (1, 1, 1, 1)
		[MainTexture][NoScaleOffset] _ColorMap ("Base Color Map", 2D) = "white" {}
		[MainColor] _ColorTint ("Base Color Tint", Color) = (1, 1, 1, 1)
		//_Smoothness ("Smoothness", Range(0.01, 1)) = 0.5
		[NoScaleOffset] _RoughnessMap ("Roughness Map", 2D) = "white" {}
		_RoughnessFac ("Roughness", Range(0, 0.99)) = 0.5
		_SpecularTint ("Specular Color Tint", Color) = (1, 1, 1, 1)
		[Normal][NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
		// _NormalStrength ("Normal Strength", Float) = 1
		_Cool ("Cool Color", Color) = (1, 1, 1, 1)
		_Warm ("Warm Color", Color) = (1, 1, 1, 1)
		_Alpha ("Alpha", Range(0.01, 1)) = 0.5
		_Beta ("Beta", Range(0.01, 1)) = 0.5
	}

	SubShader {
		Pass {
			Tags {
				"RenderType" = "Opaque"
			}

			CGPROGRAM

			#pragma vertex vp
			#pragma fragment fp

			#include "UnityPBSLighting.cginc"

			sampler2D _ColorMap;
			sampler2D _RoughnessMap;
			sampler2D _NormalMap;

			float4 _ColorTint, _SpecularTint, _Warm, _Cool;

			float _RoughnessFac, _Alpha, _Beta;
			// float _RoughnessFac, _NormalStrength, _Alpha, _Beta;

			struct VertexData {
				float2 uv : TEXCOORD0;
				float4 position : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 position : SV_POSITION;
				float3 normal : TEXCOORD1;
				float4 tangent : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
			};

			v2f vp(VertexData v) {
				v2f i;
				i.uv = v.uv;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
				return i;
			}

			float4 fp(v2f i) : SV_TARGET {
				// float3 tangentSpaceNormal = UnpackScaleNormal(tex2D(_NormalMap, i.uv), _NormalStrength);
				float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
				float3 binormal = cross(i.normal, i.tangent.xyz) * i.tangent.w;

				i.normal = normalize(
					tangentSpaceNormal.x * i.tangent +
					tangentSpaceNormal.y * binormal +
					tangentSpaceNormal.z * i.normal
				);

				float3 lightDir = normalize(float3(1, 1, 0));
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 reflectionDir = reflect(-lightDir, i.normal);
				float3 specular = DotClamped(viewDir, reflectionDir);
				fixed s = 1 - (tex2D(_RoughnessMap, i.uv) * _RoughnessFac);
				specular = pow(specular, s * 500) * _SpecularTint;//tex2D(_RoughnessMap, i.uv);

				float goochDiffuse = (1.0f + dot(lightDir, i.normal)) / 2.0f;
				
				fixed4 c = tex2D(_ColorMap, i.uv) * _ColorTint;

				float3 kCool = _Cool.rgb + _Alpha * c.rgb;
				float3 kWarm = _Warm.rgb + _Beta * c.rgb;

				float3 gooch = (goochDiffuse * kWarm) + ((1 - goochDiffuse) * kCool);
				return float4(gooch, 1.0f);

				return float4(gooch + specular, 1.0f);
			}

			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}
 
			CGPROGRAM
			#pragma vertex vp
			#pragma fragment fp

			#include "UnityCG.cginc"
 
			struct VertexData {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
 
			v2f vp(VertexData v) {
				v2f o;

				o.pos = UnityClipSpaceShadowCasterPos(v.vertex.xyz, v.normal);
				o.pos = UnityApplyLinearShadowBias(o.pos);
				o.uv = v.uv;

				return o;
			}
 
			float4 fp(v2f i) : SV_Target {
				return 0;
			}

			ENDCG
		}
	}
}