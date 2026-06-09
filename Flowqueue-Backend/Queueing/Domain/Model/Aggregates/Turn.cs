using Flowqueue_Backend.Queueing.Domain.Model.Commands;
using Flowqueue_Backend.Queueing.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Domain.Model;

namespace Flowqueue_Backend.Queueing.Domain.Model.Aggregates;

public partial class Turn : IAuditableEntity
{
    protected Turn()
    {
        CitizenName = null!;
        CitizenDocumentNumber = null!;
        TicketCode = null!;
        Status = null!;
    }

    public Turn(CreateTurnCommand command, int turnNumber, TicketCode ticketCode)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.BranchOfficeId <= 0)
            throw new ArgumentException("Branch office id must be greater than zero.", nameof(command.BranchOfficeId));

        if (command.ServiceId <= 0)
            throw new ArgumentException("Service id must be greater than zero.", nameof(command.ServiceId));

        if (turnNumber <= 0)
            throw new ArgumentException("Turn number must be greater than zero.", nameof(turnNumber));

        BranchOfficeId = command.BranchOfficeId;
        ServiceId = command.ServiceId;
        CitizenName = NormalizeRequiredText(command.CitizenName, nameof(command.CitizenName));
        CitizenDocumentNumber = NormalizeRequiredText(command.CitizenDocumentNumber, nameof(command.CitizenDocumentNumber));
        TurnNumber = turnNumber;
        TicketCode = ticketCode ?? throw new ArgumentNullException(nameof(ticketCode));
        Status = TurnStatus.NewWaiting();
        RegisteredAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }
    public int BranchOfficeId { get; private set; }
    public int ServiceId { get; private set; }
    public string CitizenName { get; private set; }
    public string CitizenDocumentNumber { get; private set; }
    public int TurnNumber { get; private set; }
    public TicketCode TicketCode { get; private set; }
    public TurnStatus Status { get; private set; }
    public DateTimeOffset RegisteredAt { get; private set; }
    public DateTimeOffset? CalledAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public DateTimeOffset? MarkedAbsentAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Call()
    {
        if (!HasStatus(TurnStatus.Waiting))
            throw new InvalidOperationException("Only waiting turns can be called.");

        Status = TurnStatus.NewCalled();
        CalledAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (!HasStatus(TurnStatus.Called))
            throw new InvalidOperationException("Only called turns can be completed.");

        Status = TurnStatus.NewCompleted();
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (HasStatus(TurnStatus.Completed) || HasStatus(TurnStatus.Cancelled) || HasStatus(TurnStatus.Absent))
            throw new InvalidOperationException("Turn cannot be cancelled.");

        Status = TurnStatus.NewCancelled();
        CancelledAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsAbsent()
    {
        if (!HasStatus(TurnStatus.Called))
            throw new InvalidOperationException("Only called turns can be marked as absent.");

        Status = TurnStatus.NewAbsent();
        MarkedAbsentAt = DateTimeOffset.UtcNow;
    }

    private bool HasStatus(string status) => Status.Value == status;

    private static string NormalizeRequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

        return value.Trim();
    }
}
