using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectManagementSystemAPI.ViewModels
{
    public class TaskModel
    {
        public string Status { get; set; }
        public double Progress { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public int DeveloperId { get; set; }

        public static implicit operator Task(TaskModel model)
        {
            Task t = new Task();
            t.Id = 0;
            t.Deadline = model.Deadline;
            t.Description = model.Description;
            t.Progress = model.Progress;

            if (model.Status == "new")
            {
                t.Status = Data.Models.Status.New;
            }
            else if (model.Status == "in progress")
            {
                t.Status = Data.Models.Status.InProgress;
            }
            else if (model.Status == "finished")
            {
                t.Status = Data.Models.Status.Finished;
            }

            return t;
        }
    }
}
