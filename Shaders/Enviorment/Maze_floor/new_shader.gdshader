shader_type spatial;
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
	
	vec2 UVScaled = UV * scale;
	UVScaled.x *= 2.0;
	
	// float xCoorInTile = mod(UVScaled.x, 1.0);
	// float invXCoorInTile = (1.0 - xCoorInTile) / scaleOfNode;
	
	float tileYpos = round(UVScaled.y) / scale;
	float yCoorInTile = mod(UV.y + 0.02 / scale, 0.5 / scale) / 0.4;
	
	float tileXpos;
	float xCoorInTile;
	if (mod(UV.y, 1.0 / scale) < 0.5 / scale) {
		tileXpos = round(UVScaled.x + 5.0 / scale) / scale;
		xCoorInTile = mod(UV.x + 0.5 / scale, 1.0 / scale);
	} else {
		tileXpos = round(UVScaled.x) / scale;
		xCoorInTile = mod(UV.x, 1.0 / scale);
	}
	
	// float yCoorInTile = mod(UV.y, 1.0 / scale);
	float randNoiseTile = texture(noise, vec2(tileXpos, tileYpos) * PI).r;
	float lineSizeScaled = lineSize / scale;
	
	// float distFromLineCenterXnorm = xCoorInTile * scale * randNoiseTile;
	float distFromLineCenterXnorm = (xCoorInTile * scale + randNoiseTile) * 0.25 + 0.25;
	
	
	ALBEDO = base_color.rgb;
	ALBEDO -= texture(noise, UV * scale / 10.0).rgb * noiseAmount;
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
