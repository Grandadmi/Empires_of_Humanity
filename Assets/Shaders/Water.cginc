float Foam(float shore, float2 worldXZ, sampler2D noiseTex) 
{
	shore = sqrt(shore) * 0.9;

	float2 noiseUV = worldXZ + _Time.y * 0.025;
	float4 noise = tex2D(noiseTex, noiseUV * 0.0575);

	float distortion1 = noise.x * (1 - shore);
	float foam1 = sin((shore + distortion1) * 10 - _Time.y);
	foam1 *= foam1 * shore;

	float distortion2 = noise.y * (1 - shore);
	float foam2 = sin((shore + distortion1) * 10 - _Time.y + 2);
	foam2 *= foam2 * 0.7;

	return max(foam1, foam2) * shore;
}
float Waves(float2 worldXZ, sampler2D noiseTex)
{
	float2 uv1 = worldXZ;
	uv1.y += _Time.y * 0.05;
	float4 noise1 = tex2D(noiseTex, uv1 * 0.0115);

	float2 uv2 = worldXZ;
	uv2.x += _Time.y * 0.05;
	float4 noise2 = tex2D(noiseTex, uv2 * 0.0115);

	float blendWave = /*0,*/ sin((worldXZ.x + worldXZ.y) * 0.05 + (noise1.y + noise2.z) + (_Time.y * 0.5));
	blendWave *= blendWave;

	float waves = lerp(noise1.z, noise1.w, blendWave) + lerp(noise2.x, noise2.y, blendWave);
	return smoothstep(0.75, 2, waves);
}
float River(float2 riverUV, sampler2D noiseTex) 
{
	float2 uv = riverUV;
	uv.x = uv.x * 0.0625 + _Time.y * 0.00005;
	uv.y -= _Time.y * 0.125;
	float4 noise = tex2D(noiseTex, uv);

	float2 uv2 = riverUV;
	uv2.x = uv2.x * 0.0625 + _Time.y * 0.000052;
	uv2.y -= _Time.y * 0.0623;
	float4 noise2 = tex2D(noiseTex, uv2);

	return noise.x * noise2.w;
}