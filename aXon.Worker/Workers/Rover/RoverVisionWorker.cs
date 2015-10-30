using System;
using aXon.Worker.Interfaces;
using MongoDB.Driver;
using aXon.Worker.Delegates;
using aXon.Worker.EventArgs;

namespace aXon.Worker
{

	public class RoverVisionWorker:ITaskWorker
	{
		#region ITaskWorker implementation

		public void Execute (Guid taskId)
		{
			//			try {
			//				double[][] input = Generate (ROW_COUNT, INPUT_COUNT);
			//				double[][] output = Generate (ROW_COUNT, OUTPUT_COUNT);
			//				DateTime start = DateTime.Now;
			//				long avg = 0;
			//				var sw = new Stopwatch ();
			//				sw.Start ();
			//				for (int i = 0; i < 10; i++) {
			//					long time = BenchmarkEncog (input, output);
			//					RaiseOnProgress (new OnProgressArgs () {
			//						CurrentTime = DateTime.Now,
			//						PercentComplete = (i * 10),
			//						StartTime = start,
			//						Status = TaskStatus.InProcess,
			//						TaskId = taskId
			//					});
			//					avg += time;
			//					Console.WriteLine ("Regular: {0}ms", time);
			//				}
			//				sw.Stop ();
			//				avg = avg / 10;
			//				RaiseOnComplete (new OnCompletionArgs (){ TaskId = taskId, Totaltime = sw.ElapsedMilliseconds, Average = avg });
			//				var db = client.GetDatabase ("Results");
			//				var collection = db.GetCollection<BenchmarkResult> ("Benchmark");
			//				collection.InsertOneAsync (new BenchmarkResult () { Id = taskId, Average = avg, Totaltime = sw.ElapsedMilliseconds });
			//
			//			} catch (SystemException e) {
			//				RaiseOnErrorOccured (new OnErrorArgs (){ TaskId = taskId, Error = e.Message });
			//			}
		}

		public event OnError ErrorOccured;

		protected virtual void RaiseOnErrorOccured (OnErrorArgs args)
		{
			OnError handler = ErrorOccured;
			if (handler != null)
				handler (this, args);
		}

		public event OnProgress Progress;

		protected virtual void RaiseOnProgress (OnProgressArgs args)
		{
			OnProgress handler = Progress;
			if (handler != null)
				handler (this, args);
		}

		public event OnCompletion Complete;

		protected virtual void RaiseOnComplete (OnCompletionArgs args)
		{
			OnCompletion handler = Complete;
			if (handler != null)
				handler (this, args);
		}

		public Guid Id { get; set; }

		#endregion

		public RoverVisionWorker ()
		{
		}
	}
}
