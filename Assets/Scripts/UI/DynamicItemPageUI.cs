using System;
using UnityEngine;
using System.Collections.Generic;

public class DynamicItemPageUI : MonoBehaviour
{
    [SerializeField] private BuildingButtonUI buttonPrefab;
    private List<BuildingButtonUI> buttons;
    private Action<int> callback;
    int length;
    public void Init(Action<int> OnMagicBuyCallBack)
    {
        buttons = new List<BuildingButtonUI>();
        callback = OnMagicBuyCallBack;
        length = 0;
    }

    public void UpdateVisual(int ID, Sprite sprite)
    {
        buttons[ID].SetSprite(sprite);
    }
    public void ResetVisuals(Sprite[] sprites)
    {
        if(sprites == null)
        {
            foreach(var b in buttons)
            {
                b.DeactivateVisuals();
            }
            length = 0;
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i < sprites.Length) buttons[i].SetSprite(sprites[i]);
                else buttons[i].DeactivateVisuals();
            }
            length = sprites.Length;
        }
    }
    public void ActivateVisuals(int uiID)
    {
        buttons[uiID].ActivateVisuals();
    }
    public void DeactivateVisuals(int uiID)
    {
        buttons[uiID].DeactivateVisuals();
    }
    public void AddItem(SpellData item)
    {
        int id = length++;
        BuildingButtonUI ui;
        if (id < buttons.Count)
        {
            ui = buttons[id];
        }
        else
        {
            ui = Instantiate(buttonPrefab, transform);
            buttons.Add(ui);
        }
        ui.Init(callback, item.UIicon, id);
    }
    public void RemoveItem(int id)
    {
        buttons[id] = buttons[--length];
        buttons[id].SetID(id);
        buttons[length].gameObject.SetActive(false);
    }
}