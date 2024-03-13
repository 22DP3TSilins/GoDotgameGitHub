shader_type spatial;
uniform float noiseAmount: hint_range(0.0, 1.0) = 0.08;
uniform vec4 base_color: source_color = vec4(0.537, 0.482, 0.447, 0.0);
uniform sampler2D noise;
uniform float tiling: hint_range(0.0, 0.1) = 0.0;
uniform float lineSize: hint_range(0.0, 0.1) = 0.0;
uniform float scaleOfNode: hint_range(10.0, 100.0) = 10.0;
varying vec3 pos;


void vertex() {
	// Called for every vertex the material is visible on.
	pos = VERTEX;
}

void fragment() {
	// Called for every pixel the material is visible on.
	float scaleOfNodeDiv10 = scaleOfNode / 10.0;
	
	vec2 UVScaled = UV * scaleOfNode;
	
	// float xCoorInTile = mod(UVScaled.x, 1.0);
	// float invXCoorInTile = (1.0 - xCoorInTile) / scaleOfNode;
	
	float noiseY = round(UVScaled.y) / scaleOfNode;
	float noiseX = round(UVScaled.x) / scaleOfNode;
	float tileSize = 0.1;
	float noiseXPrev = round(UVScaled.x + 1.0) / scaleOfNode;
	float noiseXAhead = round(UVScaled.x - 1.0) / scaleOfNode;
	
	float randTiles = texture(noise, vec2(noiseX, noiseY)).r;
	float randTilesPrev = texture(noise, vec2(noiseXPrev, noiseY)).r;
	float randTilesAhead = texture(noise, vec2(noiseXAhead, noiseY)).r;
	
	float lineSizeScaled = lineSize / scaleOfNodeDiv10;
	
	float tilingX = mod(UV.x * randTiles, tiling / scaleOfNodeDiv10);
	float tilingY = mod(UV.y + 0.252 / scaleOfNodeDiv10, tiling / scaleOfNodeDiv10);
	float lineSizeRandTiles = lineSize * randTiles / scaleOfNodeDiv10;
	
	float tilingXAhead = mod(UV.x * randTilesAhead, tiling / scaleOfNodeDiv10);
	float lineSizeRandTilesAhead = lineSize * randTilesAhead  / scaleOfNodeDiv10;
	
	float tilingXPrev = mod(UV.x * randTilesPrev, tiling / scaleOfNodeDiv10);
	float lineSizeRandTilesPrev = lineSize * randTilesPrev  / scaleOfNodeDiv10;
	
	ALBEDO = base_color.rgb;
	vec3 noiseAtUV = texture(noise, UV * scaleOfNodeDiv10).rgb;
	ALBEDO -= noiseAtUV * noiseAmount;
	
	
	if (tilingX <= lineSizeRandTiles || tilingY <= lineSizeScaled || tilingXPrev <= lineSizeRandTilesPrev || tilingXAhead <= lineSizeRandTilesAhead) {
		float normLineX = tilingX / lineSizeRandTiles;
		float normLineY = tilingY / lineSizeScaled;
		
		float normLineXAhead = tilingXAhead / lineSizeRandTilesAhead;
		float normLineXPrev = tilingXPrev / lineSizeRandTilesPrev;
		
		float powKoef = 0.6;
		
		float xDark = (1.0 - pow(sin(normLineX * PI), powKoef)) / 10.0;
		float yDark = (1.0 - pow(sin(normLineY * PI), powKoef)) / 10.0;
		float xDarkPrev = (1.0 - pow(sin(normLineXPrev * PI), powKoef)) / 10.0;
		float xDarkAhead = (1.0 - pow(sin(normLineXAhead * PI), powKoef)) / 10.0;
		bool a = false;
		if (tilingXPrev <= lineSizeRandTilesPrev && tilingY <= lineSizeScaled) {
			ALBEDO *= min(xDarkPrev, yDark);
			// ALBEDO.r = 1.0;
			a = true;
			
		} else if (tilingXPrev <= lineSizeRandTilesPrev) {
			ALBEDO *= xDarkPrev;
			// ALBEDO.r = 1.0;
			a= true;
		}
		if (tilingXAhead <= lineSizeRandTilesAhead && tilingY <= lineSizeScaled) {
			ALBEDO *= min(xDarkAhead, yDark);
			// ALBEDO.b = 1.0;
			
		} else if (tilingXAhead <= lineSizeRandTilesAhead) {
			ALBEDO *= xDarkAhead;
			// ALBEDO.b = 1.0;
			
		} else if (a) {
		
		} else if (tilingXPrev <= lineSizeRandTilesPrev * 3.0 && tilingY > lineSizeScaled) {
		} else if (tilingXAhead <= lineSizeRandTilesAhead * 3.0 && tilingY > lineSizeScaled) {
			
		} else if (tilingXPrev <= lineSizeRandTilesPrev * 3.0 && tilingY <= lineSizeScaled) {
			ALBEDO *= yDark;
			// ALBEDO.r = 1.0;
			
		} else if (tilingXAhead <= lineSizeRandTilesAhead * 3.0 && tilingY <= lineSizeScaled) {
			ALBEDO *= yDark;
			// ALBEDO.b = 1.0;
			
		} else if (tilingX <= lineSizeRandTiles && tilingY <= lineSizeScaled) {
			ALBEDO *= min(xDark, yDark);
			// ALBEDO.g = 1.0;
		
		} else if (tilingX <= lineSizeRandTiles) {
			ALBEDO *= xDark;
			// ALBEDO.g = 1.0;
			
		} else if (tilingY <= lineSizeScaled) {
			ALBEDO *= yDark;
		}
	}
	
	
	
	// ALBEDO *= pow(randTilesPrev, 2.0);
	// ALBEDO *= pow(randTilesAhead, 2.0);
	// ALBEDO *= pow(randTilesAhead, 2.0) / pow(randTilesPrev, 2.0);
}

//void light() {s
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
