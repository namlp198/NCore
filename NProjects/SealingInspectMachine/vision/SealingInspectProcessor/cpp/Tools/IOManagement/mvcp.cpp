#include "pch.h"
#include "mvcp.h"

#include <assert.h>
#include <sstream>
#include <iomanip>

namespace mvsol {

	namespace protocol {

		int Security::Encrypt(const unsigned char* indata, int indatasize, unsigned char* outdata, int* outdatasize)
		{
			assert(indata);
			assert(indatasize > 0);
			assert(outdata);
			assert(outdatasize);

			int startindex = 0;
			int endindex = indatasize - 1;

			//시작위치
			while (startindex < indatasize && indata[startindex] != STX)
			{
				startindex++;
			}

			if (startindex == indatasize)
			{
				return -1;
			}


			//종료위치
			while (endindex > 0 && indata[endindex] != ETX)
			{
				endindex--;
			}

			if (endindex <= 0)
			{
				return -1;
			}

			int k = 1;
			outdata[0] = STX;
			for (int i = startindex + 1; i < endindex; i++)
			{
				if (indata[i] == STX ||
					indata[i] == ETX ||
					indata[i] == UNIQUECODE)
				{
					outdata[k + 0] = UNIQUECODE;
					outdata[k + 1] = indata[i] ^ MAGICCODE;
					k += 2;
				}
				else
				{
					outdata[k] = indata[i];
					k++;
				}
			}
			outdata[k] = ETX;

			*outdatasize = k + 1;

			return 0;
		}


		int Security::Decrypt(const unsigned char* indata, int indatasize, unsigned char* outdata, int* outdatasize)
		{
			assert(indata);
			assert(indatasize > 0);
			assert(outdata);
			assert(outdatasize);

			int startindex = 0;
			int endindex = indatasize - 1;

			//시작위치
			while (startindex < indatasize && indata[startindex] != STX)
			{
				startindex++;
			}

			if (startindex == indatasize)
			{
				return -1;
			}


			//종료위치
			while (endindex > 0 && indata[endindex] != ETX)
			{
				endindex--;
			}

			if (endindex <= 0)
			{
				return -1;
			}


			int k = 1;
			outdata[0] = STX;
			for (int i = startindex + 1; i < endindex; i++)
			{
				if (indata[i] == UNIQUECODE)
				{
					outdata[k] = indata[i + 1] ^ MAGICCODE;
					i++;
				}
				else
				{
					outdata[k] = indata[i];
				}
				k++;
			}
			outdata[k] = ETX;

			*outdatasize = k + 1;


			return 0;
		}


		BaseProtocol::BaseProtocol()
		{

		}

		BaseProtocol::~BaseProtocol()
		{

		}

		unsigned char BaseProtocol::CalcCheckSum(const unsigned char* data, int datasize)
		{
			unsigned int sum = 0;

			for (int i = 1; i < datasize - 2; i++)
			{
				sum += data[i];
			}

			return static_cast<unsigned char>(sum);
		}

		int MVBRCP::GetBytes(unsigned char command, unsigned short address, unsigned int serialnumber, unsigned long long data, unsigned char status, unsigned char* outdata, int* outdatasize)
		{
			assert(outdata);
			assert(outdatasize);

			outdata[0] = STX;
			outdata[1] = command;
			*((unsigned short*)&outdata[2]) = address;
			*((unsigned int*)&outdata[4]) = serialnumber;
			*((unsigned long long*)&outdata[8]) = data;
			outdata[LENGTH - 3] = status;
			outdata[LENGTH - 2] = CalcCheckSum(outdata, LENGTH);
			outdata[LENGTH - 1] = ETX;

			*outdatasize = LENGTH;

			return LENGTH;
		}

		int MVBRCP::GetBytes(unsigned char* outdata, int* outdatasize)
		{
			return 0;
		}

		int MVBACP::GetBytes(unsigned char* outdata, int* outdatasize)
		{
			return 0;
		}

		int MVBACP::Parse(const unsigned char* indata, int indatasize)
		{
			return 0;
		}

		MVCP::MVCP()
			: command_(0x00)
			, address_(0x0000)
			, data_(0x00000000)
			, status_(0x00)
		{

		}

		MVCP::MVCP(unsigned char command, unsigned short address, unsigned int data, unsigned char status /*= 0x00*/)
			: command_(command)
			, address_(address)
			, data_(data)
			, status_(status)
		{

		}

		int MVCP::GetBytes(unsigned char* outdata, int* outdatasize)
		{
			assert(outdata);
			assert(outdatasize);

			outdata[0] = STX;
			outdata[1] = command_;
			*((unsigned short*)&outdata[2]) = address_;
			*((unsigned int*)&outdata[4]) = data_;
			outdata[8] = status_;
			outdata[9] = CalcCheckSum(outdata, LENGTH);
			outdata[LENGTH - 1] = ETX;

			*outdatasize = LENGTH;

			return LENGTH;
		}


		int MVCP::GetBytes(unsigned char command, unsigned short address, unsigned int data, unsigned char status, unsigned char* outdata, int* outdatasize)
		{
			assert(outdata);
			assert(outdatasize);

			outdata[0] = STX;
			outdata[1] = command;
			*((unsigned short*)&outdata[2]) = address;
			*((unsigned int*)&outdata[4]) = data;
			outdata[8] = status;
			outdata[9] = CalcCheckSum(outdata, LENGTH);
			outdata[LENGTH - 1] = ETX;

			*outdatasize = LENGTH;

			return LENGTH;
		}

		void MVCP::Parse(unsigned char* indata, int indatasize)
		{
			assert(indata);
			assert(*indata == STX);
			assert(*(indata + indatasize - 1) == ETX);

			command_ = *(indata + 1);
			address_ = *reinterpret_cast<unsigned short*>(indata + 2);
			data_ = *reinterpret_cast<unsigned int*>(indata + 4);
			status_ = *(indata + 8);
			checksum_ = *(indata + 9);
		}

		std::string MVCP::ToString()
		{
			std::ostringstream oss;

			oss << "COMAND=" << (char)command_
				<< ", ADDRESS=0x" << std::hex << std::setw(4) << std::uppercase << std::setfill('0') << address_
				<< ", DATA=" << data_
				<< ", STATUS=0x" << std::hex << std::setw(2) << std::uppercase << std::setfill('0') << 0xFF;

			return oss.str();
		}

		} // namespace protocol

} // namespace mvsol



