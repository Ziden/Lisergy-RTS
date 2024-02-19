using Game.Systems.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Systems.Harvesting
{
    /// <summary>
    /// Snapshot of the current harvesting state of the given entity.
    /// This is to help with harvesting predictions.
    /// TODO: Make a generic "logical task" system that can be used to do client-prediction like
    /// harvesting system
    /// </summary>
    public unsafe struct EntityTaskState
    {
        /// <summary>
        /// Time the task has started
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// Time the task is suposed to end
        /// </summary>
        public DateTime EndTime;

        /// <summary>
        /// The total time the entity already spent on the task
        /// </summary>
        public TimeSpan TimeSpentOnTask;

        /// <summary>
        /// Amount of time remaining to finish the task
        /// </summary>
        public TimeSpan TimeToFinishTask;

        /// <summary>
        /// Gets the total time required for this task
        /// </summary>
        public TimeSpan TotalTimeRequiredForTask;
    }
}
