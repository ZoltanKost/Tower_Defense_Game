using UnityEngine;
using System.Collections.Generic;
public class ShopUI : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private List<ShopTab> tabs;
    [SerializeField] private List<Transform> pages;
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