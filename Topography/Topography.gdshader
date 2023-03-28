shader_type canvas_item;

global uniform int Seed;
global uniform float Frequency;
global uniform float Amplitude;
global uniform int Octaves;
global uniform float PingPongStrength;
global uniform float Lacunarity;
global uniform float Gain;
global uniform float BottomNumber;
global uniform int LevelCnt;

const int primeX = 501125321;
const int primeY = 1136930381;

uniform float scale;
uniform vec2 transform;
uniform vec2 windowSize;

int primedHash(int seed, int xPrimed, int yPrimed) {
	int hash = seed ^ xPrimed ^ yPrimed;
	hash *= 0x27d4eb2d;
	return hash;
}

// 生成[-1,1)的伪随机数
float pseudorandom(int seed, int xPrimed, int yPrimed) {
	int hash = primedHash(seed, xPrimed, yPrimed);
	hash *= hash;
	hash ^= hash << 19;
	return float(hash) * (1.0 / 2147483648.0);
}

// 1       ^       ^
//        / \     / \
//       /   \   /   \
//      /     \ /     \
// 0   /       v       \
//     0   1   2   3   4 …
float pingpong(float t) {
	t -= float(int(t * 0.5) * 2);
	return t < 1.0 ? t : 2.0 - t;
}

float linearInterpolation(float a, float b, float t) {
	return a + t * (b - a);
}

// 曲线是过(0, b)和(1, c)的三次曲线
float cubicInterpolation(float a, float b, float c, float d, float t) {
	float p = (d - c) - (a - b);
    return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
}

// 双三次插值
float bicubicInterpolation(int seed, float x, float y) {
	int x1 = int(floor(x));
	int y1 = int(floor(y));
	float xs = x - float(x1);
	float ys = y - float(y1);
	x1 *= primeX;
	y1 *= primeY;
	int x0 = x1 - primeX;
	int y0 = y1 - primeY;
	int x2 = x1 + primeX;
	int y2 = y1 + primeY;
	int x3 = x2 + primeX;
	int y3 = y2 + primeY;
	return cubicInterpolation(
            cubicInterpolation(
				pseudorandom(seed, x0, y0), 
				pseudorandom(seed, x1, y0), 
				pseudorandom(seed, x2, y0), 
				pseudorandom(seed, x3, y0),
                xs
			),
            cubicInterpolation(
				pseudorandom(seed, x0, y1), 
				pseudorandom(seed, x1, y1), 
				pseudorandom(seed, x2, y1), 
				pseudorandom(seed, x3, y1),
                xs),
            cubicInterpolation(
				pseudorandom(seed, x0, y2), 
				pseudorandom(seed, x1, y2), 
				pseudorandom(seed, x2, y2), 
				pseudorandom(seed, x3, y2),
                xs),
            cubicInterpolation(
				pseudorandom(seed, x0, y3), 
				pseudorandom(seed, x1, y3), 
				pseudorandom(seed, x2, y3), 
				pseudorandom(seed, x3, y3),
                xs),
            ys) * (1.0 / (1.5 * 1.5));
}

float discretize(float x) {
	return float(int(x * float(LevelCnt))) / float(LevelCnt);
}

void fragment() {
	// Place fragment code here.
	vec2 pos = (VERTEX - transform - windowSize / 2.0) / scale;
	float x = pos.x * Frequency;
	float y = pos.y * Frequency;
	int seed = Seed;
	
	float sum = 0.0;
	float amp = Amplitude;
	for (int i = 0; i < Octaves; ++i) {
        // seed++ —— 改变种子以生成不同噪声
		highp float singleNoise = bicubicInterpolation(seed++, x, y);
		// 使用pingpong函数进行标准化
		float normalizedNoise = pingpong((singleNoise + 1.0) * PingPongStrength);
		// 叠加
        // (normalizedNoise - 0.5) * 2.0 —— 从[0, 1]映射到[-1, 1]
        // * amp —— amp越大，当前噪声对结果影响越大
		sum += (normalizedNoise - 0.5) * 2.0 * amp;
		
		x *= Lacunarity;
		y *= Lacunarity;
		amp *= Gain;
	}
	
	float result = 
		(pow(BottomNumber, -sum) - 1.0 / BottomNumber)
		/ (BottomNumber - 1.0 / BottomNumber);
	if (LevelCnt > 0) 
		result = discretize(result);
	COLOR = vec4(
		linearInterpolation(205.0, 255.0, result) / 256.0, 
		linearInterpolation(218.0, 123.0, result) / 256.0, 
		linearInterpolation(134.0, 0.0, result) / 256.0, 
	1.0);
}
