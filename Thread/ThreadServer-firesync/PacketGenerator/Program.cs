using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPackets; //패킷 스트링을 가지고 있는다.
        static int packetId = 0; //패킷 종류별 이름
        static string packetEnums; // 패킷 종류의 이넘형 
        static Dictionary<string, string> typeDic = new Dictionary<string, string>();

        static string clientRegister = "";
        static string serverRegister = "";

        static Program()
        {
            typeDic.Add("bool", "ToBoolean");
            typeDic.Add("short", "ToInt16");
            typeDic.Add("ushort", "ToUInt16");
            typeDic.Add("int", "ToInt32");
            typeDic.Add("long", "ToInt64");
            typeDic.Add("float", "ToSingle");
            typeDic.Add("double", "ToDouble");
        }


        static void Main(string[] args)
        {

            string pdlPath = "../PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true, //xml내부의 주석 무시
                IgnoreWhitespace = true //xml 내부의 공백 무시
            };
            //XmlReader reader = XmlReader.Create("PDL.xml", settings);

            if(args.Length >= 1)
            {
                pdlPath = args[0]; //인자를 함께 실행하면 해당 파일로 생성한다.
            }

            //reader.Dispose();

            //위와 같이 해도 되지만
            using(XmlReader reader = XmlReader.Create(pdlPath, settings))
            {
                //이렇게 하면 닫힐때 자동으로 Dispose된다.
                reader.MoveToContent(); //해더를 건너뛰고 내용으로 들어온다 
                //즉 PDL까지 무시하고 packet부터 읽어들인다.
                while(reader.Read())
                {
                    //XMLNodeType이 EndElement면 닫는 태그라는 뜻임
                    if(reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    {
                        ParsePacket(reader); //리딩하면서 결과물은 genPackets에 기록될꺼야.
                    }                    
                }
                // {0} 패킷이름 번호 목록(enum)
                // {1} 나머지 데이터(패킷목록)
                string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);
                File.WriteAllText("GenPacks.cs", fileText);
                string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);
                string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);
            }
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;

            if (r.Name.ToLower() != "packet") return;

            string packetName = r["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Error : Packet without name");
                return;
            }

            string usage = r["usage"];
            if (string.IsNullOrEmpty(usage))
            {
                Console.WriteLine("Error : Packet without usage");
                return;
            }

            //튜플을 분해해서 변수로 가져온다.
            (string member, string read, string write) = ParseMemeber(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, member, read, write);
            
            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";


            if (usage.Equals("client"))
            {
                //클라에서 쓰는거면 서버가 받는거니까 이렇게 작성해야해
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            }
            else if (usage.Equals("server"))
            { //서버에서 쓰는거면 클라가 받아야하고
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            }else
            {
                Console.WriteLine($"Error : Packet usage is not exist {packetName}");
                return;
            }
        }

        // {1} 멤버 변수
        // {2} 멤버변수 리드 부분
        // {3} 멤버변수 라이트 부분
        public static Tuple<string, string, string> ParseMemeber(XmlReader r)
        {
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1; //파싱할 깊이
            while(r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Error : Member without name");
                    return null;
                }

                //이미 멤버코드가 존재한다면 엔터한번 쳐주고 (엔터는 실행환경에 따라 인코드 코드가 달라져서 이렇게 해야해)
                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                
                switch(memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.byteReadFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.byteWriteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, typeDic[memberType] , memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.stringReadFormat, memberName);
                        writeCode += string.Format(PacketFormat.stringWriteFormat, memberName);
                        break;
                    case "list":
                        (string member, string read, string write) = ParseList(r);
                        memberCode += member;
                        readCode += read;
                        writeCode += write;
                        break;
                    case "vector3":
                        memberCode += string.Format(PacketFormat.memberFormat, r.Name, memberName);
                        readCode += string.Format(PacketFormat.vector3ReadFormat, memberName);
                        writeCode += string.Format(PacketFormat.vector3WriteFormat, memberName);
                        break;
                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("ERROR : List without name");
                return null;
            }

            (string member, string read, string write) = ParseMemeber(r);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName), FirstCharToLower(listName),
                member,read,write);
            string readCode = string.Format(PacketFormat.listReadFormat, FirstCharToUpper(listName), FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.listWriteFormat, FirstCharToUpper(listName), FirstCharToLower(listName)); ;

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    } //End of Program
    
}


