
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.Action FileAction;
	
	private global::Gtk.Action stopAction;
	
	private global::Gtk.Action newAction;
	
	private global::Gtk.Action mediaPlayAction;
	
	private global::Gtk.VBox vbox1;
	
	private global::Gtk.MenuBar menubar2;
	
	private global::Gtk.Toolbar toolbar1;
	
	private global::Gtk.HBox hbox1;
	
	private global::Gtk.Notebook notebook;
	
	private global::Gtk.HBox hbox2;
	
	private global::Gtk.VBox vbox2;
	
	private global::Gtk.Label label2;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	
	private global::Gtk.TextView txtLog;
	
	private global::Gtk.VBox vbox3;
	
	private global::Gtk.Label label3;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	
	private global::Gtk.TextView txtProgress;
	
	private global::Gtk.Label label1;
	
	private global::Gtk.Statusbar statusbar1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.stopAction = new global::Gtk.Action ("stopAction", global::Mono.Unix.Catalog.GetString ("_Stop"), null, "gtk-stop");
		this.stopAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Stop");
		w1.Add (this.stopAction, null);
		this.newAction = new global::Gtk.Action ("newAction", null, null, "gtk-new");
		w1.Add (this.newAction, null);
		this.mediaPlayAction = new global::Gtk.Action ("mediaPlayAction", null, null, "gtk-media-play");
		w1.Add (this.mediaPlayAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name=\'menubar2\'><menu name=\'FileAction\' action=\'FileAction\'><menuite" +
		"m name=\'stopAction\' action=\'stopAction\'/></menu></menubar></ui>");
		this.menubar2 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar2")));
		this.menubar2.Name = "menubar2";
		this.vbox1.Add (this.menubar2);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.menubar2]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><toolbar name=\'toolbar1\'><toolitem name=\'newAction\' action=\'newAction\'/><tool" +
		"item name=\'mediaPlayAction\' action=\'mediaPlayAction\'/></toolbar></ui>");
		this.toolbar1 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar1")));
		this.toolbar1.Name = "toolbar1";
		this.toolbar1.ShowArrow = false;
		this.vbox1.Add (this.toolbar1);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.toolbar1]));
		w3.Position = 1;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.notebook = new global::Gtk.Notebook ();
		this.notebook.CanFocus = true;
		this.notebook.Name = "notebook";
		this.notebook.CurrentPage = 0;
		// Container child notebook.Gtk.Notebook+NotebookChild
		this.hbox2 = new global::Gtk.HBox ();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		// Container child vbox2.Gtk.Box+BoxChild
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("LOG MESSAGES");
		this.vbox2.Add (this.label2);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.label2]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.WidthRequest = 400;
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.txtLog = new global::Gtk.TextView ();
		this.txtLog.CanFocus = true;
		this.txtLog.Name = "txtLog";
		this.GtkScrolledWindow.Add (this.txtLog);
		this.vbox2.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.GtkScrolledWindow]));
		w6.Position = 1;
		this.hbox2.Add (this.vbox2);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox2]));
		w7.Position = 0;
		w7.Expand = false;
		w7.Fill = false;
		// Container child hbox2.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("PROGRESS MESSAGES");
		this.vbox3.Add (this.label3);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label3]));
		w8.Position = 0;
		w8.Expand = false;
		w8.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.WidthRequest = 400;
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.txtProgress = new global::Gtk.TextView ();
		this.txtProgress.CanFocus = true;
		this.txtProgress.Name = "txtProgress";
		this.GtkScrolledWindow1.Add (this.txtProgress);
		this.vbox3.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow1]));
		w10.Position = 1;
		this.hbox2.Add (this.vbox3);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox3]));
		w11.Position = 1;
		w11.Expand = false;
		w11.Fill = false;
		this.notebook.Add (this.hbox2);
		// Notebook tab
		this.label1 = new global::Gtk.Label ();
		this.label1.Name = "label1";
		this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Welcome");
		this.notebook.SetTabLabel (this.hbox2, this.label1);
		this.label1.ShowAll ();
		this.hbox1.Add (this.notebook);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.notebook]));
		w13.Position = 0;
		w13.Expand = false;
		w13.Fill = false;
		this.vbox1.Add (this.hbox1);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
		w14.Position = 2;
		// Container child vbox1.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.vbox1.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.statusbar1]));
		w15.Position = 3;
		w15.Expand = false;
		w15.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 808;
		this.DefaultHeight = 300;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.stopAction.Activated += new global::System.EventHandler (this.StopApp);
		this.newAction.Activated += new global::System.EventHandler (this.AddNewJob);
		this.mediaPlayAction.Activated += new global::System.EventHandler (this.RunTasks);
	}
}
