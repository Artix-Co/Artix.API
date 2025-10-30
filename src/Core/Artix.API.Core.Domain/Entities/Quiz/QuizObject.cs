namespace Artix.API.Core.Domain.Entities.Quiz;

using Common;
using Object;

public class QuizObject : BaseEntity
{
    public Guid QuestId { get; private set; } // لینک به Quest.Id در Mongo
    public long ObjectId { get; private set; } // FK to Object.Id در SQL
    public virtual Object Object { get; private set; } // Navigation property
    public int Order { get; private set; } // ترتیب یا وزن Object در Quest (مثل ترتیب اسکن)

    protected QuizObject() { }

    public QuizObject(Guid questId, long objectId, int order = 0)
    {
        this.QuestId = questId;
        this.ObjectId = objectId;
        this.Order = order;
    }

    public void UpdateOrder(int newOrder)
    {
        this.Order = newOrder;
    }
}
