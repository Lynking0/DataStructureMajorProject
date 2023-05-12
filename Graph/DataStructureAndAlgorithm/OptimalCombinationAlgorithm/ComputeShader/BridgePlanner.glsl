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

// 0~1
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

// uint范围
uint randi()
{
    uint value;

    // Use a linear congruential generator (LCG) to update the state of the PRNG
    prng_state *= 1103515245;
    prng_state += 12345;
    value = (prng_state >> 16) & 0x0FFF;

    prng_state *= 1103515245;
    prng_state += 12345;
    value <<= 10;
    value |= (prng_state >> 16) & 0x03FF;

    prng_state *= 1103515245;
    prng_state += 12345;
    value <<= 10;
    value |= (prng_state >> 16) & 0x03FF;
    // Return the random value
    return value;
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

vec2 linear_interpolation(vec2 a, vec2 b, float t)
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
layout(set = 0, binding = 5, std430) restrict buffer VertexACtrlX
{
    float data[];
} vertex_a_ctrl_x;
layout(set = 0, binding = 6, std430) restrict buffer VertexACtrlY
{
    float data[];
} vertex_a_ctrl_y;
layout(set = 0, binding = 7, std430) restrict buffer VertexBCtrlX
{
    float data[];
} vertex_b_ctrl_x;
layout(set = 0, binding = 8, std430) restrict buffer VertexBCtrlY
{
    float data[];
} vertex_b_ctrl_y;
layout(set = 0, binding = 9, std430) restrict buffer Seeds
{
    uint data[];
} seeds;
layout(set = 0, binding = 10, std430) restrict buffer Params
{
    float MaxVertexAltitude;
    float CtrlPointDistance;
	float MaxGID;
	float MinX;
	float MinY;
	float MaxX;
	float MaxY;
} params;
layout(set = 0, binding = 11, std430) restrict buffer PosX
{
    float data[];
} pos_x;
layout(set = 0, binding = 12, std430) restrict buffer PosY
{
    float data[];
} pos_y;
layout(set = 0, binding = 13, std430) restrict buffer CtrlOffsetX
{
    float data[];
} ctrl_offset_x;
layout(set = 0, binding = 14, std430) restrict buffer CtrlOffsetY
{
    float data[];
} ctrl_offset_y;

vec2 A;
vec2 B;
vec2 ACtrl;
vec2 BCtrl;
vec2 CentralPosition;
float CtrlPointDist;
float MaxSemiMajorAxis;
float MaxSemiMinorAxis;

struct IndividualInfo
{
	uint individual;
	float fitness;
};
struct BridgeData
{
	vec2 pos;
	vec2 ctrlOffset;
};

IndividualInfo BestIndividual;

#define PopulationSize 20
#define MaxStagesPower 16
IndividualInfo Population[PopulationSize];
const int MaxIterTimes = 100;
const float CrossProb = 0.99;
const float MutateProb = 0.1;
const int MaxUnchangedTimes = 10;
const int ToleranceLength = 5;
const int RadiusBitCount = 12;
const uint RadiusSelector = 0xFFF00000;
const int RadianBitCount = 12;
const uint RadianSelector = 0x000FFF00;
const int DirectionBitCount = 8;
const uint DirectionSelector = 0x000000FF;
const float Tau = 6.2831855;
const float Pi = 3.1415927;
const float PAngleLimited = Pi * 5 / 180;

float cross(vec2 a, vec2 b)
{
	return (a.x * b.y) - (a.y * b.x);
}

float Angle(vec2 v)
{
	return atan(v.y, v.x);
}

float AngleTo(vec2 from, vec2 to)
{
	return atan(cross(from, to), dot(from, to));
}

vec2 Rotated(vec2 v, float angle)
{
	float sine = sin(angle);
	float cosi = cos(angle);
	return vec2(
		v.x * cosi - v.y * sine,
		v.x * sine + v.y * cosi);
}

vec2 vertices[34];
struct StackInfo
{
	float minT;
	float maxT;
	int depth;
	bool isLeftFinish;
} stack[16];

void __getBrokenLinePoints(vec2 a, vec2 b, vec2 c, vec2 d, inout int verticesSize)
{
	int stackIndex = 0; // 记录栈顶索引
	stack[0] = StackInfo(0, 1, 1, false);
	
	while (stackIndex >= 0)
	{
		int newDepth = stack[stackIndex].depth << 1;
		float minT = stack[stackIndex].minT;
		float maxT = stack[stackIndex].maxT;
		float midT = (minT + maxT) * 0.5;
		if (stack[stackIndex].isLeftFinish)
		{
			float _minT = 1 - minT;
			float _midT = 1 - midT;
			float _maxT = 1 - maxT;

			vec2 start = a * (_minT * _minT * _minT) + b * (3 * _minT * _minT * minT) + c * (3 * _minT * minT * minT) + d * (minT * minT * minT);
			vec2 middle = a * (_midT * _midT * _midT) + b * (3 * _midT * _midT * midT) + c * (3 * _midT * midT * midT) + d * (midT * midT * midT);
			vec2 end = a * (_maxT * _maxT * _maxT) + b * (3 * _maxT * _maxT * maxT) + c * (3 * _maxT * maxT * maxT) + d * (maxT * maxT * maxT);

			vec2 normalA = normalize((middle - start));
			vec2 normalB = normalize((end - middle));

			// 角度过大则进行分段
			if (dot(normalA, normalB) < cos(PAngleLimited))
				vertices[verticesSize++] = middle;

			if (newDepth < MaxStagesPower)
				stack[stackIndex] = StackInfo(midT, maxT, newDepth, false);
			else
				--stackIndex;
		}
		else
		{
			stack[stackIndex].isLeftFinish = true;
			if (newDepth < MaxStagesPower)
				stack[++stackIndex] = StackInfo(minT, midT, newDepth, false);
		}
	}
}

float TessellateEvenLengthAndCalc(vec2 a, vec2 b, vec2 c, vec2 d, vec2 e, vec2 f, vec2 g)
{
	int verticesSize = 0;
	vertices[verticesSize++] = a;
	__getBrokenLinePoints(a, b, c, d, verticesSize);
	vertices[verticesSize++] = d;
	__getBrokenLinePoints(d, e, f, g, verticesSize);
	vertices[verticesSize++] = g;
	float result = 0;
	float dist = 0;
	for (int i = 1; i < verticesSize; ++i)
	{
		float segmentLen = distance(vertices[i - 1], vertices[i]);
		for (; dist < segmentLen; dist += ToleranceLength)
		{
			vec2 v = linear_interpolation(vertices[i - 1], vertices[i], dist / segmentLen);
			float temp = get_noise(v);
			result += temp * temp;
		}
		dist -= segmentLen;
	}
	return 1 / result;
}

uint GetFullOneNumbers(int x)
{
	return 0xFFFFFFFF >> (32 - x);
}

BridgeData ConvertToData(uint individual)
{
	uint _radius = (individual & RadiusSelector) >> (RadianBitCount + DirectionBitCount);
	uint _radian = (individual & RadianSelector) >> DirectionBitCount;
	uint _direction = individual & DirectionSelector;

	float semiMajorAxis = _radius * MaxSemiMajorAxis / float(1u << RadiusBitCount);
	float semiMinorAxis = _radius * MaxSemiMinorAxis / float(1u << RadiusBitCount);
	float radian = _radian * Tau / float(1u << RadianBitCount);
	vec2 posOffset = vec2(semiMinorAxis * cos(radian), semiMajorAxis * sin(radian));
	posOffset = Rotated(posOffset, Angle(B - A));
	vec2 pos = CentralPosition + posOffset;
	if (pos.x < params.MinX + params.CtrlPointDistance)
		pos.x = 2 * (params.MinX + params.CtrlPointDistance) - pos.x;
	if (pos.x > params.MaxX - params.CtrlPointDistance)
		pos.x = 2 * (params.MaxX - params.CtrlPointDistance) - pos.x;
	if (pos.y < params.MinY + params.CtrlPointDistance)
		pos.y = 2 * (params.MinY + params.CtrlPointDistance) - pos.y;
	if (pos.y > params.MaxY - params.CtrlPointDistance)
		pos.y = 2 * (params.MaxY - params.CtrlPointDistance) - pos.y;
	
	float direction = ((float(_direction) / float(1u << DirectionBitCount)) * 0.6 - 0.3) * Pi;
	vec2 ctrlOffset = vec2(CtrlPointDist * cos(direction), CtrlPointDist * sin(direction));
	ctrlOffset = Rotated(ctrlOffset, Angle(B - A));
	if (abs(AngleTo(ctrlOffset, A - pos)) >= Pi / 2)
		ctrlOffset = -ctrlOffset;
	return BridgeData(pos, ctrlOffset);
}

float CalcFitness(uint individual)
{
	BridgeData bData = ConvertToData(individual);
	float ans = TessellateEvenLengthAndCalc(
		A,
		ACtrl,
		bData.pos + bData.ctrlOffset,
		bData.pos,
		bData.pos - bData.ctrlOffset,
		BCtrl,
		B
	);
	if (ans > BestIndividual.fitness)
		BestIndividual = IndividualInfo(individual, ans);
	return ans;
}

void InitPopulation()
{
	for (int i = 0; i < PopulationSize; ++i)
	{
		Population[i].individual = randi();
		Population[i].fitness = CalcFitness(Population[i].individual);
	}
}

float cumulativeFitness[PopulationSize];

void Select(out uint individual1, out uint individual2)
{
	// 计算累计适应度
	cumulativeFitness[0] = Population[0].fitness;
	for (int i = 1; i < PopulationSize; ++i)
		cumulativeFitness[i] = cumulativeFitness[i - 1] + Population[i].fitness;
	// 转轮盘
	float value = rand() * cumulativeFitness[PopulationSize - 1];
	for (int i = 0; ; ++i)
	{
		if (value < cumulativeFitness[i])
		{
			individual1 = Population[i].individual;
			// 对应个体概率减为0，防止重复选择
			for (int j = i; j < PopulationSize; ++j)
				cumulativeFitness[j] -= Population[i].fitness;
			break;
		}
	}
	// 再转一次
	value = rand() * cumulativeFitness[PopulationSize - 1];
	for (int i = 0; ; ++i)
	{
		if (value < cumulativeFitness[i])
		{
			individual2 = Population[i].individual;
			break;
		}
	}
}

void Cross(inout uint individual1, inout uint individual2)
{
	int startPos = int((randi() & 31) + 1);
	int endPos = int((randi() & 31) + 1);
	if (startPos > endPos)
	{
		int temp__ = startPos;
		startPos = endPos;
		endPos = temp__;
	}
	uint sliceSelector = GetFullOneNumbers(startPos - 1) ^ GetFullOneNumbers(endPos);
	// 交换对应段
	uint a1 = individual1 & sliceSelector;
	uint a2 = individual2 & ~sliceSelector;
	uint b1 = individual2 & sliceSelector;
	uint b2 = individual1 & ~sliceSelector;
	individual1 = a1 | a2;
	individual2 = b1 | b2;
}

void Mutate(inout uint individual)
{
	individual ^= 1u << (randi() & 31);
}

float lastFitness;
float maintainTimes;

bool CanEndEarly()
{
	if (BestIndividual.fitness != lastFitness)
	{
		lastFitness = BestIndividual.fitness;
		maintainTimes = 0;
	}
	else
	{
		++maintainTimes;
		if (maintainTimes > MaxUnchangedTimes)
			return true;
	}
	return BestIndividual.fitness > ToleranceLength / (distance(A, B) * 1.15 * params.MaxVertexAltitude * params.MaxVertexAltitude * 1.5);
}

IndividualInfo Subpopulation[PopulationSize];

void main()
{
	int gID = int(gl_GlobalInvocationID.x);
	if (gID >= params.MaxGID)
		return;
	A = vec2(vertex_a_x.data[gID], vertex_a_y.data[gID]);
	B = vec2(vertex_b_x.data[gID], vertex_b_y.data[gID]);
	ACtrl = vec2(vertex_a_ctrl_x.data[gID], vertex_a_ctrl_y.data[gID]);
	BCtrl = vec2(vertex_b_ctrl_x.data[gID], vertex_b_ctrl_y.data[gID]);
	CentralPosition = (A + B) * 0.5;
	float temp__ = distance(A, B);
	CtrlPointDist = temp__ * 0.25;
	MaxSemiMajorAxis = temp__ * 0.45;
	MaxSemiMinorAxis = temp__ * 0.15;
	init_rand(seeds.data[gID]);

	BestIndividual = IndividualInfo(0, -1);
	lastFitness = -1;
	maintainTimes = 0;
	InitPopulation();
	for (int i = 0; i < MaxIterTimes; ++i)
	{
		if (CanEndEarly())
			break;
		for (int idx = 0; idx < PopulationSize;)
		{
			uint individual1, individual2;
			Select(individual1, individual2);
			if (rand() < CrossProb)
				Cross(individual1, individual2);
			if (rand() < MutateProb)
				Mutate(individual1);
			if (rand() < MutateProb)
				Mutate(individual2);
			Subpopulation[idx++] = IndividualInfo(individual1, CalcFitness(individual1));
			Subpopulation[idx++] = IndividualInfo(individual2, CalcFitness(individual2));
		}
		for (int j = 0; j < PopulationSize; ++j)
			Population[j] = Subpopulation[j];
	}
	BridgeData bData = ConvertToData(BestIndividual.individual);
	pos_x.data[gID] = bData.pos.x;
	pos_y.data[gID] = bData.pos.y;
	ctrl_offset_x.data[gID] = bData.ctrlOffset.x;
	ctrl_offset_y.data[gID] = bData.ctrlOffset.y;
}
