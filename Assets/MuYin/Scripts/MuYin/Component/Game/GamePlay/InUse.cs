using Unity.Entities;

namespace MuYin
{
    public struct InUse : IComponentData
    {
        public Entity User;
    }
}