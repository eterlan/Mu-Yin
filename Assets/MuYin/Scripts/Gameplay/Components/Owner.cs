using Unity.Entities;

namespace MuYin.Gameplay.Components
{
    public struct Owner : IComponentData
    {
        public Entity OwnerEntity;
    }
}