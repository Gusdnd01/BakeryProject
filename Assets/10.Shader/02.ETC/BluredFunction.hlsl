void BluredFunction_float(float4 Seed, float Min, float Max, float BlurX, float BlurY, out float4 BlurOut){
	float randnum = frac(sin(dot(Seed.xy, float2 (13, 78))) * 43758.5453);
	float noise = lerp(Min, Max , randnum);

	float uvx = float(sin(noise)) * BlurX;
	float uvy = float(cos(noise))* BlurY;

	float4 uvpos = float4( Seed.x + uvx, Seed.y + uvy, Seed.zw);

	BlurOut = uvpos;
}