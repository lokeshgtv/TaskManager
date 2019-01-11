using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace TaskManagerData
{
    public partial class TaskManagerEntities : DbContext
    {
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
