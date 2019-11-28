using Unity.Entities;

namespace MuYin
{
    public struct SetPlaceOwnerEvent : IComponentData
    {
        public Entity OwnerEntity;
        public Entity ObjectEntity;
        public bool   IsForce;

        public SetPlaceOwnerEvent (Entity ownerEntity, Entity objectEntity, bool isForce): this()
        {
            OwnerEntity  = ownerEntity;
            ObjectEntity = objectEntity;
            IsForce      = isForce;
        }
        // Set by system.
        public bool IsLegal;
        public bool HasOwner;
        public bool IsValidate;
    }
}

