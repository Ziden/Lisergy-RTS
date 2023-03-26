Shader "Custom/Splatmap" {

Properties {
   _MainTex ("Splat Map", 2D) = "white" {}
   _Texture1 ("Texture 1", 2D) = "white" {}
   _Texture2 ("Texture 2", 2D) = "white" {}
   _Texture3 ("Texture 3", 2D) = "white" {}
   _Texture4 ("Texture 4", 2D) = "white" {}
}



SubShader {
    
    Tags {"RenderType" = "Opaque"}
    
    
    CGPROGRAM
    #pragma surface surf Lambert
    
    
    struct Input {
            float2 uv_MainTex;
            float2 uv_Texture1;
            float2 uv_Texture2;
            float2 uv_Texture3;
    };
    
    
    sampler2D _MainTex;
    sampler2D _Texture1;
    sampler2D _Texture2;
    sampler2D _Texture3;
    
    
    void surf (Input i, inout SurfaceOutput o) {		
            half4 splatmapMask = tex2D (_MainTex, i.uv_MainTex);
            half4 combinedColor = tex2D (_Texture1, i.uv_Texture1) * splatmapMask.r;
            combinedColor += tex2D (_Texture2, i.uv_Texture2) * splatmapMask.g;
            combinedColor += tex2D (_Texture3, i.uv_Texture3) * splatmapMask.b;
            
            o.Albedo = combinedColor;
            }
    ENDCG
}
    
    
    
FallBack "Standard (Specular Setup)"
}