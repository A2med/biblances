using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopListingManager : ListingManager<ItemData>
{
    //Variables to keep track of what the player is trying to purchase (selection)
    ItemData itemToBuy;
    int quantity;

    [Header("Confirmation Screen")]
    public GameObject confirmationScreen;
    public Text confirmationPrompt;
    public Text quantityText;
    public Text costCalculationText;
    public Button purchaseButton;

    protected override void DisplayListing(ItemData listingItem, GameObject listingGameObject)
    {
        listingGameObject.GetComponent<ShopListing>().Display(listingItem);
    }

    public void OpenConfirmationScreen(ItemData item)
    {
        itemToBuy = item;
        quantity = 1;
        RenderConfirmationScreen();
    }

    public void RenderConfirmationScreen()
    {
        confirmationScreen.SetActive(true);

        confirmationPrompt.text = $"{itemToBuy.name} ﻯﺮﺘﺷﺇ";

        quantityText.text = "x" + quantity;

        int cost = itemToBuy.cost * quantity;

        int playerMoneyLeft = PlayerStats.Money - cost;

        //Stop the player from purchasing the item if he does not have enough money
        if(playerMoneyLeft < 0)
        {
            costCalculationText.text = "ِﻑﺎﻛ ﺮﻴﻏ ﺪﻴﺻﺭ";
            purchaseButton.interactable = false;
            return;
        }

        purchaseButton.interactable = true;

        costCalculationText.text = $"{PlayerStats.Money} > {playerMoneyLeft} ";
    }

    public void AddQuantity()
    {
        quantity++;
        RenderConfirmationScreen();
    }

    public void SubstractQuantity()
    {
        if(quantity > 1)
        {
            quantity--;
        }
        RenderConfirmationScreen();
    }

    //Purchase the item and close the confirmation screen
    public void ConfirmPurchase()
    {
        Shop.Purchase(itemToBuy, quantity);
        confirmationScreen.SetActive(false);
    }

    public void CancelPurchase()
    {
        confirmationScreen.SetActive(false);
    }
}
