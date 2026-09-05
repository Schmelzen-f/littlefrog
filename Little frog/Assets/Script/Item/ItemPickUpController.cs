using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemPickUpController : MonoBehaviour
{

    public static ItemPickUpController Instance { get; private set; }//Awake

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration = 3f;

    private readonly Queue<GameObject> activePopups = new();

    private void Awake()
    { 
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple ItemPickupUIManager instance detected! Destroying the extra one.");
            Destroy(gameObject); 
        }
    }

    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }

        //Fade out and destroy
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }


    private IEnumerator FadeOutAndDestroy(GameObject popup) 
    {
        yield return new WaitForSeconds(popupDuration);
        
            if (popup == null) yield break;

            CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
            for(float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
            {
                if(popup == null) yield break;
                canvasGroup.alpha = 1f - timePassed;
                yield return null;
            }
        Destroy(popup);
    }

}
