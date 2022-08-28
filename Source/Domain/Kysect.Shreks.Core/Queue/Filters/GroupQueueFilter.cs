using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class GroupQueueFilter : SubmissionQueueFilter
{
    public GroupQueueFilter(IReadOnlyCollection<StudentGroup> groups)
    {
        Groups = groups;
    }

    public virtual IReadOnlyCollection<StudentGroup> Groups { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(s => Groups.Contains(s.Student.Group));
}