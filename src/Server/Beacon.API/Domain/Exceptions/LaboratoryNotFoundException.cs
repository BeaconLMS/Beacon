namespace Beacon.API.Domain.Exceptions;

public class LaboratoryNotFoundException : Exception
{
    public LaboratoryNotFoundException(Guid labId) : base($"Laboratory with id {labId} was not found.")
    {
    }
}
