using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class inventoryItem 
{

    public inventoryItem(ItemData data, int count)
    {
        itemData = data;
        numberOfTheItem = count;

    }
    [SerializeField]
    public ItemData itemData;
    [SerializeField]

    public int numberOfTheItem;



}


[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory", order = 0)]
public class Inventory : ScriptableObject
{
    private bool _isOpen;
    [SerializeField] List<inventoryItem> items = new List<inventoryItem>();
    public Inventory()
    {

    }

    public   Inventory(List<inventoryItem> _items) 
    {   
        items = _items;

    }





    public void deleteItem(ItemData data)
    {
        int foundIndex = items.FindIndex((a) => a.itemData == data);
        if (foundIndex != -1)
        {
            if (items[foundIndex].numberOfTheItem > 1)
            {
                items[foundIndex].numberOfTheItem--;
            }
            else
            {

                items.RemoveAt(foundIndex);
            }
        }
        else
        {
            Debug.LogError("item not " + data + " in " + this);
        }

    }



    public void addItem(ItemData data)
    {
        int foundIndex = items.FindIndex((a) => a.itemData == data);
        if (foundIndex == -1)
        {
            items.Add(new inventoryItem(data, 1));
        }
        else
        {
            items[foundIndex].numberOfTheItem++;
        }
    }

    public void OpenInventory()
    {
        _isOpen = !_isOpen;
        gameobject.setactive(_isOpen);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

public class Store
{
    private Inventory shop;
}