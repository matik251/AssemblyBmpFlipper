#include <iostream>

using namespace std;

extern "C"
{
	int GetValueFromASM();

	__declspec(dllexport) int GetIntASM()
	{
		int x = GetValueFromASM();
		cout << " ASM returns:" << x << endl;
		return x;
	}

	void _stdcall MyProc1(char* bmp, char* resbmp, int length);

	__declspec(dllexport) void GetValueAsm(char* bmp, char* resbmp, int length) 
	{
		MyProc1(bmp, resbmp, length);
	}
}


int main() 
{
	return 0;
}