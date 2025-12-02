sampler uImage0 : register(s0); // Contents of the screen.
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float4 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime; // Should be set to Main.GlobalTime or a fraction of incrementing time for shine effect.
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float2 uImageSize0;
float Scale;
float ScaleBuffer;
float Pixels;
float4 Color;

float4 Outline(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = float2((coords.x - 0.5f) * ScaleBuffer + 0.5f, (coords.y - 0.5f) * ScaleBuffer + 0.5f);
    
    float4 color = abs(uv.x - 0.5f) > 0.5f || abs(uv.y - 0.5f) > 0.5f ? float4(0, 0, 0, 0) : tex2D(uImage0, uv);
    
    if (color.a != 0 || color.r != 0 || color.g != 0 || color.b != 0)
    {   
        return color;
    }
    
    float pixel = Pixels * Scale / uImageSize0.x;
    
    float angle = 0.0f;
    for (float a = 4; a > 0.0f; a -= 1)
    {
        angle += 3.14f / 2.0f;
        float2 testPoint = uv + float2(pixel * cos(angle), pixel * sin(angle));
        
        color = abs(testPoint.x - 0.5f) > 0.5f || abs(testPoint.y - 0.5f) > 0.5f ? color.rgba : tex2D(uImage0, testPoint);
        
        if (color.a != 0 || color.r != 0 || color.g != 0 || color.b != 0)
        {
            color = Color;
            break;
        }
        
    }
    
    return color;
}

technique Technique1
{
    pass Outline
    {
        PixelShader = compile ps_3_0 Outline();
    }
}