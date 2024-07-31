using System;
using UnityEngine;

public class PlayerShopUIManager : MonoBehaviour {
    [SerializeField] private EventSubscribeButton shopButton;
    [SerializeField] private EventSubscribeButton startButton;
    [SerializeField] private EventSubscribeButton inventoryButton;
    private HideShowUI shopButtonHideShow;
    private HideShowUI startButtonHideShow;
    private HideShowUI inventoryButtonHideShow;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ShopUI inventoryUI;
    bool shopOpen = false;
    bool inventoryOpen = false;
    bool buttonsOpen = true;
    public void Init(Action startLevelCallback)
    {
        shopButtonHideShow = shopButton.GetComponent<HideShowUI>();
        startButtonHideShow = startButton.GetComponent<HideShowUI>();
        inventoryButtonHideShow = inventoryButton.GetComponent<HideShowUI>();
        shopButton.Init(OpenShop);
        startButton.Init(startLevelCallback);
        inventoryButton.Init(OpenInventory);
        shopUI.Init();
        inventoryUI.Init();
    }
    public void CloseAll()
    {
        ShowButtons();
        CloseShop();
        CloseInventory();
    }
    void OpenShop()
    {
        if(shopOpen) return;
        HideButtons();
        shopUI.ShowUI();
        shopOpen = true;
    }
    void CloseShop()
    {
        if(!shopOpen) return;
        ShowButtons();
        shopUI.HideUI();
        shopOpen = false;
    }
    void OpenInventory()
    {
        if(inventoryOpen) return;
        HideButtons();
        inventoryUI.ShowUI();
        inventoryOpen = true;
    }
    void CloseInventory()
    {
        if (!inventoryOpen) return;
        ShowButtons();
        inventoryUI.HideUI();
        inventoryOpen = false;
    }
    void ShowButtons()
    {
        if (buttonsOpen) return;
        shopButtonHideShow.ShowUI();
        startButtonHideShow.ShowUI();
        inventoryButtonHideShow.ShowUI();
        buttonsOpen = true;
    }
    void HideButtons()
    {
        if (!buttonsOpen) return;
        shopButtonHideShow.HideUI();
        startButtonHideShow.HideUI();
        inventoryButtonHideShow.HideUI();
        buttonsOpen = false;
    }
}