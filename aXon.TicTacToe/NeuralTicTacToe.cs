using System;
using aXon.TicTacToe.Players;
using aXon.TicTacToe.Game;
using Encog.Neural.Pattern;
using Encog.Neural.Networks;
using Encog.Engine.Network.Activation;

using Encog.Util;
using Encog.Util.Validate;
using System.IO;
using MongoDB.Driver;
using System.Diagnostics;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Encog.ML.Genetic;
using Encog.ML;
using Encog.ML.Train;

namespace aXon.TicTacToe
{
	public class NeuralTicTacToe
	{
		public const int POPULATION_SIZE = 200;
		public const double MUTATION_PERCENT = 0.10;
		public const double MATE_PERCENT = 0.25;
		public const int NEURONS_HIDDEN_1 = (Board.SIZE * Board.SIZE) * 2;
		public const int TRAIN_MINUTES = 60;
		public const int SCORE_SAMPLE = 100;
		public const int THREAD_POOL_SIZE = 8;
		private GameType _t;

		public NeuralTicTacToe (GameType t, Player p1, Player p2)
		{
			player1 = p1;
			player2 = p2;
			_t = t;

		}

		public void Execute ()
		{
			switch (_t) {
			case GameType.Game:
				singleGame ();
				break;
			case GameType.Match:
				playMatch ();
				break;
			case GameType.Train:
				geneticNeural ();
				break;
			}
		}

		public static BasicNetwork loadNetwork ()
		{
			MongoServer server = new MongoServer (MongoServerSettings.FromUrl (new MongoUrl ("mongodb://Dev-Svr2.systest.sc2services.com")));
			var fs = new MongoDB.Driver.GridFS.MongoGridFS (server.GetDatabase ("Results"));
			var file = fs.OpenRead ("tttnetwork.net");
			var lf = System.IO.File.OpenWrite ("tttnetwork.net");
			var l = file.Length;
			for (int x = 0; x < l; x++) {
				lf.WriteByte (Convert.ToByte (file.ReadByte ()));	
			}
			lf.Close ();
			file.Close ();
			var network = SerializeObject.Load ("tttnetwork.net");
			System.IO.File.Delete ("tttnetwork.net");
			return (BasicNetwork)network;
		}

		public static BasicNetwork createNetwork ()
		{
			var pattern = new FeedForwardPattern { InputNeurons = (Board.SIZE * Board.SIZE) };
			pattern.AddHiddenLayer (NEURONS_HIDDEN_1);
			pattern.OutputNeurons = 1;
			pattern.ActivationFunction = new ActivationTANH ();
			var network = (BasicNetwork)pattern.Generate ();
			network.Reset ();
			return network;
		}

		public static void SaveNetwork (BasicNetwork net)
		{
			MongoServer server = new MongoServer (MongoServerSettings.FromUrl (new MongoUrl ("mongodb://Dev-Svr2.systest.sc2services.com")));
			var fs = new MongoDB.Driver.GridFS.MongoGridFS (server.GetDatabase ("Results"));
			var file = fs.Open ("tttnetwork.net", FileMode.OpenOrCreate);
			SerializeObject.Save ("tttnetwork.net", net);
			var lf = System.IO.File.OpenRead ("tttnetwork.net");
			var l = lf.Length;
			for (int x = 0; x < l; x++) {
				file.WriteByte (Convert.ToByte (lf.ReadByte ()));	
			}
			lf.Close ();
			file.Close ();
			System.IO.File.Delete ("tttnetwork.net");
		}

		public static Player getPlayer (PlayerType name)
		{
			switch (name) {
			case PlayerType.Boring:
				return new PlayerBoring ();
				break;
			case PlayerType.Logic:
				return new PlayerLogic ();
				break;
			case PlayerType.MinMax:
				return new PlayerMinMax ();
				break;
			case PlayerType.NeuralBlank:
				return new PlayerNeural (createNetwork ());
				break;
			case PlayerType.NeuralLoad:
				return new PlayerNeural (loadNetwork ());
				break;
			case PlayerType.Random:
				return new PlayerRandom ();
				break;
			case PlayerType.Human:
				return new PlayerHuman ();
				break;
			}
			return new PlayerBoring ();
		}

		private Player player1;

		private Player player2;

		public void playMatch ()
		{
			Player[] players = new Player[2];
			players [0] = this.player1;
			players [1] = this.player2;
			ScorePlayer score = new ScorePlayer (players [0], players [1], true);
			Console.WriteLine ("Score:" + score.score ());
		}



		public void singleGame ()
		{
			Game.Game game = null;

			Player[] players = new Player[2];
			players [0] = this.player1;
			players [1] = this.player2;
			game = new Game.Game (players);
			Player winner = game.play ();
			Console.WriteLine ("Winner is:" + winner);
		}

		public void geneticNeural ()
		{
			IMLTrain train = new MLMethodGeneticAlgorithm (() => {
				BasicNetwork result = ((PlayerNeural)player1).network;
				((IMLResettable)result).Reset ();
				return result;
			}, new ScorePlayer (player1, player2, false), POPULATION_SIZE);

			int epoch = 1;

			DateTime started = DateTime.Now;

			int minutes = 0;
			do {
				train.Iteration ();

				TimeSpan span = (DateTime.Now - started);
				minutes = span.Minutes;

				Console.WriteLine ("Epoch #" + epoch + " Error:" + train.Error);
				epoch++;

			} while (minutes <= NeuralTicTacToe.TRAIN_MINUTES);
			SaveNetwork ((BasicNetwork)train.Method);
		}
	
	}
}

