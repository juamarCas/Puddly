Shader "Custom/ToonShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[Space(10)]
		_OutColor("Outline Color", Color) = (1, 1, 1, 1)
		_OutValue("Outline Value", Range(0.0, 0.2)) = 0.1
		_TexColor("Texture Color", Color) = (0.4,0.4,0.4,1)
		_Color("Color", Color) = (1,1,1,1)
		_ShadowStrength("Shadow", Float) = 1
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
		_Glossiness("Glosiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amout", Range(0,1)) = 0.7
		_RimThreshold("Rim Threshold", Range(0,1)) = 0.1
	

		[HDR]
		[Space(10)]
		_AmbientColor("Ambient Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			//TOON SHADER ----------------------------------
			Pass{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
				}

			//Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			    #pragma multi_compile_fwdbase
				#pragma multi_compile_instancing

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 pos : SV_POSITION;
					float3 worldNormal: NORMAL;
					float3 viewDir: TEXCORD1; 
					SHADOW_COORDS(2)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				float4 _AmbientColor; 
				float4 _SpecularColor; 
				float4 _RimColor; 
				float _RimAmount; 
				float _RimThreshold; 

				float _Glossiness; 
				float _ShadowStrength; 

				v2f vert(appdata v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.viewDir = WorldSpaceViewDir(v.vertex); 
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					TRANSFER_SHADOW(o)
					
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
				
					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);

					float shadow = SHADOW_ATTENUATION(i);
					float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
					float4 light = lightIntensity * _LightColor0; 
					float3 viewDir = normalize(i.viewDir); 

					float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir); 
					float NdotH = dot(normal, halfVector);
					float specularIntesnity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
					float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntesnity);
					float4 specular = specularIntensitySmooth * _SpecularColor; 

					float4 rimDot = 1 - dot(viewDir, normal); 
					float rimIntensity = rimDot * pow(NdotL, _RimThreshold); 
					rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
					float rim =  rimIntensity * _RimColor; 
					fixed4 col = tex2D(_MainTex, i.uv);
					return _Color * col*  (_AmbientColor + light + specular + rim);
				}

					ENDCG

			}
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

			Pass
			{
			Name "CastShadow"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
			}
			
			
		}
}
