using TMPro;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private WorldManager worldManager;
    public void Save()
    {
        worldManager.Save(inputField.text);
        inputField.text = "Saved!";
    }
}
