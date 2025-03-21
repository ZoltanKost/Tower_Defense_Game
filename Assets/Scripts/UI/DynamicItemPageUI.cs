using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DynamicItemPageUI : MonoBehaviour
{
    [SerializeField] private SpellButtonUI buttonPrefab;
    private List<SpellButtonUI> buttons;
    private Action<int> callback;
    int length;
    Resource resource;
    public void Init(Action<int> OnMagicBuyCallBack, Resource res)
    {
        resource = res;
        buttons = new List<SpellButtonUI>();
        callback = OnMagicBuyCallBack;
        length = 0;
    }

    public void UpdateVisual(int ID, Sprite sprite)
    {
        buttons[ID].SetSprite(sprite);
    }
    public void UpdateCooldown(int id, float value)
    {
        buttons[id].UpdateValue(1f-value);
    }
    public void ResetVisuals(Sprite[] sprites)
    {
        if(sprites == null)
        {
            foreach(var b in buttons)
            {
                b.gameObject.SetActive(false);
            }
            length = 0;
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i < sprites.Length) buttons[i].SetSprite(sprites[i]);
                //else buttons[i].DeactivateVisuals();
            }
            length = sprites.Length;
        }
    }
    public void ActivateVisuals(int uiID)
    {
        //buttons[uiID].ActivateVisuals();
    }
    public void DeactivateVisuals(int uiID)
    {
        //buttons[uiID].DeactivateVisuals();
    }
    public void AddItem(SpellData item)
    {
        int id = length++;
        SpellButtonUI ui;
        if (id < buttons.Count)
        {
            ui = buttons[id];
        }
        else
        {
            ui = Instantiate(buttonPrefab, transform);
            buttons.Add(ui);
        }
        int cost = -1;
        switch (resource)
        {
            case Resource.Gold:
                cost = item.goldCost;
                break;
            case Resource.Mana:
                cost = item.manaCost;                
                break;
        }
        ui.Init(callback, item.UIicon,cost, id);
        ui.gameObject.SetActive(true);
    }
    public void RemoveItem(int id)
    {
        var remove = buttons[id];
        if (id != length - 1)
        {
            buttons[id] = buttons[length - 1];
            buttons[id].SetID(id);
            buttons[length - 1] = remove;
            remove.SetID(id);
        }
        remove.gameObject.SetActive(false);
        length--;
    }
}