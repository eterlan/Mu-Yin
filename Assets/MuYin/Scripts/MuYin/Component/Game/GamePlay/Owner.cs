using Unity.Entities;

namespace MuYin
{
    public struct Owner : IComponentData
    {
        public Entity OwnerEntity;
    }
}