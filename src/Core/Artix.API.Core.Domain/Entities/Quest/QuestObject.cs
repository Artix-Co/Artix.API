namespace Artix.API.Core.Domain.Entities.Quest;

using Common;
using Object;

public class QuestObject : BaseEntity
{
    public Guid QuestId { get; private set; } // لینک به Quest.Id در Mongo
    public long ObjectId { get; private set; } // FK to Object.Id در SQL
    public virtual Object Object { get; private set; } // Navigation property
    public int Order { get; private set; } // ترتیب یا وزن Object در Quest (مثل ترتیب اسکن)

    protected QuestObject() { }

    public QuestObject(Guid questId, long objectId, int order = 0)
    {
        QuestId = questId;
        ObjectId = objectId;
        Order = order;
    }

    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
    }
}
