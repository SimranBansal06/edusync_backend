﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace webapi.Models;

public partial class AssessmentModel
{
    public Guid AssessmentId { get; set; }

    public Guid? CourseId { get; set; }

    public string? Title { get; set; }

    public string? Questions { get; set; }

    public int? MaxScore { get; set; }

    public virtual CourseModel? Course { get; set; }

    public virtual ICollection<ResultModel> ResultModels { get; set; } = new List<ResultModel>();
}
