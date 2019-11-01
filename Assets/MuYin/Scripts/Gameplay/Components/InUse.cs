using Unity.Entities;


namespace MuYin.Gameplay.Components
{
    public struct InUse : IComponentData
    {
        public Entity User;
    }
}