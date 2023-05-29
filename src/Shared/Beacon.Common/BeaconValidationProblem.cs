namespace Beacon.Common;

public class BeaconValidationProblem : BeaconResult
{
    public required Dictionary<string, string[]> Errors { get; init; }
}
