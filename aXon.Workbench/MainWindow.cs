using System;
using Gtk;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using RabbitMQ.Client;
using aXon.Workbench;

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
		try {
			var text = "Task: " + args.TaskId.ToString () + " % Complete: " + args.PercentComplete + "\n";// + txtProgress.Buffer.ToString ();
			//txtProgress.Buffer.Clear ();
			txtProgress.Buffer.Insert (txtProgress.Buffer.GetIterAtLine (0), text);
			//txtProgress.ShowAll ();
		} catch {
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		_TaskQueue.Dispose ();
		_LogQueue.Dispose ();
		_ProgressQueue.Dispose (); 
		Application.Quit ();
		a.RetVal = true;
	}

	private void _Log_OnReceivedMessage (object sender, TaskLogMessage args)
	{
		try {
			txtLog.Buffer.Insert (txtLog.Buffer.GetIterAtLine (0), args.LogMessage + "\n");
//			txtLog.Buffer.Text = args.LogMessage + "\n" + txtLog.Buffer.Text;
//			txtLog.ShowAll ();
		} catch {
		}
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
		_TaskQueue.Dispose ();
		_LogQueue.Dispose ();
		_ProgressQueue.Dispose (); 
		Application.Quit ();
	}



	
	protected void AddNewJob (object sender, EventArgs e)
	{
		notebook.AppendPageMenu (new Results (), new Label ("New Job"), new Label ("New Job"));
		notebook.ShowAll ();
	}

	protected void RunTasks (object sender, EventArgs e)
	{
		//for (int x = 0; x != 1000; x++) {
		TaskMessage t = new TaskMessage ();
		t.LogReportingLevel = LogLevel.Verbatium;
		t.MessageId = Guid.NewGuid ();
		t.TaskId = Guid.NewGuid ();
		t.TransmisionDateTime = DateTime.Now;
		t.ScriptType = TaskScriptType.CSharp;
		_TaskQueue.Publish (t);
		//System.Threading.Thread.Sleep (10000);
		//}
	}

	protected void OnDestroy (object o, DestroyEventArgs args)
	{
		_TaskQueue.Dispose ();
		_LogQueue.Dispose ();
		_ProgressQueue.Dispose (); 
		Application.Quit ();
	}
}