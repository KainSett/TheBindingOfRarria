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
float3 color;
float opacity;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float alpha = tex2D(uImage0, uv).a;
    return float4(color, alpha * opacity);
}

Technique tech1
{
    Pass TintPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}