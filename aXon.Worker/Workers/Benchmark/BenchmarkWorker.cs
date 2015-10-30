using System;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using aXon.TaskTransport;
using aXon.Worker.Delegates;
using aXon.Worker.EventArgs;
using aXon.Worker.Interfaces;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Util;

namespace aXon.Worker
{
	public class BenchmarkWorker:ITaskWorker
	{
		public BenchmarkWorker ()
		{
			try {
				BsonClassMap.RegisterClassMap<BenchmarkResult> ();
			} catch {
			}
		}

		public const int ROW_COUNT = 100000;
		public const int INPUT_COUNT = 10;
		public const int OUTPUT_COUNT = 1;
		public const int HIDDEN_COUNT = 20;
		public const int ITERATIONS = 10;

		#region ITaskWorker implementation

		public void Execute (Guid taskId)
		{
			try {
				double[][] input = Generate (ROW_COUNT, INPUT_COUNT);
				double[][] output = Generate (ROW_COUNT, OUTPUT_COUNT);
				DateTime start = DateTime.Now;
				long avg = 0;
				var sw = new Stopwatch ();
				sw.Start ();
				for (int i = 0; i < 10; i++) {
					long time = BenchmarkEncog (input, output);
					RaiseOnProgress (new OnProgressArgs () {
						CurrentTime = DateTime.Now,
						PercentComplete = (i * 10),
						StartTime = start,
						Status = TaskStatus.InProcess,
						TaskId = taskId
					});
					avg += time;
					
				}
				sw.Stop ();
				avg = avg / 10;
				RaiseOnComplete (new OnCompletionArgs (){ TaskId = taskId, Totaltime = sw.ElapsedMilliseconds, Average = avg });
                //var db = client.GetDatabase ("Results");
                //var collection = db.GetCollection<BenchmarkResult> ("Benchmark");
                //collection.InsertOneAsync (new BenchmarkResult () { Id = taskId, Average = avg, Totaltime = sw.ElapsedMilliseconds });

			} catch (SystemException e) {
				RaiseOnErrorOccured (new OnErrorArgs (){ TaskId = taskId, Error = e.Message });
			}
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

		public static long BenchmarkEncog (double[][] input, double[][] output)
		{
			var network = new BasicNetwork ();
			network.AddLayer (new BasicLayer (null, true,
				input [0].Length));
			network.AddLayer (new BasicLayer (new ActivationSigmoid (), true,
				HIDDEN_COUNT));
			network.AddLayer (new BasicLayer (new ActivationSigmoid (), false,
				output [0].Length));
			network.Structure.FinalizeStructure ();
			network.Reset (23); // constant seed for repeatable testing

			IMLDataSet trainingSet = new BasicMLDataSet (input, output);

			// train the neural network
			IMLTrain train = new Backpropagation (network, trainingSet, 0.7, 0.7);

			var sw = new Stopwatch ();
			sw.Start ();
			// run epoch of learning procedure
			for (int i = 0; i < ITERATIONS; i++) {
				train.Iteration ();
			}
			sw.Stop ();

			return sw.ElapsedMilliseconds;
		}

		private static double[][] Generate (int rows, int columns)
		{
			double[][] result = EngineArray.AllocateDouble2D (rows, columns);
			var rand = new Random (42); // same value every time for a benchmark

			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < columns; j++) {
					result [i] [j] = rand.NextDouble ();
				}
			}

			return result;
		}
	}
}

