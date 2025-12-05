Shader "Sprites/FlashEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        // 피격 효과용 프로퍼티
        _FlashAmount ("Flash Amount", Range(0, 1)) = 0
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragFlash
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            // 추가 프로퍼티
            float _FlashAmount;
            fixed4 _FlashColor;

            fixed4 SpriteFragFlash(v2f IN) : SV_Target
            {
                // 기본 Sprite 색상 계산
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                
                // FlashAmount만큼 FlashColor로 Lerp
                c.rgb = lerp(c.rgb, _FlashColor.rgb, _FlashAmount);
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}

