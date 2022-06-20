using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    //해야될 액션의 목록을 들고 있는 것이 잡큐이다.
    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        
        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock(_lock)
            {
                _jobQueue.Enqueue(job);
                //send하고 유사하다
                if (!_flush)
                {
                    flush = _flush = true;
                }
            }
            //만약 락킹에서 처리를 해야겠다라고 결정했다면
            //(참고로 여기서 지역변수 flush는 지역변수기 때문에 병행에 문제 없다)

            if (flush)
                Flush();
        }

        void Flush()
        {
            while(true)
            {
                Action action = Pop(); //팝 자체가 락킹 되어 있어서 괜찮다.
                if (action == null) return;
                action(); //실행
            }
        }

        Action Pop()
        {
            lock(_lock)
            {
                if(_jobQueue.Count == 0)
                {
                    _flush = false; //다 꺼내서 수행이 되었으니 Flush를 다시 실행시킬 수 있도록 함.
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
