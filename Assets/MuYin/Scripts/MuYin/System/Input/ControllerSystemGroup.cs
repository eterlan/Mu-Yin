using Unity.Entities;

namespace MuYin
{
    [UpdateAfter(typeof(StreamingInputSystem))]
    public class ControllerSystemGroup : ComponentSystemGroup
    {
        
    }
}