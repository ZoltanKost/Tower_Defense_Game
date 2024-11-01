using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class GameLoadManager : MonoBehaviour
{
    [SerializeField] private LoadTemplateUI Load_template;
    [SerializeField] private Transform Parent;
    private List<LoadTemplateUI> buttons;
    private List<LevelData> datas;
    Action<LevelData> loadCallback;
    string[] fileNames;
    public void Init(Action<LevelData> LoadCallback)
    {
        buttons = new List<LoadTemplateUI>();
        loadCallback = LoadCallback;
    }
    public void ReadSaveData()
    {
        datas = new List<LevelData>();
        fileNames = Directory.GetFiles(Application.persistentDataPath + "/saves");
        foreach(var b in buttons)
        {
            b.gameObject.SetActive(false);
        }
        foreach (string file in fileNames)
        {
            string data = File.ReadAllText(file);
            datas.Add(JsonUtility.FromJson<LevelData>(data));
        }
        for (int i = 0; i < datas.Count; i++)
        {
            LevelData data = datas[i];
            if(i >= buttons.Count)
            {
                buttons.Add(Instantiate(Load_template, Parent));
            }
            string s = fileNames[i].Remove(0, Application.persistentDataPath.Length + 7);
            s.Remove(s.Length - 5);
            Debug.Log(s);
            buttons[i].Init(Load, Remove, i, s);
        }
        OpenWindow();
    }
    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }
    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
    public void Load(int i)
    {
        loadCallback?.Invoke(datas[i]);
    }
    public void Remove(int id)
    {
        Debug.Log(id);
        File.Delete(fileNames[id]);
        ReadSaveData();
    }
}