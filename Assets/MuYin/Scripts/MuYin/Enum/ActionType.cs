using Unity.Entities;

namespace MuYin
{
    public enum ActionType
    {
        Null,
        Eat,
        Drink,
        Sleep,
    }

    // public static class ActionTypeExtension
    // {
    //     public static ComponentType GetMappingActionTag(this ActionType actionType)
    //     {
    //         switch (actionType)
    //         {
    //             case ActionType.Eat : return typeof(ConsumeActionTag);
    //             case ActionType.Drink : return typeof(ConsumeActionTag);
    //             case ActionType.Sleep : return typeof(ConsumeActionTag);
    //             default : return null;
    //         }
    //     }
    // }
}