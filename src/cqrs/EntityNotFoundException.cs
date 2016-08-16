using System;

namespace cqrs
{
    public class EntityNotFoundException : Exception
    {
        private readonly Guid entityId;
        private readonly string entityType;

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(Guid entityId) : base(entityId.ToString())
        {
            this.entityId = entityId;
        }

        public EntityNotFoundException(Guid entityId, string entityType)
            : base(entityType + ": " + entityId.ToString())
        {
            this.entityId = entityId;
            this.entityType = entityType;
        }

        public EntityNotFoundException(Guid entityId, string entityType, string message, Exception inner)
            : base(message, inner)
        {
            this.entityId = entityId;
            this.entityType = entityType;
        }


        public Guid EntityId
        {
            get { return this.entityId; }
        }

        public string EntityType
        {
            get { return this.entityType; }
        }
    }
}
