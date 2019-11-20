using MuYin.AI.ActionProcessor;
using MuYin.AI.Systems;
using Unity.Entities;

namespace MuYin.AI.Action
{
    [UpdateAfter(typeof(AISystemGroup))]
    public class ActionProcessorGroup : ComponentSystemGroup
    {
    
    }
    
    // Action Implementation Should Update before the GAP-FSM and let it handle the state shift.  
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    [UpdateBefore(typeof(GeneralActionProcessor))]
    public class ConcreteActionProcessorGroup : ComponentSystemGroup
    {
    
    }
}
