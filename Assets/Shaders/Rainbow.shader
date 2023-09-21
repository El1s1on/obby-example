Shader "Custom/RainbowShader" {
    Properties{
        _Speed("Speed", Range(0, 10)) = 1
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            fixed _Speed;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                fixed3 rainbowColor = fixed3(sin(_Speed * _Time.y + IN.uv_MainTex.x),
                                            sin(_Speed * _Time.y + IN.uv_MainTex.x + 2),
                                            sin(_Speed * _Time.y + IN.uv_MainTex.x + 4));

                o.Albedo = rainbowColor;
                o.Alpha = 1;
            }
            ENDCG
    }
}