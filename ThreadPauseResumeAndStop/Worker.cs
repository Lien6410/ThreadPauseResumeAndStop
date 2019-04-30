using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadPauseResumeAndStop
{
    public class Worker
    {
        //Initializes a new instance of the ManualResetEvent class
        //with a Boolean value indicating whether to set the initial state to signaled.
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public event EventHandler<string> SendMessageEvent;

        Thread _thread;
        public Worker() { }

        private void SendMessage(string msg)
        {
            var timeStamp = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff]");
            msg = $"[{Thread.CurrentThread.ManagedThreadId}]" + timeStamp + msg;
            if (SendMessageEvent != null)
            {
                SendMessageEvent.Invoke(this, msg);
            }
        }

        public void Job()
        {
            int cnt = 0;
            while (true)
            {
                // 封鎖目前執行緒, 直到waitHandle收到通知, 
                // Timeout.Infinite表示無限期等候
                _pauseEvent.WaitOne(Timeout.Infinite);

                // return true if the current instance receives a signal.
                // If the current instance is never signaled, WaitOne never returns
                if (_shutdownEvent.WaitOne(0))
                {
                    break;
                }

                //if (ShutdownEvent.WaitOne(Timeout.Infinite))
                //    *因為沒有收到signal, 所以會停在if()這一行, 造成cnt無法累加

                //Console.WriteLine("{0}", cnt++);
                SendMessage(cnt.ToString());
                cnt++;
                Thread.Sleep(2000);
            }
        }

        public void Start()
        {
            _thread = new Thread(Job);
            _thread.Start();
            //Console.WriteLine("Thread started running");
            SendMessage("Thread started running");
        }

        public void Pause()
        {
            // Sets the state of the event to nonsignaled,
            // causing threads to block.

            _pauseEvent.Reset();
            //Console.WriteLine("Thread paused");
            SendMessage("Thread paused");
        }

        public void Resume()
        {
            // Sets the state of the event to signaled,
            // allowing one or more waiting threads to proceed.
            _pauseEvent.Set();
            //Console.WriteLine("Thread resuming ");
            SendMessage("Thread resuming");
        }

        public void Stop()
        {
            // Signal the shutdown event
            _shutdownEvent.Set();
            //Console.WriteLine("Thread Stopped ");
            SendMessage("Thread Stopped");

            // Make sure to resume any paused threads
            _pauseEvent.Set();

            // Wait for the thread to exit
            _thread.Join();
        }

    }
}
