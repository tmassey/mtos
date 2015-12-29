using System;
using System.Linq;
using aXon.Data;
using MongoDB.Driver.Builders;
using aXon.Rover;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.Worker.Interfaces;
using MongoDB.Driver;
using aXon.Worker.Delegates;
using aXon.Worker.EventArgs;
using Encog.Util;

namespace aXon.Worker
{
	public class RoverWorker:ITaskWorker
	{
		#region ITaskWorker implementation

        public aXonEntities Mds { get; set; }
	    private DateTime _starttime;
		public void Execute (Guid taskId)
		{
		    _starttime = DateTime.Now;
		    try
		    {
		        Mds = new aXonEntities();
		        var task = Mds.TrainWarehouseNetworkTasks.FirstOrDefault(t=>t.Id== taskId);
		        if (task != null)
		        {		            
		            RobotContol c = new RobotContol(_starttime, taskId);
		            c.BuildNetwork(task.StartPosition.X, task.StartPosition.Y, task.StopPosition.X, task.StopPosition.Y,task.WareHouse);		         
		        }
		        else
		        {
		            this.RaiseOnErrorOccured(new OnErrorArgs() {Error = "Task Not Found!", TaskId = taskId});
		            this.RaiseOnProgress(new OnProgressArgs()
		                                 {
		                                     CurrentTime = DateTime.Now,
		                                     PercentComplete = 0,
		                                     Status = TaskStatus.Failed,
		                                     TaskId = taskId,
		                                     StartTime = _starttime
		                                 });
		        }
		    }
		    catch (Exception err)
		    {
                RaiseOnErrorOccured(new OnErrorArgs() { TaskId = taskId, Error = err.Message });
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

        //private static double[][] Generate (int rows, int columns)
        //{
        //    double[][] result = EngineArray.AllocateDouble2D (rows, columns);
        //    var rand = new Random (42); // same value every time for a benchmark

        //    for (int i = 0; i < rows; i++) {
        //        for (int j = 0; j < columns; j++) {
        //            result [i] [j] = rand.NextDouble ();
        //        }
        //    }

        //    return result;
        //}

		public RoverWorker ()
		{
		}
	}

}

