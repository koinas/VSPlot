
#include <math.h>
#include <stdlib.h>

char* char_test()
{
	const int N = 1024;
	char* data = new char[N];
	for(int i = 0;i<N;++i)
	data[i] = i;
	return data;
}

unsigned char* unsigned_char_test()
{
	const int N = 1024;
	unsigned char* data = new unsigned char[N];
	for(int i = 0;i<N;++i)
	data[i] = i;
	return data;
}

short* short_test()
{
	const int N = 131072;
	short* data = new short[N];
	for(int i = 0;i<N;++i)
	data[i] = i;
	return data;
}

unsigned short* unsigned_short_test()
{
	const int N = 131072;
	unsigned short* data = new unsigned short[N];
	for(int i = 0;i<N;++i)
	data[i] = i;
	return data;
}


int* int_test()
{
	const __int64 N = 1024;
	int* data = new int[N];
	for(int i = 0;i<N;++i)
	data[i] = i*i*i*i;
	return data;
}

unsigned int* unsigned_int_test()
{
	const int N = 1024;
	unsigned int* data = new unsigned int[N];
	for(int i = 0;i<N;++i)
	data[i] = i*i*i*i;
	return data;
}

__int64* int64_test()
{
	const __int64 N = 4096;
	__int64* data = new __int64[N];
	for(int i = 0;i<N;++i)
	data[i] = (__int64)i*i*i*i*i*i*i;
	return data;
}

unsigned __int64* unsigned_int64_test()
{
	const int N = 4096;
	unsigned __int64* data = new unsigned __int64[N];
	for(int i = 0;i<N;++i)
	data[i] = (__int64)i*i*i*i*i*i*i;
	return data;
}

float* float_test()
{
	const int N = 4096;
	float* data = new float[N];
	for(int i = 0;i<N;++i)
	data[i] = (float)i*i*i*i*i*i*i;
	data[1024] = (float)4096*4096*4096*4096*4096*4096;
	return data;
}

double* double_test()
{
	const float N = 4096;
	double* data = new double[N];
	for(int i = 0;i<N;++i)
	data[i] = (float)i*i*i*i*i*i*i;
	return data;
}

template <typename T> T* normal_dst(double u,double s,int point_num)
{
	T* normal_data = new T[point_num];
	const T a1 = 1/(s*sqrt(2.f*3.1415926535));
	for(int i = 0;i<point_num;++i)
		normal_data[i] = a1*exp(-(i-u) *(i-u)/2.f/(s*s));
	return normal_data;	
}

template <typename T> T* noise(float A,int point_num)
{
	T* random_data = new T[point_num];	
	for(int i = 0;i<point_num;++i)
		random_data[i] = (T)rand()/RAND_MAX*A;
	return random_data;	
}

template <typename T> T* saw_signal(int point_num)
{
	T* normal_data = new T[point_num];
	for(int i = 0;i<point_num;++i)
		normal_data[i] = i;
	return normal_data;	
}


template <typename T> T* pulse_signal(int point_num)
{
	const int pulse_num = 100;
	const int N_pulse = point_num / pulse_num;
	T* pulse = new T[point_num];
	for(int i = 0;i<point_num;++i)
		pulse[i] = (float)((i % N_pulse) >= (N_pulse / 2) ? 0:1);
	return pulse;	
}


template <typename T> T* sine_signal(int point_num,int period_num)
{
	const int point_per = point_num / period_num;
	T* pulse = new T[point_num];
	for(int i = 0;i<point_num;++i)
		pulse[i] = (T)(sin((i%point_per)/(float)point_per*2*3.1415926));
	return pulse;	
}

void test_all()
{
	char* char_data = char_test();
	unsigned char* unsigned_char_data = unsigned_char_test();
	short* short_data = short_test();
	unsigned short* unsigned_short_data = unsigned_short_test();
	int* int_data = int_test();
	unsigned int* unsigned_int_data = unsigned_int_test();
	__int64* int64_data = int64_test();
	unsigned __int64* unsigned_int64_data = unsigned_int64_test();
	float* float_data = float_test();
	double* double_data = double_test();
	
	float* normal_distribution = normal_dst<float>(2048,100,4096);
	float* noise_data = noise<float>(100,1024);

	float signal[256];
	char* p = (char*)signal;

}