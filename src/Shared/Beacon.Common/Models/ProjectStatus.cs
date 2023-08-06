﻿namespace Beacon.Common.Models;

public enum ProjectStatus
{
    Active,
    Completed,
    Canceled,
    Expired,
    OnHold,
    Pending,
    Preliminary,
    Quoted
}

public static class ProjectStatusExtensions
{
    public static string GetDescription(this ProjectStatus status) => status switch
    {
        ProjectStatus.OnHold => "On Hold",
        _ => status.ToString()
    };
}