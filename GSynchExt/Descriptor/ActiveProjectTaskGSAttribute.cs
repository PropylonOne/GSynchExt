using PX.Data;
using System;

namespace PX.Objects.PM
{
    [PXRestrictor(typeof(Where<PMTask.isCancelled, Equal<False>>), "Task is Canceled and cannot be used for data entry.", new[] { typeof(PMTask.taskCD) })]
    [PXRestrictor(typeof(Where<PMTask.isCompleted, Equal<False>>), "Task is Completed and cannot be used for data entry.", new[] { typeof(PMTask.taskCD) })]
	[PXRestrictor(typeof(Where<PMTask.isActive, Equal<True>>), "Task is not Active and cannot be used for data entry.", new[] { typeof(PMTask.taskCD) })]

	public class ActiveProjectTaskGSAttribute : BaseProjectTaskAttribute
    {
		public ActiveProjectTaskGSAttribute(Type projectID) : base(projectID)
		{
			Filterable = true;
		}

		public ActiveProjectTaskGSAttribute(Type projectID, string Module) : base(projectID, Module)
		{
			Filterable = true;
		}
	}
}