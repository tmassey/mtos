using System;
using aXon.Worker.Interfaces;
using aXon.TicTacToe;

namespace aXon.Worker
{
	public class TickTackToeTrainWorker:ITaskWorker
	{
		public TickTackToeTrainWorker ()
		{
		}

		#region ITaskWorker implementation

		public event aXon.Worker.Delegates.OnError ErrorOccured;

		protected virtual void OnErrorOccured (aXon.Worker.EventArgs.OnErrorArgs args)
		{
			var handler = ErrorOccured;
			if (handler != null)
				handler (this, args);
		}

		public event aXon.Worker.Delegates.OnProgress Progress;

		protected virtual void OnProgress (aXon.Worker.EventArgs.OnProgressArgs args)
		{
			var handler = Progress;
			if (handler != null)
				handler (this, args);
		}

		public event aXon.Worker.Delegates.OnCompletion Complete;

		protected virtual void OnComplete (aXon.Worker.EventArgs.OnCompletionArgs args)
		{
			var handler = Complete;
			if (handler != null)
				handler (this, args);
		}

		public void Execute (Guid taskId, MongoDB.Driver.MongoClient client)
		{
			aXon.TicTacToe.NeuralTicTacToe ttt = new aXon.TicTacToe.NeuralTicTacToe (GameType.Train, NeuralTicTacToe.getPlayer (PlayerType.NeuralBlank), NeuralTicTacToe.getPlayer (PlayerType.Logic));

			ttt.Execute ();
		}

		public Guid Id { get; set; }

		#endregion
	}
}

