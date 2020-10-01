﻿Shader "Custom/Water"
{
    Properties
    {
		_DepthGradientShallow("Depth gradient shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1

		[Space]

		_SurfaceNoise("Surface noise", 2D) = "white"{}
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}

		[Space]

		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0,1)) = 0.27
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0,1)) = 0.777

		[Space]

		_FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04
		_FoamColor("Foam Color", Color) = (1 ,1 ,1 ,1)

		[Space]

		_SurfaceNoiseScroll("Surface Noise Scroll Amout", Vector) = (0.03, 0.03, 0, 0)

		[Space]
		_Divide("Divide", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

        Pass
        {
            CGPROGRAM
			#define SMOOTHSTEP_AA 0.01
            #pragma vertex vert
            #pragma fragment frag
       

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 uv: TEXCOORD0;
				float3 normal : NORMAL; 
				
            };

            struct v2f
            {
                float2 noiseUV : TEXCOORD0;
             
                float4 vertex : SV_POSITION;
				float2 distortUV : TEXCOORD1;
				float4 screenPosition : TEXCOORD2; 
				float3 viewNormal : NORMAL; 
				float4 worldSpacePos : TEXCOORD3; 
            };


			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;

			float _DepthMaxDistance;

			sampler2D _SurfaceNoise; 
			float4 _SurfaceNoise_ST;

			float _SurfaceNoiseCutoff; 

			sampler2D _CameraDepthTexture;

			float _FoamMaxDistance;
			float _FoamMinDistance;
			float4 _FoamColor; 

			float2 _SurfaceNoiseScroll;

			sampler2D _SurfaceDistortion;
			sampler2D _CameraNormalsTexture; 
			float4 _SurfaceDistortion_ST;

			float _SurfaceDistortionAmount;

			float _Divide;

            v2f vert (appdata v)
            {
                v2f o;
				float4 wPosition = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex); 
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				o.noiseUV = wPosition.xz;

				o.noiseUV = (o.noiseUV + v.uv)/ _Divide; 
				o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				o.viewNormal = COMPUTE_VIEW_NORMAL; 
				
				
      
                return o;
            }

			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

				float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition)); 
				float3 normalDot = saturate(dot(existingNormal, i.viewNormal)); 
				float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
				float foamDepthDifference01 = saturate(depthDifference / foamDistance);				

				float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
				float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1)*_SurfaceDistortionAmount; ;
				float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
				float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;				
				float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample); 
		
				float4 surfaceNoiseColor = _FoamColor; 
				surfaceNoiseColor.a *= surfaceNoise;


				return alphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDCG
        }
    }
}
