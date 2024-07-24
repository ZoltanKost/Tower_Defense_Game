using System;
using UnityEngine;

public class PlayerShopUIManager : MonoBehaviour {
    [SerializeField] private EventSubscribeButton shopButton;
    [SerializeField] private EventSubscribeButton startButton;
    private HideShowUI shopButtonHideShow;
    private HideShowUI startButtonHideShow;
    [SerializeField] private ShopUI shopUI;
    bool shopOpen = false;
    public void Init(Action startLevelCallback)
    {
        shopButtonHideShow = shopButton.GetComponent<HideShowUI>();
        startButtonHideShow = startButton.GetComponent<HideShowUI>();
        shopButton.Init(OpenShop);
        startButton.Init(startLevelCallback);
        shopUI.Init();
    }
    public void OpenShop()
    {
        if(shopOpen) return;
        shopButtonHideShow.HideUI();
        startButtonHideShow.HideUI();
        shopUI.ShowUI();
        shopOpen = true;
    }
    public void CloseShop()
    {
        if(!shopOpen) return;
        shopButtonHideShow.ShowUI();
        startButtonHideShow.ShowUI();
        shopUI.HideUI();
        shopOpen = false;
    }
}