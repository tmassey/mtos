using System;

namespace aXon.TaskTransport
{
	public enum TaskStatus
	{
		Arrived,
		Pending,
		Starting,
		InProcess,
		SavingResults,
		Complete,
        Failed
	}
	
}
