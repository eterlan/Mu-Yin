using System;
using Unity.Collections;
using Unity.Entities;

namespace MuYin
{
   public class InventoryUtilityMethod
    {
        public static int? FindItemInInventory(ItemType itemType, ref DynamicBuffer<Inventory> inventory, SearchStrategy strategy)
        {
            var itemList = new NativeList<int>(Allocator.Temp); 
            for (var i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ItemType == itemType)
                {
                    itemList.Add(i);
                }
            }
            // TODO implement SearchStrategy later. 
            if (itemList.Length > 0)
            {
                return itemList[0];
            }

            return null;
        }

        public static bool ConsumeItemInInventory(int itemIndex, ref DynamicBuffer<Inventory> inventory)
        {
            if (inventory.Length <= itemIndex)
                return false;
            var item = inventory[itemIndex];
            // TODO : If Inventory checker clean all item with 0 count, I don't need to check it here right?
            if (item.Count - 1 < 0)
                return false;

            item.Count --;
            inventory[itemIndex] = item;
            return true;
        }

        internal static ItemData GetItemInfo(uint itemId, ref BlobAssetReference<ItemDataSet> itemDataSet)
        {
            ref var itemArray = ref itemDataSet.Value.ItemDatas;
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i].ItemID != itemId)
                    continue;
                
                return itemArray[i];
            }
            return new ItemData();
        }
    }
        
    public enum SearchStrategy
    {
        Default,
        Cheapest,
    }
}
