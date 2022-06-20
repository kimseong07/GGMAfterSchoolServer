using ServerCore;
using System;

namespace Server
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick; //실행시간
        public Action action; //수행 작업

        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - execTick;
        }
        //내가 실행시간이 더 크다면 + 값을 리턴할 것이고 그러면 우선순위큐에서 하단에 배치된다.
    }

    public class JobTimer
    {
        PriorityQueue<JobTimerElement> _pq = new PriorityQueue<JobTimerElement>();

        object _lock = new object(); // 멀티쓰레드에서 사용할 수 있도록 

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0) //몇초 (틱) 후에 실행할꺼냐?
        {
            JobTimerElement job;
            job.execTick = System.Environment.TickCount + tickAfter; //다음 실행 시간
            job.action = action;

            lock(_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JobTimerElement job;
                //큐에 있는 값이 현재틱보다 지난애들은 전부 실행시켜준다
                lock(_lock)
                {
                    if (_pq.Count == 0) break;

                    job = _pq.Peek();
                    if (job.execTick > now) break;

                    _pq.Pop();
                    job.action(); //액션 실행
                }
            }
        }
    }
}
