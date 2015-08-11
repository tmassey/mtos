using System;
using aXon.Models.Base;
using aXon.Models.Enumerations;

namespace aXon.Models.JobModels
{
    public class Task:BaseModel
	{
        public Guid JobId { get; set; }
        public TaskStatus Status { get; set; }

	}
}

