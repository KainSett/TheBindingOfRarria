sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float uDistance;
float uOffset;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float offset;
float progress;
float power = 3;
float vignetteRadius = 0.1;
float vignetteSoftness = 0.3;

const static float3x3 sobelX = float3x3(-1.0, -2.0, -1.0, 0.0, 0.0, 0.0, 1.0, 2.0, 1.0);
const static float3x3 sobelY = float3x3(-1.0, 0.0, 1.0, -2.0, 0.0, 2.0, -1.0, 0.0, 1.0);

float sigmoid(float x, float f) //a function such that x => -infinity; y => 0, x => +infinity, y => 1 (range: (0, 1), domain: XER)
{
    return 1.0 / (1.0 + exp(-f * x));
}

float vignette(float2 uv, float2 resolution, float softness, float radius)
{
    float dist = length(uv * 2.0 - 1.0);

    float final = 1.0 - smoothstep(radius, radius + softness, dist);
    return final;
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, uv);
    float4 final = float4(0, 0, 0, 0);
    float2 sum = float2(0, 0);
    
    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            float2 offset = float2(uv.x + x * 2 / uScreenResolution.x, uv.y + y * 2 / uScreenResolution.y);
            
            sum.x += length(tex2D(uImage0, offset).xyz) * float(sobelX[1 + x][1 + y]) * ((1.0 - abs(uv.x - 0.5)) * 0.5);
            sum.y += length(tex2D(uImage0, offset).xyz) * float(sobelY[1 + x][1 + y]) * ((1.0 - abs(uv.x - 0.5)) * 0.5);
        }
    }
    
    float3 blackInterpolant = lerp(color.rgb, float3(0, 0, 0), saturate(progress * 15.0));
    
    float g = sigmoid(abs(sum.x) + abs(sum.y) - offset, power);
    g = saturate(g * 2.0); 
    
    float3 vignetteOverlay = vignette(uv, uScreenResolution, vignetteSoftness, vignetteRadius);
    
    final.rgb = lerp(blackInterpolant, float3(g, g, g), progress * vignetteOverlay);
    
    return final;
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}