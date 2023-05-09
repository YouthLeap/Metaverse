using AdvancedPeopleSystem;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Equipping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEquipper : Equipper
{
    public override bool Equip(Item item, int index)
    {
        Debug.Log(item.Category.name + ":" + index);

        
       // var result = base.Equip(item, index);
        switch (item.Category.name)
        {
            case "Head Wear":
                GameObject.Find("CustomizableCharacter").GetComponent<CharacterCustomization>().SetElementByIndex(CharacterElementType.Accessory, index);
                break;
            case "Car":
                GameManager.instance.mycontroller.OnTakeCar(true);
                break;
        }   
                
        return true;
    }
     

    public override void UnEquip(Item item)
    { 
        switch (item.Category.name)
        {
            case "Head Wear":
                GameObject.Find("CustomizableCharacter").GetComponent<CharacterCustomization>().SetElementByIndex(CharacterElementType.Accessory, -1);
                break;
            case "Car":
                GameManager.instance.mycontroller.OnTakeCar(false);
                break;
        }
        //base.UnEquip(index);

       
    }
}
