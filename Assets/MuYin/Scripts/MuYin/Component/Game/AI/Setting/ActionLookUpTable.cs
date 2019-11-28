// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Entities;

// namespace MuYin
// {
//     // TODO change to BlobAsset?
//     public class ActionLookUpTable
//     {
//         public static   ActionLookUpTable                       Instance;
//         public          NativeHashMap<int, ComponentType>       ActionTagsHashMap;
//         public          NativeHashMap<int, ActionExtraInfo>     ActionExtraInfosHashMap;
//         public readonly Dictionary<ActionType, ActionExtraInfo> ActionExtraInfosDictionary;
//         public readonly Dictionary<ActionType, ComponentType>   ActionTagsDictionary;

//         public ActionLookUpTable
//         (
//             Dictionary<ActionType, ActionExtraInfo> actionExtraInfosDictionary,
//             Dictionary<ActionType, ComponentType>   actionActionTagsDictionary,
//             NativeHashMap<int, ComponentType>       actionTagsHashMap,
//             NativeHashMap<int, ActionExtraInfo>     actionExtraInfosHashMap)
//         {
//             Instance                   = this;
//             ActionExtraInfosDictionary = actionExtraInfosDictionary;
//             ActionTagsDictionary       = actionActionTagsDictionary;
//             ActionTagsHashMap          = actionTagsHashMap;
//             ActionExtraInfosHashMap    = actionExtraInfosHashMap;
//         }

//         public void AddNewAction(ActionType actionType, ComponentType actionTag, ActionExtraInfo actionExtraInfo)
//         {
//             AddActionTag(actionType, actionTag);
//             AddExtraInfo(actionType, actionExtraInfo);
//         }

//         public void AddNewAction(ActionType actionType, ComponentType actionTag)
//         {
//             AddActionTag(actionType, actionTag);
//             AddExtraInfo(actionType, ActionExtraInfo.Null);
//         }

//         private void AddActionTag(ActionType actionType, ComponentType actionTag)
//         {
//             ActionTagsDictionary.Add(actionType, actionTag);
//         }

//         private void AddExtraInfo(ActionType actionType, ActionExtraInfo actionExtraInfo)
//         {
//             ActionExtraInfosDictionary.Add(actionType, actionExtraInfo);
//         }
//     }
// }