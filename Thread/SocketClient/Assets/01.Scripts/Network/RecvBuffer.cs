using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos; //커서 

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize
        {
            get { return _writePos - _readPos; }
        }
        public int FreeSize
        {
            get { return _buffer.Count - _writePos; }
        }

        public ArraySegment<byte> ReadSegment
        {
            //받은 데이터를 리턴해주는 것
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize) ;  }
        }

        public ArraySegment<byte> WriteSegment
        {
            //데이터를 받기 위한 비어있는 공간
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            //중간에 정리해주는 함수
            int dataSize = DataSize; //r과 w가 겹치는 상태에서 0이 됨
            if(dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                //처음위치로 readPos에서부터 dataSize만큼 복사해서 가져간다.
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }


        public bool OnRead(int numOfByte)
        {
            if (numOfByte > DataSize)
                return false;
            _readPos += numOfByte;
            return true;
        }

        public bool OnWrite(int numOfByte)
        {
            if(numOfByte > FreeSize)
            {
                return false;
            }
            _writePos += numOfByte;
            return true;
        }
    }
}
