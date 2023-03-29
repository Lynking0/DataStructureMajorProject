#[compute]
#version 450

precision highp int;

layout(local_size_x = 10, local_size_y = 1, local_size_z = 1) in;

// 随机数生成部分
uint prng_state = 0;

void init_rand(uint seed)
{
    // Seed the pseudo-random number generator
    prng_state += seed;
}

float rand()
{
    uint value;

    // Use a linear congruential generator (LCG) to update the state of the PRNG
    prng_state *= 1103515245;
    prng_state += 12345;
    value = (prng_state >> 16) & 0x07FF;

    prng_state *= 1103515245;
    prng_state += 12345;
    value <<= 10;
    value |= (prng_state >> 16) & 0x03FF;

    prng_state *= 1103515245;
    prng_state += 12345;
    value <<= 10;
    value |= (prng_state >> 16) & 0x03FF;
    // Return the random value
    return value * (1.0 / 2147483648.0);
}

vec2 random_normal_distribution()
{
	float u = 0.0, v = 0.0, w = 0.0, c = 0.0;
	do
	{
		u = rand() * 2.0 - 1.0;
		v = rand() * 2.0 - 1.0;
		w = u * u + v * v;
	} while (w == 0.0 || w >= 1.0);
	c = sqrt((-2.0 * log(w)) / w);
	return vec2(u * c, v * c);
}

float normal_random(float mean, float deviation, float min_num, float max_num)
{
	while (true)
	{
		vec2 v = random_normal_distribution();
		float temp = v.x * deviation + mean;
		if (temp >= min_num && temp <= max_num)
			return temp;
		temp = v.y * deviation + mean;
		if (temp >= min_num && temp <= max_num)
			return temp;
	};
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

int primed_hash(int seed, int x_primed, int y_primed)
{
	int hash = seed ^ x_primed ^ y_primed;
	hash *= 0x27d4eb2d;
	return hash;
}

// 生成[-1,1)的伪随机数
float pseudorandom(int seed, int x_primed, int y_primed)
{
	int hash = primed_hash(seed, x_primed, y_primed);
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
float pingpong(float t)
{
	t -= float(int(t * 0.5) * 2);
	return t < 1.0 ? t : 2.0 - t;
}

float linear_interpolation(float a, float b, float t)
{
	return a + t * (b - a);
}

// 曲线是过(0, b)和(1, c)的三次曲线
float cubic_interpolation(float a, float b, float c, float d, float t)
{
	float p = (d - c) - (a - b);
    return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
}

// 双三次插值
float bicubic_interpolation(int seed, float x, float y)
{
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
                xs),
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

float get_noise(vec2 v)
{
	// Place fragment code here.
	float x = v.x * noise_params.Frequency;
	float y = v.y * noise_params.Frequency;
	int seed = int(noise_params.Seed);
	
	float sum = 0.0;
	float amp = noise_params.Amplitude;
	for (int i = 0; i < noise_params.Octaves; ++i)
	{
        // seed++ —— 改变种子以生成不同噪声
		float single_noise = bicubic_interpolation(seed++, x, y);
		// 使用pingpong函数进行标准化
		float normalized_noise = pingpong((single_noise + 1.0) * noise_params.PingPongStrength);
		// 叠加
        // (normalized_noise - 0.5) * 2.0 —— 从[0, 1]映射到[-1, 1]
        // * amp —— amp越大，当前噪声对结果影响越大
		sum += (normalized_noise - 0.5) * 2.0 * amp;
		
		x *= noise_params.Lacunarity;
		y *= noise_params.Lacunarity;
		amp *= noise_params.Gain;
	}
	
	return (pow(noise_params.BottomNumber, -sum) - 1.0 / noise_params.BottomNumber)
			/ (noise_params.BottomNumber - 1.0 / noise_params.BottomNumber);
}
// -----------

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
    uint data[];
} seeds;
layout(set = 0, binding = 10, std430) restrict buffer Params
{
    float MaxVertexAltitude;
    float CtrlPointDistance;
	float MaxGID;
} params;
layout(set = 0, binding = 11, std430) restrict buffer IsVaild
{
    float data[];
} is_vaild;

const float AttenuationRate = 0.997;
const float InitTemperature = 5000.0;
const float LowestTemperature = 2000.0;
const int MaxRejectTimes = 10;

vec2 A;
vec2 B;
vec2 C;
vec2 D;

float get_energy(float t)
{
	float _t = 1.0 - t;
	vec2 v = A * (_t * _t * _t) + B * (3.0 * _t * _t * t) + C * (3.0 * _t * t * t) + D * (t * t * t);
	return get_noise(v);
}

float get_near_status(float cur_status, float temperature)
{
	float range = 1.0 - exp(-10.0 * (temperature / InitTemperature - (LowestTemperature - 100.0) / InitTemperature));
	return normal_random(cur_status, range, 0.0, 1.0);
}

float get_accept_PR(float cur_energy, float next_energy, float temperature)
{
	if (next_energy > cur_energy)
        return 1.0;
    return exp((next_energy - cur_energy) / (temperature * (1.0 / 2400000.0)));
}

void main()
{
	int gID = int(gl_GlobalInvocationID.x);
	if (gID >= params.MaxGID)
		return;
	A = vec2(vertex_a_x.data[gID], vertex_a_y.data[gID]);
	B = vec2(vertex_b_x.data[gID], vertex_b_y.data[gID]);
	C = vec2(vertex_c_x.data[gID], vertex_c_y.data[gID]);
	D = vec2(vertex_d_x.data[gID], vertex_d_y.data[gID]);
	init_rand(seeds.data[gID]);
	
	float status = rand();
	float energy = get_energy(status);
	if (energy >= params.MaxVertexAltitude)
	{
		is_vaild.data[gID] = 0.0;
		return;
	}
	int reject_cnt = 0;
	for (float t = InitTemperature; ; t *= AttenuationRate)
	{
		if (t < LowestTemperature)
			t = LowestTemperature;
		float next_status = get_near_status(status, t);
		float next_energy = get_energy(next_status);
		if (rand() < get_accept_PR(energy, next_energy, t))
		{
			reject_cnt = 0;
			status = next_status;
			energy = next_energy;
			if (energy >= params.MaxVertexAltitude)
			{
				is_vaild.data[gID] = 0.0;
				return;
			}
		}
		else if (t == LowestTemperature)
		{
			if (reject_cnt < MaxRejectTimes)
				++reject_cnt;
			else
			{
				is_vaild.data[gID] = 1.0;
				return;
			}
		}
	}
}
