using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class ShopUI : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private TMP_Text text;
    [SerializeField] private List<ShopTab> tabs;
    [SerializeField] private List<Transform> pages;
    [SerializeField] private List<string> names;
    Transform currentPage;
    HideShowUI hideShowUI;
    void Awake(){
        hideShowUI = GetComponent<HideShowUI>();
    }
    public void Init(){
        InitTabs();
        HideUI();
    }
    public void ShowUI(){
        hideShowUI.ShowUI();
    }
    public void HideUI(){
        hideShowUI.HideUI();
    }
    public void OpenPage(int pageID){
        text.text = names[pageID];
        currentPage?.gameObject.SetActive(false);
        currentPage = pages[pageID];
        pages[pageID].gameObject.SetActive(true);
    }
    public void InitTabs(){
        for(int i = 0; i < tabs.Count; i++){
            tabs[i].Init(OpenPage,i);
        }
    }
}