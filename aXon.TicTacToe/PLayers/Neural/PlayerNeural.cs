using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aXon.TicTacToe.Game;
using Encog.Neural.Networks;
using Encog.ML.Data;
using Encog.ML.Data.Basic;



namespace aXon.TicTacToe.Players
{
	class PlayerNeural: Player
	{

		public  BasicNetwork network{ get; set; }

		public PlayerNeural (BasicNetwork network)
		{
			this.network = network;
		}

		public Move getMove (int[,] board, Move prev, int color)
		{
			Move bestMove = null;
			double bestScore = double.MinValue;

			for (int x = 0; x < Board.SIZE; x++) {
				for (int y = 0; y < Board.SIZE; y++) {
					Move move = new Move ((int)x, (int)y, color);
					if (Board.isEmpty (board, move)) {
						double d = tryMove (board, move);
						if ((d > bestScore) || (bestMove == null)) {
							bestScore = d;
							bestMove = move;
						}
					}
				}
			}

			return bestMove;

		}

		private double tryMove (int[,] board, Move move)
		{
			var input = new BasicMLData (Board.SIZE * Board.SIZE);
			int index = 0;

			for (int x = 0; x < Board.SIZE; x++) {
				for (int y = 0; y < Board.SIZE; y++) {
					if (board [x, y] == aXon.TicTacToe.Game.TicTacToe.NOUGHTS) {
						input [index] = -1;
					} else if (board [x, y] == aXon.TicTacToe.Game.TicTacToe.CROSSES) {
						input [index] = 1;
					} else if (board [x, y] == aXon.TicTacToe.Game.TicTacToe.EMPTY) {
						input [index] = 0;
					}

					if ((x == move.x) && (y == move.y)) {
						input [index] = -1;
					}

					index++;
				}
			}
			//var input = new BasicMLData(Board.SIZE*Board.SIZE);

			IMLData output = this.network.Compute (input);
			return output [0];
		}

	}
}
