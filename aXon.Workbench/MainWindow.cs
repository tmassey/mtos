using System;
using Gtk;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using RabbitMQ.Client;
using GLib;

public partial class MainWindow: Gtk.Window
{
	private static MessageQueue<TaskMessage> _TaskQueue;

	private static MessageQueue<TaskLogMessage> _LogQueue;
	private static MessageQueue<TaskProgressMessage> _ProgressQueue;

	private static IConnection _Connection;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		InitConnection ();
		_TaskQueue = new MessageQueue<TaskMessage> (false, _Connection);
		_LogQueue = new MessageQueue<TaskLogMessage> (true, _Connection);
		_ProgressQueue = new MessageQueue<TaskProgressMessage> (true, _Connection);
		_LogQueue.OnReceivedMessage += _Log_OnReceivedMessage;
		_ProgressQueue.OnReceivedMessage += _ProgressQueue_OnReceivedMessage;
	}

	void _ProgressQueue_OnReceivedMessage (object sender, TaskProgressMessage args)
	{
		txtProgress.Buffer.Text = "% Complete: " + args.PercentComplete + "\n" + txtProgress.Buffer.Text;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void _Log_OnReceivedMessage (object sender, TaskLogMessage args)
	{
		txtLog.Buffer.Text = args.LogMessage + "\n" + txtLog.Buffer.Text;
	}

	private void InitConnection ()
	{

		var factory = new ConnectionFactory () { HostName = "dev-svr2.systest.sc2services.com" };//{ HostName = "192.168.1.140" };
		factory.AutomaticRecoveryEnabled = true;
		factory.NetworkRecoveryInterval = TimeSpan.FromSeconds (10);
		_Connection = factory.CreateConnection ();
	}

	protected void StopApp (object sender, EventArgs e)
	{
		Application.Quit ();
	}



	
	protected void AddNewJob (object sender, EventArgs e)
	{
		notebook.AppendPageMenu (new Button (), new Label ("New Job"), new Label ("New Job"));
		notebook.ShowAll ();
	}

	protected void RunTasks (object sender, EventArgs e)
	{
		for (int x = 0; x != 1000; x++) {
			TaskMessage t = new TaskMessage ();
			t.LogReportingLevel = LogLevel.Verbatium;
			t.MessageId = Guid.NewGuid ();
			t.TaskId = Guid.NewGuid ();
			t.TransmisionDateTime = DateTime.Now;
			t.ScriptType = TaskScriptType.CSharp;
			_TaskQueue.Publish (t);
			System.Threading.Thread.Sleep (10000);
		}
	}

}