Shader "Custom/UnlitLightNColor" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }

  SubShader
    {
        LOD 200
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Lighting off

        CGPROGRAM
        
        #pragma surface surf NoLighting noambient noforwardadd

        //================================================================
        // VARIABLES
        
        fixed4 _Color;
        sampler2D _MainTex;

        uniform fixed _IsRim;
        uniform fixed4 _RimColor;
        uniform fixed _RimPower;


        struct Input
        {
            half2 uv_MainTex;
            half3 viewDir;
        };
        

        half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten)
        {
           return half4(s.Albedo, s.Alpha);
        }
        
        
        //================================================================
        // SURFACE FUNCTION
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            fixed rim = (1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)));

            o.Albedo = (mainTex.rgb + (_Color.rgb * 0.0f)) / 2.0f;
            //o.Albedo = mainTex.rgb * _Color.rgb;

            o.Alpha = mainTex.a * _Color.a;

            o.Emission = _RimColor.rgb * pow(rim, 1.5 - _RimPower) * _IsRim;

        }
        
        ENDCG
    } 
    FallBack "Diffuse"
}