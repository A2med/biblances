using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowableHarvestBehaviour : InteractableObject
{
    CropBehaviour parentCrop;
    bool isHarvestable = false;

    //Sets the parent crop
    public void SetParent(CropBehaviour parentCrop)
    {
        this.parentCrop = parentCrop;
    }

    public void HarvestStatus(bool status)
    {
        isHarvestable = status;
    }

    public override void Pickup()
    {
        //To ensure that the player can harvest it
        if (isHarvestable)
        {
            //Set the player's inventory to the item
            InventoryManager.Instance.EquipHandSlot(item);

            //Update the changes in the scene
            InventoryManager.Instance.RenderHand();
            UIManager.Instance.RenderInventory();

            //To ensure that the player cannot harvest it after switching it's state to a seedling
            HarvestStatus(false);

            //Set the parent crop back to seedling to regrow it
            parentCrop.Regrow();
        }
    }
}