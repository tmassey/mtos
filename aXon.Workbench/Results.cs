using System;
using Gtk;
using MongoDB.Driver;
using aXon.Worker;
using System.Collections.Generic;
using MongoDB.Bson;

namespace aXon.Workbench
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class Results : Gtk.Bin
	{
		public Results ()
		{
			this.Build ();
			SetupColumns ();
			Refresh (null, null);
		}

		private MongoClient _client = new MongoClient ("mongodb://Dev-Svr2.systest.sc2services.com");

		private TreeViewColumn _id { get; set; }

		private TreeViewColumn _average { get; set; }

		private TreeViewColumn _total { get; set; }

		private ListStore _liststore { get; set; }

		void SetupColumns ()
		{
			_id = new TreeViewColumn ();
			_average = new TreeViewColumn ();
			_total = new TreeViewColumn ();
			_id.Title = "Task Id";
			_average.Title = "Average Time";
			_total.Title = "Total Time";


			Gtk.CellRendererText idcell = new Gtk.CellRendererText ();
			Gtk.CellRendererText avgcell = new Gtk.CellRendererText ();
			Gtk.CellRendererText totalcell = new Gtk.CellRendererText ();

			_id.PackStart (idcell, true);
			_average.PackStart (avgcell, true);
			_total.PackStart (totalcell, true);
			// Add the columns to the TreeView
			treeview1.AppendColumn (_id);
			treeview1.AppendColumn (_average);
			treeview1.AppendColumn (_total);

			_id.AddAttribute (idcell, "text", 0);
			_average.AddAttribute (avgcell, "text", 1);
			_total.AddAttribute (totalcell, "text", 2);

			// Create a model that will hold two strings - Artist Name and Song Title
			_liststore = new Gtk.ListStore (typeof(string), typeof(string), typeof(string));
			treeview1.Model = _liststore;
		}

		protected void Refresh (object o, Gtk.ButtonPressEventArgs args)
		{
			_liststore.Clear ();
			treeview1.ShowAll ();
			var db = _client.GetDatabase ("Results");
			var collection = db.GetCollection<BenchmarkResult> ("Benchmark");
			var rese = collection.Find<BenchmarkResult> (Builders<BenchmarkResult>.Filter.Ne ("_id", Guid.Empty)).ForEachAsync (t => _liststore.AppendValues (t.Id.ToString (), t.Average.ToString (), t.Totaltime.ToString ()));
			treeview1.ShowAll ();
			//			foreach (BenchmarkResult t in cursor) {
//				_liststore.AppendValues (t.Id.ToString (), t.Average, t.Totaltime);
//			}
		}

		protected void RefreshClicked (object sender, EventArgs e)
		{
			_liststore.Clear ();
			treeview1.ShowAll ();
			var db = _client.GetDatabase ("Results");
			var collection = db.GetCollection<BenchmarkResult> ("Benchmark");
			var rese = collection.Find<BenchmarkResult> (Builders<BenchmarkResult>.Filter.Ne ("_id", Guid.Empty)).ForEachAsync (t => _liststore.AppendValues (t.Id.ToString (), t.Average.ToString (), t.Totaltime.ToString ()));
			treeview1.ShowAll ();
		}
	}
}

