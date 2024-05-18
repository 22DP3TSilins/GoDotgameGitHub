shader_type spatial;
render_mode world_vertex_coords;

uniform float noiseAmount: hint_range(0.0, 1.0) = 0.08;
uniform vec4 base_color: source_color = vec4(0.537, 0.482, 0.447, 0.0);
uniform sampler2D noise;
uniform float xTiling;
uniform float yTiling;
uniform float scale;
uniform float lineSize;
varying vec3 pos;


void vertex() {
	// Called for every vertex the material is visible on.
	pos = VERTEX;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 a = vec2(pos.x + pos.z, pos.y);
	vec2 UVScaled = a * scale;
	UVScaled.x *= 2.0;
	vec2 pos2 = a;
	
	// float xCoorInTile = mod(UVScaled.x, 1.0);
	// float invXCoorInTile = (1.0 - xCoorInTile) / scaleOfNode;
	
	float tileYpos = round(UVScaled.y) / scale;
	float yCoorInTile = mod(pos2.y + 0.02 / scale, 0.5 / scale) / 0.4;
	
	float tileXpos;
	float xCoorInTile;
	if (mod(pos.y, 1.0 / scale) < 0.5 / scale) {
		tileXpos = round(UVScaled.x + 5.0 / scale) / scale;
		xCoorInTile = mod(pos2.x + 0.5 / scale, 1.0 / scale);
	} else {
		tileXpos = round(UVScaled.x) / scale;
		xCoorInTile = mod(pos2.x, 1.0 / scale);
	}
	
	// float yCoorInTile = mod(UV.y, 1.0 / scale);
	float randNoiseTile = texture(noise, vec2(tileXpos, tileYpos) * PI).r;
	float lineSizeScaled = lineSize / scale;
	
	// float distFromLineCenterXnorm = xCoorInTile * scale * randNoiseTile;
	float distFromLineCenterXnorm = (xCoorInTile * scale + randNoiseTile) * 0.25 + 0.25;
	
	
	ALBEDO = base_color.rgb;
	ALBEDO -= texture(noise, pos2.xy * scale / 10.0).rgb * noiseAmount;
	
	float normal = texture(noise, pos2.xy * scale / 10.0).r;
	
	float normalxm1 = texture(noise, vec2(pos2.x - 1.0, pos2.y) * scale / 10.0).r;
	float normalxp1 = texture(noise, vec2(pos2.x + 1.0, pos2.y) * scale / 10.0).r;
	
	float normalym1 = texture(noise, vec2(pos2.x, pos2.y - 1.0) * scale / 10.0).r;
	float normalyp1 = texture(noise, vec2(pos2.x, pos2.y + 1.0) * scale / 10.0).r;
	
	float normalx = (normalxm1 - normalxp1) / 4.0 + normal;
	NORMAL_MAP.x -= normalx * 0.5;
	// NORMAL_MAP.x -= texture(noise, pos2.xy * scale / 10.0).rgb * 0.8;
	// ALBEDO *= distFromLineCenterXnorm;
	//float a = 0.5;
	//ALBEDO = vec3(0.0, 0.0, 0.0);
	//if (distFromLineCenterXnorm > a) {
		//ALBEDO.r = distFromLineCenterXnorm;
	//} else if (distFromLineCenterXnorm < a) {
		//ALBEDO.g = distFromLineCenterXnorm;
	//} else {
		//ALBEDO.b = distFromLineCenterXnorm;
	//}
	// ALBEDO *= distFromLineCenterXnorm;
	
	//ALBEDO *= yDark;
	//ALBEDO *= xDark;
	
	
	if (((distFromLineCenterXnorm > 0.5 - lineSizeScaled) && (distFromLineCenterXnorm < 0.5 + lineSizeScaled)) || ((yCoorInTile < lineSizeScaled))) {
		float normLineX = distFromLineCenterXnorm / 0.75 / (lineSizeScaled * 4.68) + 0.25;
		float normLineY = yCoorInTile / lineSizeScaled;
		
		float powKoef = 0.6;
		
		float xDark = (1.0 - pow(sin(normLineX * PI), powKoef)) / 10.0;
		float yDark = (1.0 - pow(sin(normLineY * PI), powKoef)) / 10.0;
		
		
		// ALBEDO = vec3(0.0, 0.0, 0.0);
		
		if (distFromLineCenterXnorm > 0.5 - lineSizeScaled && distFromLineCenterXnorm < 0.5 + lineSizeScaled && yCoorInTile < lineSizeScaled) {
			ALBEDO *= min(xDark, yDark);
			// ALBEDO.g = 1.0;
		
		} else if (distFromLineCenterXnorm > 0.5 - lineSizeScaled && distFromLineCenterXnorm < 0.5 + lineSizeScaled) {
			ALBEDO *= xDark;
			// ALBEDO.g = 1.0;
			
		} else {
			ALBEDO *= yDark;
		}
	}
	
	
	//float tilingX = mod(UV * randTiles, scale);
	//float tilingY = mod(UV.y + 0.252 / scale, scale);
	//float lineSizeRandTiles = lineSize * randTiles / scale;
	
	// ALBEDO = vec3(normLineX);
	
	
	
}

//void light() {s
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
