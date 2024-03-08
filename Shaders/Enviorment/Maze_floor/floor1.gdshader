shader_type spatial;
uniform float noiseAmount: hint_range(0.0, 1.0) = 0.08;
uniform vec4 base_color: source_color = vec4(0.537, 0.482, 0.447, 0.0);
uniform vec4 line_color: source_color = vec4(0.537, 0.482, 0.447, 0.0);
uniform sampler2D noise;
uniform sampler2D noise_1d_X;
uniform sampler2D noise_1d_Y;
uniform float tiling: hint_range(0.0, 0.1) = 0.0;
uniform float lineSize: hint_range(0.0, 0.1) = 0.0;
uniform float TILESIZE = 2048;
uniform float scaleOfNode: hint_range(0.0, 100.0) = 10.0;

float round_to_dec(float num, float digit) {
    return round(num * pow(10.0, digit)) / pow(10.0, digit);
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	ALBEDO = base_color.rgb;
	vec3 noiseAtUV = texture(noise, UV).rgb;
	ALBEDO -= noiseAtUV * noiseAmount;

	vec3 randTiles = texture(noise, vec2(round(UV.x * scaleOfNode) / scaleOfNode, round(UV.y * scaleOfNode) / scaleOfNode)).rgb;
	float tilingX = mod(UV.x * randTiles.r + 0.25, tiling);
	float lineSizeRandTiles = lineSize * randTiles.r;
	if (tilingX < lineSizeRandTiles) {
		vec2 normLine = normalize(vec2(tilingX, lineSizeRandTiles));
		ALBEDO *= (vec3(1.0 - pow(sin(normLine.x*3.1415 + 0.25), 0.6)) / 10.0) + 0.01;
		ROUGHNESS = 0.0;
	}

	float tilingY = mod(UV.y + 0.25, tiling);
	if (tilingY < lineSize) {
		vec2 normLine = normalize(vec2(tilingY, lineSize));
		ALBEDO *= (vec3(1.0 - pow(sin(normLine.x*3.1415 + 0.25), 0.6)) / 10.0) + 0.01;// (line_color.rgb - texture(noise, UV * 100.0).rgb * noiseAmount) * 
		ROUGHNESS = 0.0;
	}
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
