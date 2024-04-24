shader_type spatial;
uniform float noiseAmount: hint_range(0.0, 1.0) = 0.08;
uniform vec4 base_color: source_color = vec4(0.537, 0.482, 0.447, 0.0);
uniform sampler2D noise;
uniform float scale: hint_range(0.0, 10.0) = 1.0;
varying vec3 pos;


void vertex() {
	// Called for every vertex the material is visible on.
	pos = VERTEX * scale;
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	
	ALBEDO = base_color.rgb;
	vec3 noiseAtUV = texture(noise, UV + NODE_POSITION_WORLD.xz).rgb;
	ALBEDO -= noiseAtUV * noiseAmount;
	
	
	
	
	
	
	// ALBEDO *= pow(randTilesPrev, 2.0);
	// ALBEDO *= pow(randTilesAhead, 2.0);
	// ALBEDO *= pow(randTilesAhead, 2.0) / pow(randTilesPrev, 2.0);
}

//void light() {s
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
