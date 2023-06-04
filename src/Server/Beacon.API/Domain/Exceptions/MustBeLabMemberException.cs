namespace Beacon.API.Domain.Exceptions;

public class MustBeLabMemberException : Exception
{
    public Guid LaboratoryId { get; }

    public MustBeLabMemberException(Guid laboratoryId, string message = "The current user is not a member of this lab.")
        : base(message)
    {
        LaboratoryId = laboratoryId;
    }
}
