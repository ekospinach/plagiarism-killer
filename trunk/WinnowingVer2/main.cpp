/*
Date: 2010.3.27
Author: All staff
Description: This is the second implementation of winnowing algorithm. Wstring is employeed
instead of string to support Chinese characters.
*/


#include <iostream>
#include <sstream>
#include <fstream>
#include <vector>
#include <string>
#include <ctime>
#include <map>
using namespace std;

const char * const fileList = ".\\source0.txt";
const char * const targetFile = ".\\target0.txt";


int k = 15;
int w = 40;
int t = w + k - 1;
int totHN = 0;
map <wstring, int> hashValue;

typedef struct{
	vector<unsigned int> hashKey;
	vector<unsigned int> position;
	wstring file;
	vector<int> zoneStart, zoneEnd;
} info;

wstring preProcess(string fileName)
{
	wifstream input(fileName.c_str());  //use unicode version of ifstream
	input.imbue(locale("chs"));
	if (!input){
		cout << "Bad input: " << fileName << endl;
		exit(-1);
	}

	wchar_t ch;
	wstring temp;
	while (input.get(ch))
		temp += ch;

	wstring result;
	for (int i = 0 ; i < temp.size() ; ++i){
		wchar_t x = temp[i];
		if (L'A' <= x && x <=L'Z')  // an upper-case English letter
			result += towlower(x);            //convert it to lower-case letter
		if (( L'a' <= x && x <= L'z') ||  // an lower-case letter
			(0x4E00 <= x && x <= 0x9FBF))  //Chinese, Japanese, Korean
			result += x;
	}
	return result;
}

double compare(info & user, info & base)
{
	int pStart=0,nStart,same=0;
	for(int i=0;i<user.hashKey.size();i++)
	{
		nStart=pStart;
		while((nStart<base.hashKey.size())&&
			(user.hashKey[i]>base.hashKey[nStart])) nStart++;
		if(nStart==base.hashKey.size()) break;
		pStart=nStart;
		if(user.hashKey[i]==base.hashKey[nStart])
		{
			same++;
			bool isInclude = false;
			for(int k=0; k<base.zoneStart.size(); k++)
			{
				if (base.position[nStart]>=base.zoneStart[k] &&
					base.position[nStart]<=base.zoneEnd[k])
				{
					isInclude = true;
					break;
				}
			}
			int p = user.position[i];
			int q = base.position[nStart];
			int km;
			if(!isInclude && user.file[p] == base.file[q])
			{		
				km = 0;
				while(p>=km && q>=km &&
					user.file[p-km] == base.file[q-km])
					km++;
				if(p>=km && q>=km)
					base.zoneStart.push_back(q-km+1);
				else
					base.zoneStart.push_back(q-km);
				km = 0;
				while(p+km<user.file.size() && q+km<base.file.size() &&
					user.file[p+km] == base.file[q+km])
					km++;
				if(p+km<user.file.size() && q+km<base.file.size())
					base.zoneEnd.push_back(q+km-1);
				else
					base.zoneEnd.push_back(q+km);
			}
		}
	}
	return double(same)/double(user.hashKey.size());
}

void quicksort(info& buffer,int p,int r)
{
	if (p >= r)
		return;
	int i = p,j = r, x = buffer.hashKey[p + (r - p) / 2];
	while (1)
	{
		while (buffer.hashKey[i] < x && i <= r)		
			i++;	
		while (buffer.hashKey[j] > x && j >= p)		
			j--;
		if (i < j)
		{
			unsigned int temp =  buffer.hashKey[i];buffer.hashKey[i] = buffer.hashKey[j];buffer.hashKey[j] = temp;
			temp = buffer.position[i];buffer.position[i] = buffer.position[j];buffer.position[j] = temp;
			i++;
			j--;
		}
		else
			break;
	}
	quicksort(buffer,p,j);quicksort(buffer,j+1,r);
	return;
}

inline unsigned int hash( const wstring& str)     //use inline function to speed up the program
/*-----------------XJO: please check this hash function, it may go wrong with wstring------------------
-------------------------------------------------------------------------------------------------------------------*/
{ 
	if (hashValue.count(str))
		return hashValue[str];
	else
	{
		totHN++;
		hashValue[str] = totHN;
		return totHN;
	}
	/*register unsigned int h = 0;
	for (int i = 0 ; i < str.size() ; ++i){
	wchar_t p = str[i];
	h = 31 * h + p;
	}
	return h;
	-------------------This is the last version
	register unsigned int h; 
	register unsigned char *p;   
	for(h=0, p = (unsigned char *)str; *p ; p++) 
	h = 31 * h + *p; 
	return h;
	-----------------------*/
} 

info calcHash(const wstring & txt)
{
	info buffer,temp;
	for (int i = 0;i <= txt.size() - k;i++)//calculate hash-values of strings whose length is k
	{
		wstring temp1 = txt.substr(i,k);
		unsigned int hashValue = hash(temp1);
		/*unsigned int hashValue = hash((txt.substr(i,k)).c_str());*/
		temp.hashKey.push_back(hashValue);
		temp.position.push_back(i);
	}

	buffer.hashKey.push_back(temp.hashKey[0]);//find the minimum in every w hash-values
	buffer.position.push_back(temp.position[0]);
	for (int i = 1;i < w; i++)
		if (temp.hashKey[i] < buffer.hashKey[0])
		{
			buffer.hashKey[0] = temp.hashKey[i];
			buffer.position[0] = i;
		}	 

		unsigned int j = buffer.position[0];
		for (int i = w ; i <= temp.hashKey.size() - w;i ++)
		{
			if (i - j < w)
			{
				if (temp.hashKey[j] > temp.hashKey[i])
					j = i;
				if (j != buffer.position[buffer.position.size() - 1])
				{
					buffer.hashKey.push_back(temp.hashKey[j]);
					buffer.position.push_back(temp.position[j]);
				}
			}
			else
			{
				int min = j + 1;
				while (j < i)
				{
					j++;
					if (temp.hashKey[min] > temp.hashKey[j])
						min = j;
				}
				j = min;
				if (j != buffer.position[buffer.position.size() -1] && j < temp.hashKey.size())
				{
					buffer.hashKey.push_back(temp.hashKey[j]);
					buffer.position.push_back(temp.position[j]);
				}
			}
		}
		quicksort(buffer,0,buffer.hashKey.size() - 1);
		return buffer;
}


int main()
{
	wcout.imbue(locale("chs"));  //make wcout ouput Chinese correctly

	vector<info> dataBase;  //store the finger prints of all source files
	info target;  //store the finger print of target file

	time_t begin,end;

	ofstream fout("test.txt");
	for(int o=3;o<=10;o=o+1)
	{
		fout << "w= "<< o << endl;
		for(int j=3;j<=10;j=j+1)
		{
			begin=clock();
			w=o;
			k=j;
			t = w + k - 1;
			fout << k << "\t";

			ifstream input(fileList);
			vector<string> fileName; // store the source file names;
			string temp;

			while (getline(input,temp))
				fileName.push_back(temp);
			input.close();

			dataBase.clear();
			for ( size_t i = 0 ; i < fileName.size() ; ++i){
				wstring thisFile = preProcess(fileName[i]);  //read the file and delete the irrelevant characters
				info temp = calcHash(thisFile);  //calc its finger prints
				temp.file = thisFile;
				dataBase.push_back(temp);  //store it in the vector
			}

			wstring targetString = preProcess(targetFile);
			target = calcHash(targetString);
			target.file = targetString;

			for (int i = 0 ; i < dataBase.size() ; ++i){
				fout << compare(target,dataBase[i]) << "\t";
				for(int j=0; j < dataBase[i].zoneStart.size(); j++)
				{
					for(int k=dataBase[i].zoneStart[j]; k<=dataBase[i].zoneEnd[j]; k++)
						wcout << dataBase[i].file[k];
					wcout << endl;
				}
			}

			//fout << "0.17779\t0.32568\t0.49653" << endl;
			fout << "0.20062\t0.0983\t0.30083\t0.40024" << endl;
			end=clock();
			//fout << "runtime: " << double(end-begin)/CLOCKS_PER_SEC << endl;
		}
	}
	fout.close();

	return 0;
}