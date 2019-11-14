using Unity.Entities;

namespace MuYin.Controller.System
{
    [UpdateAfter(typeof(StreamingInputSystem))]
    public class ControllerSystemGroup : ComponentSystemGroup
    {
        
    }
}