using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class GroupAssignment : IEntity
{
    public GroupAssignment(StudentGroup group, Assignment assignment, DateOnly deadline)
        : this(groupId: group.Id, assignmentId: assignment.Id)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(assignment);

        Group = group;
        Assignment = assignment;
        Deadline = deadline;
    }

    [KeyProperty]
    public virtual StudentGroup Group { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }

    public DateOnly Deadline { get; set; }

    public override string ToString() => $"(GroupId: {GroupId}, assignmentId: {AssignmentId})";
}