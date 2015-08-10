using System;
using MongoDB.Driver;
using aXon.Worker.Delegates;

namespace aXon.Worker.Interfaces
{

	public interface ITaskWorker
	{
	    Guid Id{get;set;}
	    void Execute(Guid taskId,MongoClient client);
	    event OnError ErrorOccured;
	    event OnProgress Progress;
	    event OnCompletion Complete;
	}
}

