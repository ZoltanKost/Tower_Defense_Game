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
        CloseShop();
        CloseInventory();
    }
    void OpenShop()
    {
        if(shopOpen) return;
        shopButtonHideShow.HideUI();
        startButtonHideShow.HideUI();
        inventoryButtonHideShow.HideUI();
        shopUI.ShowUI();
        shopOpen = true;
    }
    void CloseShop()
    {
        if(!shopOpen) return;
        shopButtonHideShow.ShowUI();
        startButtonHideShow.ShowUI();
        inventoryButtonHideShow.ShowUI();
        shopUI.HideUI();
        shopOpen = false;
    }
    void OpenInventory()
    {
        shopButtonHideShow.HideUI();
        startButtonHideShow.HideUI();
        inventoryButtonHideShow.HideUI();
        inventoryUI.ShowUI();
    }
    public void CloseInventory()
    {
        shopButtonHideShow.ShowUI();
        startButtonHideShow.ShowUI();
        inventoryButtonHideShow.ShowUI();
        inventoryUI.HideUI();
    }
}