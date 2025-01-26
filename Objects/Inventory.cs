using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Talos.Definitions;
using Talos.Objects;
internal sealed class Inventory : IEnumerable<Item>, IEnumerable
{

    private Item[] item;

    internal int MAX_ITEMS
    {
        get;
        private set;
    }
    internal int Count
    {
        get
        {
            int count = 0;
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                if (item[i] != null)
                {
                    count++;
                }
            }
            return count;
        }
    }

    internal Item this[byte slot]
    {
        get
        {
            if (slot != 0 && slot < MAX_ITEMS)
            {
                int num = slot - 1;
                return item[num];
            }
            return null;
        }
        set
        {
            if (slot != 0 && slot < MAX_ITEMS)
            {
                int num = slot - 1;
                item[num] = value;
            }
        }
    }

    internal Inventory(int max)
    {
        MAX_ITEMS = max;
        item = new Item[max];
    }

    internal bool IsFull => item.Count((Item item) => item != null) == 59;
    internal Item this[string itemName] => Find(itemName);
    internal bool Contains(string itemName)
    {
        for (int i = 0; i < MAX_ITEMS; i++)
        {
            if (item[i] != null && item[i].Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    internal int CountOf(ushort sprite, bool includeStack = true)
    {
        int totalCount = 0;
        for (int index = 0; index < MAX_ITEMS; ++index)
        {
            if (item[index] != null && item[index].Sprite - CONSTANTS.ITEM_SPRITE_OFFSET == sprite)
            {
                totalCount += includeStack ? item[index].Quantity : 1;
            }
        }
        return totalCount;
    }


    internal int CountOf(string name, bool includeStack = true)
    {
        int totalCount = 0;
        for (int index = 0; index < MAX_ITEMS; ++index)
        {
            if (item[index] != null && item[index].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                totalCount += includeStack ? item[index].Quantity : 1;
            }
        }
        return totalCount;
    }


    internal Item Find(string itemName)
    {
        for (int i = 0; i < MAX_ITEMS; i++)
            if (item[i] != null && item[i].Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                return item[i];
        return null;
    }

    public IEnumerator<Item> GetEnumerator()
    {
        for (int i = 0; i < MAX_ITEMS; i++)
        {
            if (item[i] != null)
            {
                yield return item[i];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
