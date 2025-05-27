using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace webapi.Models;

public partial class ResultModel
{
    public Guid ResultId { get; set; }

    public Guid AssessmentId { get; set; }

    public Guid UserId { get; set; }

    public int Score { get; set; }

    public DateTime AttemptDate { get; set; }

    public virtual AssessmentModel Assessment { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;
}
