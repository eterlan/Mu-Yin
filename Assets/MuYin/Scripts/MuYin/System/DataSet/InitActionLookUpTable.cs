// using System;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Entities;

// namespace MuYin
// {
//     [DisableAutoCreation]
//     public class InitActionLookUpTable : ComponentSystem
//     {
//         protected override void OnUpdate()
//         {
//             FillActionLookUpTable();
//         }

//         private void FillActionLookUpTable()
//         {
//             // Because I need to write to the HashMap, so Complete jobs which use it is necessary.
//             World.GetOrCreateSystem<SelectActionSystem>().SelectActionJobHandle.Complete();
//             FillActionTags();
//             FillActionExtraInfos();
//         }

//         private void FillActionTags()
//         {
//             var tagsHashMap = ActionLookUpTable.Instance.ActionTagsHashMap;
//             // Already added.
//             if (tagsHashMap.Length != 0)
//                 return;

//             Dict2HashMap(ActionLookUpTable.Instance.ActionTagsDictionary, tagsHashMap);
//         }

//         private void FillActionExtraInfos()
//         {
//             var extraInfosHashMap = ActionLookUpTable.Instance.ActionExtraInfosHashMap;
//             // Already added.
//             if (extraInfosHashMap.Length != 0)
//                 return;

//             Dict2HashMap(ActionLookUpTable.Instance.ActionExtraInfosDictionary, extraInfosHashMap);
//         }

//         public static void Dict2HashMap<TDictKey, THashMapKey, TValue>
//         (
//             Dictionary<TDictKey, TValue>       dictionary,
//             NativeHashMap<THashMapKey, TValue> hashMap)
//             where TDictKey : IConvertible
//             where THashMapKey : struct, IEquatable<THashMapKey>
//             where TValue : struct
//         {
//             foreach (var pair in dictionary)
//             {
//                 var key = (THashMapKey) Convert.ChangeType(pair.Key, typeof(THashMapKey));
//                 hashMap.TryAdd(key, pair.Value);
//             }
//         }

//         public new void OnCreate()
//         {
//             var capacity = System.Enum.GetValues(typeof(ActionType)).Length;
//             var unused = new ActionLookUpTable(
//                 new Dictionary<ActionType, ActionExtraInfo>(),
//                 new Dictionary<ActionType, ComponentType>(),
//                 new NativeHashMap<int, ComponentType>(capacity, Allocator.Persistent),
//                 new NativeHashMap<int, ActionExtraInfo>(capacity, Allocator.Persistent));
//         }

//         protected override void OnDestroy()
//         {
//             ActionLookUpTable.Instance.ActionTagsHashMap.Dispose();
//             ActionLookUpTable.Instance.ActionExtraInfosHashMap.Dispose();
//         }
//     }
// }