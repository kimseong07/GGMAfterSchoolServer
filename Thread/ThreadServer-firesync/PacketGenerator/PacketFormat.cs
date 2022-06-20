using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    // {0} enum 내부 값
    // {1} 나머지 클래스들
    class PacketFormat
    {
        // {0} 패킷 처리 등록부
        public static string managerFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance
    {{
        get {{ return _instance;}}
    }}
    #endregion

    //생성자에서 실행하도록 
    private PacketManager()
    {{
        Register();
    }}

    //받은 패킷을 리딩할 핸들러를 지정하는 딕셔너리
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket >> _makeFunc 
        = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    //패킷번호에 따른 핸들러를 등록하는 딕셔너리
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket (PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallBack = null)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if(_makeFunc.TryGetValue(id, out func))
        {{
            IPacket packet = func(session, buffer);
            
            if(onRecvCallBack != null)
            {{
                onRecvCallBack(session, packet);
            }}
            else
            {{
                HandlePacket(session, packet);
            }}
        }}
    }}

    //IPacket 을 구현했으면서 new가 가능한 녀석이어야 한다.
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }}

    public void HandlePacket(PacketSession session, IPacket packet)
    {{
        Action<PacketSession, IPacket> action = null;

        if (_handler.TryGetValue(packet.Protocol, out action))
        {{
            action(session, packet);
        }}
    }}
}}";

        // {0} 패킷이름
        public static string managerRegisterFormat =
@"
        _makeFunc.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";
        // {0} 패킷이름 번호 목록(enum)
        // {1} 나머지 데이터(패킷목록)
        public static string fileFormat =
@"using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;
using System.Numerics;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

{1}
";

        // {0} 패킷 이름
        // {1} 패킷 번호
        public static string packetEnumFormat =
@"{0} = {1},";

        //@는 여러줄로 스트링 넣을 때 사용
        // 리플레이스 목적의 중괄호와 일반 중괄호의 구분을 위해 일반은 2개씩 써준다.
        // {0} 패킷 이름
        // {1} 멤버 변수
        // {2} 멤버변수 리드 부분
        // {3} 멤버변수 라이트 부분
        public static string packetFormat =
@"
public class {0} : IPacket
{{
    {1}    
    
    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.{0}), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        {3}

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
                    
        return SendBufferHelper.Close(count);
    }}
}}
";
        //{0} 변수의 형식
        //{1} 변수의 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버변수들
        // {3} 멤버변수 Read
        // {4} 멤버변수 Write
        public static string memberListFormat =
@"public class {0}
{{
    {2}

    public void Read(ArraySegment<byte> segment, ref ushort count)
    {{
        {3}
    }}

    public void Write(ArraySegment<byte> segment, ref ushort count)
    {{
        {4}  
    }}    
}}

public List<{0}> {1}s = new List<{0}>(); 
";

        //{0} 변수의 이름
        //{1} To~ 변수형식
        //{2} 변수 타입
        public static string readFormat =
@"this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";

        // {0} 변수 이름
        public static string stringReadFormat =
@"
ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;
";
        // {0} 리스트 이름 대문자
        // {1} 리스트 이름 소문자
        public static string listReadFormat =
@"
this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);

for(int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(segment, ref count);
    {1}s.Add({1});
}}
";

        // {0} 변수 이름
        // {1} 타입
        public static string byteReadFormat =
@"
this.{0} = segment.Array[segment.Offset + count];
count += sizeof({1});
";

        // {0} 변수 이름
        public static string vector3ReadFormat =
@"
this.{0} = new Vector3();
this.{0}.X = BitConverter.ToSingle(segment.Array, segment.Offset + count);
count += sizeof(float);
this.{0}.Y = BitConverter.ToSingle(segment.Array, segment.Offset + count);
count += sizeof(float);
this.{0}.Z = BitConverter.ToSingle(segment.Array, segment.Offset + count);
count += sizeof(float);
";


        //{0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"
Array.Copy(BitConverter.GetBytes(this.{0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});
";


        // {0} 변수 이름
        public static string stringWriteFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
count += {0}Len;
";
        // {0} 리스트 이름 대문자
        // {1} 리스트 이름 소문자
        public static string listWriteFormat =
@"
Array.Copy(BitConverter.GetBytes((ushort)this.{1}s.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);

foreach({0} {1} in this.{1}s)
{{
    {1}.Write(segment, ref count); 
}}
";
        // {0} 변수 이름
        // {1} 타입
        public static string byteWriteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});
";
        // {0} 변수 이름
        public static string vector3WriteFormat =
@"
Array.Copy(BitConverter.GetBytes(this.{0}.X), 0, segment.Array, segment.Offset + count, sizeof(float));
count += sizeof(float);
Array.Copy(BitConverter.GetBytes(this.{0}.Y), 0, segment.Array, segment.Offset + count, sizeof(float));
count += sizeof(float);
Array.Copy(BitConverter.GetBytes(this.{0}.Z), 0, segment.Array, segment.Offset + count, sizeof(float));
count += sizeof(float);
";
    }
}
