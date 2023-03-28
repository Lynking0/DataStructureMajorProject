#[compute]
#version 450

precision highp float;
precision highp int;

// 随机数生成部分
int prngState = 0;

void init_rand(int seed)
{
    // Seed the pseudo-random number generator
    prngState += seed;
}

float rand()
{
    int value;

    // Use a linear congruential generator (LCG) to update the state of the PRNG
    prngState *= 1103515245;
    prngState += 12345;
    value = (prngState >> 16) & 0x07FF;

    prngState *= 1103515245;
    prngState += 12345;
    value <<= 10;
    value |= (prngState >> 16) & 0x03FF;

    prngState *= 1103515245;
    prngState += 12345;
    value <<= 10;
    value |= (prngState >> 16) & 0x03FF;

    // Return the random value
    return value * (1.0 / 2147483648.0);
}
// -----------


// 噪声生成部分
layout(set = 0, binding = 0, std430) restrict buffer NoiseParams
{
    float Seed;
    float Frequency;
    float Amplitude;
    float Octaves;
    float PingPongStrength;
    float Lacunarity;
    float Gain;
    float BottomNumber;
} noise_params;

const int prime_x = 501125321;
const int prime_y = 1136930381;

int primed_hash(int seed, int xPrimed, int yPrimed) {
	int hash = seed ^ xPrimed ^ yPrimed;
	hash *= 0x27d4eb2d;
	return hash;
}

// 生成[-1,1)的伪随机数
float pseudorandom(int seed, int xPrimed, int yPrimed) {
	int hash = primed_hash(seed, xPrimed, yPrimed);
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

float linear_interpolation(float a, float b, float t) {
	return a + t * (b - a);
}

// 曲线是过(0, b)和(1, c)的三次曲线
float cubic_interpolation(float a, float b, float c, float d, float t) {
	float p = (d - c) - (a - b);
    return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
}

// 双三次插值
float bicubic_interpolation(int seed, float x, float y) {
	int x1 = int(floor(x));
	int y1 = int(floor(y));
	float xs = x - float(x1);
	float ys = y - float(y1);
	x1 *= prime_x;
	y1 *= prime_y;
	int x0 = x1 - prime_x;
	int y0 = y1 - prime_y;
	int x2 = x1 + prime_x;
	int y2 = y1 + prime_y;
	int x3 = x2 + prime_x;
	int y3 = y2 + prime_y;
	return cubic_interpolation(
            cubic_interpolation(
				pseudorandom(seed, x0, y0), 
				pseudorandom(seed, x1, y0), 
				pseudorandom(seed, x2, y0), 
				pseudorandom(seed, x3, y0),
                xs
			),
            cubic_interpolation(
				pseudorandom(seed, x0, y1), 
				pseudorandom(seed, x1, y1), 
				pseudorandom(seed, x2, y1), 
				pseudorandom(seed, x3, y1),
                xs),
            cubic_interpolation(
				pseudorandom(seed, x0, y2), 
				pseudorandom(seed, x1, y2), 
				pseudorandom(seed, x2, y2), 
				pseudorandom(seed, x3, y2),
                xs),
            cubic_interpolation(
				pseudorandom(seed, x0, y3), 
				pseudorandom(seed, x1, y3), 
				pseudorandom(seed, x2, y3), 
				pseudorandom(seed, x3, y3),
                xs),
            ys) * (1.0 / (1.5 * 1.5));
}

void get_noise(float _x, float _y) {
	// Place fragment code here.
	float x = _x * noise_params.Frequency;
	float y = _y * noise_params.Frequency;
	int seed = int(noise_params.Seed);
	
	float sum = 0.0;
	float amp = noise_params.Amplitude;
	for (int i = 0; i < noise_params.Octaves; ++i) {
        // seed++ —— 改变种子以生成不同噪声
		highp float singleNoise = bicubic_interpolation(seed++, x, y);
		// 使用pingpong函数进行标准化
		float normalizedNoise = pingpong((singleNoise + 1.0) * noise_params.PingPongStrength);
		// 叠加
        // (normalizedNoise - 0.5) * 2.0 —— 从[0, 1]映射到[-1, 1]
        // * amp —— amp越大，当前噪声对结果影响越大
		sum += (normalizedNoise - 0.5) * 2.0 * amp;
		
		x *= noise_params.Lacunarity;
		y *= noise_params.Lacunarity;
		amp *= noise_params.Gain;
	}
	
	float result = 
		(pow(noise_params.BottomNumber, -sum) - 1.0 / noise_params.BottomNumber)
		/ (noise_params.BottomNumber - 1.0 / noise_params.BottomNumber);
}
// -----------

layout(local_size_x = 10, local_size_y = 1, local_size_z = 1) in;

layout(set = 0, binding = 1, std430) restrict buffer VertexAX
{
    float data[];
} vertex_a_x;
layout(set = 0, binding = 2, std430) restrict buffer VertexAY
{
    float data[];
} vertex_a_y;
layout(set = 0, binding = 3, std430) restrict buffer VertexBX
{
    float data[];
} vertex_b_x;
layout(set = 0, binding = 4, std430) restrict buffer VertexBY
{
    float data[];
} vertex_b_y;
layout(set = 0, binding = 5, std430) restrict buffer VertexCX
{
    float data[];
} vertex_c_x;
layout(set = 0, binding = 6, std430) restrict buffer VertexCY
{
    float data[];
} vertex_c_y;
layout(set = 0, binding = 7, std430) restrict buffer VertexDX
{
    float data[];
} vertex_d_x;
layout(set = 0, binding = 8, std430) restrict buffer VertexDY
{
    float data[];
} vertex_d_y;
layout(set = 0, binding = 9, std430) restrict buffer Seeds
{
    int data[];
} seeds;
layout(set = 0, binding = 10, std430) restrict buffer Params
{
	float PI;
    float MaxVertexAltitude;
    float CtrlPointDistance;
} params;
layout(set = 0, binding = 11, std430) restrict buffer IsVaild
{
    float data[];
} is_vaild;

const float AttenuationRate = 0.997;
const float InitTemperature = 5000;
const float LowestTemperature = 2000;
const float MaxRejectTimes = 10;



void main()
{
	int gID = int(gl_GlobalInvocationID.x);
	vec2 a = vec2(vertex_a_x.data[gID], vertex_a_y.data[gID]);
	vec2 b = vec2(vertex_b_x.data[gID], vertex_b_y.data[gID]);
	vec2 c = vec2(vertex_c_x.data[gID], vertex_c_y.data[gID]);
	vec2 d = vec2(vertex_d_x.data[gID], vertex_d_y.data[gID]);
	init_rand(seeds.data[gID]);
	
	float status = rand();
	float energy = get_energy(status);
}
