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

namespace aXon.TicTacToe
{
	public enum PlayerType
	{
		MinMax,
		Boring,
		Random,
		NeuralLoad,
		NeuralBlank,
		Logic,
		Human
	}
	
}
