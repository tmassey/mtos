using aXon.Models.Base;
using aXon.Models.Enumerations;

namespace aXon.Models.JobModels
{
    public class Job:BaseModel
    {
        public string Name { get; set; }
        public JobType JobType { get; set; }
        public string Description { get; set; }
    }
}