using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Talos.Objects;

internal sealed class Inventory : IEnumerable<Item>, IEnumerable
{

    private Item[] item;

    internal int MAX_ITEMS
    {
        get;
        private set;
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
    internal Item this[string itemName] => HasItemReturnSlot(itemName);


    internal Inventory(int max)
    {
        MAX_ITEMS = max;
        item = new Item[max];
    }

    internal bool FullInventory => item.Count((Item item) => item != null) == 59;
    internal bool HasItem(string itemName)
    {
        for (int i = 0; i < MAX_ITEMS; i++)
            if (item[i] != null && item[i].Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                return true;
        return false;
    }

    internal int HasItemReturnCount(string string_0, bool bool_0 = true)
    {
        int totalCount = 0;
        for (int i = 0; i < MAX_ITEMS; i++)
            if (item[i] != null && item[i].Name.Equals(string_0, StringComparison.CurrentCultureIgnoreCase))
                totalCount += ((!bool_0) ? 1 : item[i].Quantity);
        return totalCount;
    }

    internal Item HasItemReturnSlot(string itemName)
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
