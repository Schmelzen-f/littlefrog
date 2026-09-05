using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    //Stack item
    public int quantity = 1;
    private TMP_Text quantityText;


    //Shop fields
    public int buyPrice = 10;
    [Range(0f, 1f)]
    public float sellPriceMultiplier = 1f; // sell for 100%
     



    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }


    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }



    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }


    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }



    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(quantity, amount);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }


    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickUpController.Instance != null)
        {
            ItemPickUpController.Instance.ShowItemPickup(Name, itemIcon);
        }

    }
}
