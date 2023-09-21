using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class SkinItem : MonoBehaviour
{
    public Skin skin;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skinPriceText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image skinImage;

    public delegate void SkinItemDelegate();
    public static event SkinItemDelegate OnEquip;

    private void OnEnable()
    {
        YandexGame.GetDataEvent += UpdateUI;
        OnEquip += UpdateUI;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= UpdateUI;
        OnEquip -= UpdateUI;
    }

    private void Awake()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!YandexGame.SDKEnabled && skin == null) return;

        skinPriceText.text = $"<sprite name=\"Robux\">{skin.skinPrice}";
        skinImage.sprite = skin.skinIcon;

        if(Purchased())
        {
            if(skin.skinID == YandexGame.savesData.equippedSkinId) buttonText.text = "equipped";
            else buttonText.text = "equip";
        }
        else buttonText.text = "buy";
    }

    private bool Purchased()
    {
        foreach (int id in YandexGame.savesData.purchasedSkinsId)
        {
            if (id == skin.skinID)
            {
                return true;
            }
        }

        return false;
    }

    public void Buy()
    {
        if (skin.skinID == YandexGame.savesData.equippedSkinId) return;
        
        if (YandexGame.savesData.robux >= skin.skinPrice && !Purchased())
        {
            Game.instance.Robux(-skin.skinPrice);

            Array.Resize(ref YandexGame.savesData.purchasedSkinsId, YandexGame.savesData.purchasedSkinsId.Length + 1);
            YandexGame.savesData.purchasedSkinsId[YandexGame.savesData.purchasedSkinsId.Length - 1] = skin.skinID;
            YandexGame.SaveProgress();
        }

        if (skin.skinID != YandexGame.savesData.equippedSkinId && Purchased())
        {
            YandexGame.savesData.equippedSkinId = skin.skinID;
            YandexGame.SaveProgress();

            OnEquip?.Invoke();
        }

        UpdateUI();
    }
}
