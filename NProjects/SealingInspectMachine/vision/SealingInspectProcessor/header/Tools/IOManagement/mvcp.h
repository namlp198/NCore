#ifndef _MVCP_H_
#define _MVCP_H_

#include <string>
#include <stdint.h>

namespace mvsol {

	namespace protocol {

		const unsigned char STX			= 0x02;
		const unsigned char ETX			= 0x03;
		const unsigned char UNIQUECODE	= 0x7D;
		const unsigned char MAGICCODE	= 0x20;

		const unsigned char READ		= 0x52;
		const unsigned char WRITE		= 0x57;

		const unsigned char READACK		= 0x72;
		const unsigned char WRITEACK	= 0x77;

		enum ProtocolVersion
		{
			kProtocolVersion1 = 1,
		};


		class AFX_CLASS_EXPORT Security
		{
		public:
			static int Encrypt(const unsigned char* indata, int indatasize, unsigned char* outdata, int* outdatasize);
			static int Decrypt(const unsigned char* indata, int indatasize, unsigned char* outdata, int* outdatasize);
		};
		

		class AFX_CLASS_EXPORT BaseProtocol
		{
		public:
			BaseProtocol();
			~BaseProtocol();

		public:
			static unsigned char CalcCheckSum(const unsigned char* data, int datasize);

		public:
			virtual int GetBytes(unsigned char* outdata, int* outdatasize) = 0;
		};


		class AFX_CLASS_EXPORT MVCP : public BaseProtocol
		{
		public:
			MVCP();
			MVCP(unsigned char command, unsigned short address, unsigned int data, unsigned char status = 0x00);

			enum { LENGTH = 11, };

		public:
			virtual int GetBytes(unsigned char* outdata, int* outdatasize);
			virtual void Parse(unsigned char* indata, int indatasize);

			virtual std::string ToString();

		public:
			static int GetBytes(unsigned char command, unsigned short address, unsigned int data, unsigned char status, unsigned char* outdata, int* outdatasize);

		//加己
		public:
			unsigned char GetCommand() const { return command_; }
			unsigned short GetAddress() const { return address_; }
			unsigned int GetData() const { return data_; }
			unsigned char GetStatus() const { return status_; }
			unsigned char GetCheckSum() const { return checksum_; }

		protected:
			unsigned char command_;
			unsigned short address_;
			unsigned int data_;
			unsigned char status_;
			unsigned char checksum_;
		};


		// Broadcast sending data
		class AFX_CLASS_EXPORT MVBRCP : public BaseProtocol
		{
		public:
			MVBRCP();
			~MVBRCP();

			enum { LENGTH = 19, };

		public:
			virtual int GetBytes(unsigned char* outdata, int* outdatasize);

		// 加己
		public:
			static int GetBytes(unsigned char command, unsigned short address, unsigned int serialnumber, unsigned long long data, unsigned char status, unsigned char* outdata, int* outdatasize);
		};

		class AFX_CLASS_EXPORT MVBACP : public BaseProtocol
		{
		public:
			MVBACP();
			~MVBACP();

			enum { LENGTH = 71, };

		public:
			virtual int GetBytes(unsigned char* outdata, int* outdatasize);
			virtual int Parse(const unsigned char* indata, int indatasize);

			//virtual int Send()

		public:
			static int GetBytes(unsigned char command, unsigned short address, unsigned int data, unsigned char status, unsigned char* outdata, int* outdatasize);
		};

	} // namespace protocol

} // namespace mvsol


#endif //__MVCP_H__