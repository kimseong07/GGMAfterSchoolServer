using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance;  } }

        int _sessionId = 0; //증가하면서 부여
        Dictionary<int, ClientSession> _session = new Dictionary<int, ClientSession>();

        object _lock = new object();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++_sessionId;

                //나중에 풀매니저를 넣는다면 여기에 넣어주면 돼. 세션 초기화에 관한 로직이 없으니 여기서는 풀링은 안해 (크게 성능향상은 없어)
                ClientSession session = new ClientSession();
                //아이디 부여하고
                session.SessionId = sessionId;
                //딕셔너리에 넣는다
                _session.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");
                return session;
            }
        }

        //원하는 세션을 가져오는 함수
        public ClientSession Find(int id)
        {
            lock(_lock)
            {
                ClientSession session = null;
                _session.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock(_lock)
            {
                _session.Remove(session.SessionId);
            }
        }
    }
}
