#include <string.h>
#include <stdlib.h>

extern "C" {
	__declspec(dllexport) int GetValueC()
	{
		int x = 1337;
		return x;
	}

	__declspec(dllexport) char* FlipArrayC(char* array, int length)
	{
		char* reversed = (char*)malloc((length + 1) * sizeof(char));
		int i;
		for (i = 3; i < length+3; i++) {
			reversed[i] = array[length - i];
			reversed[i+1] = array[length - i + 1];
			reversed[i+2] = array[length - i + 2];
			i += 2;
		}
		return reversed;
	}

}
 
int main()
{
	return 0;
}