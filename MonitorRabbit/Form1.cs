using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyNetQ.Management.Client;

namespace MonitorRabbit
{
    public partial class Form1 : Form
    {
        private Timer _t = new Timer();
        public Form1()
        {
            InitializeComponent();
            _t.Interval = 10000;
            _t.Tick += new EventHandler(_t_Tick);
            _t.Start();
        }

        void _t_Tick(object sender, EventArgs e)
        {
            var initial = new ManagementClient("http://192.169.164.138", "guest", "guest");
            var nodes = initial.GetNodes();
            grdMain.DataSource = nodes;
            grdMain.RefreshDataSource();
            var queues = initial.GetQueues();
            grd.DataSource = queues;

            grd.RefreshDataSource();
            Application.DoEvents();
        }
                
        private void button1_Click(object sender, EventArgs e)
        {
            var initial = new ManagementClient("http://192.169.164.138", "guest", "guest");
           
            var queues = initial.GetQueues();
           foreach (var queue in queues)
            {
                if(queue.Name.Contains("error") || queue.MessagesReady >=25)
                    initial.DeleteQueue(queue);
                Console.WriteLine(queue.Name + ": " + queue.IdleSince);
            }            
        }

        private void RefreshNodes()
        {
        }
    }
}
