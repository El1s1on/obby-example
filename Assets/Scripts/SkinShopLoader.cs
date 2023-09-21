using System.Linq;
using UnityEngine;

public class SkinShopLoader : MonoBehaviour
{
    [SerializeField] private float maxItemsInLine;
    [SerializeField] private Transform shopContent;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private GameObject itemsLinePrefab;
    private Skin[] skins;

    private void Start()
    {
        skins = Resources.LoadAll<Skin>("ScriptableObjects/Skins");
        GenerateShop();
    }

    private void GenerateShop()
    {
        if (skins == null || skins.Length <= 0) return;

        var sortedSkins = skins.OrderBy(item => item.skinPrice);

        int loadedItemsCount = 0;
        Transform itemsLine = null;

        foreach (var i in sortedSkins)
        {
            if (loadedItemsCount >= maxItemsInLine)
            {
                loadedItemsCount = 0;
                itemsLine = null;
            }

            if(itemsLine == null)
            {
                itemsLine = Instantiate(itemsLinePrefab, shopContent).transform;
            }

            SkinItem skinItem = Instantiate(skinItemPrefab, itemsLine).GetComponent<SkinItem>();
            skinItem.skin = i;
            skinItem.UpdateUI();

            loadedItemsCount++;
        }
    }
}
