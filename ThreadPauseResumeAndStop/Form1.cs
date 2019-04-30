using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ThreadPauseResumeAndStop
{
    public partial class Form1 : Form
    {
        private Worker worker;

        public Form1()
        {
            InitializeComponent();
            worker = new Worker();
            worker.SendMessageEvent += Worker_SendMessageEvent;
        }

        private void Worker_SendMessageEvent(object sender, string e)
        {
            ListBoxAdd(listBox1, e);
        }

        delegate void ListBoxAddHandler(ListBox listBox, string msg);
        private void ListBoxAdd(ListBox listBox, string msg)
        {
            if (listBox.InvokeRequired)
            {
                ListBoxAddHandler handler = ListBoxAdd;
                msg += $"[{Thread.CurrentThread.ManagedThreadId}]";
                this.Invoke(handler, new object[] { listBox, msg });
            }
            else
            {
                listBox.Items.Add(msg + Environment.NewLine);
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            worker.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            worker.Pause();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            worker.Resume();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            worker.Stop();
        }
    }
}
