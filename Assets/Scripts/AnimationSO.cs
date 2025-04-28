/*using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[CreateAssetMenu(fileName = "AnimationSO",menuName ="Animation/AnimationSO")]
public class AnimationSO : ScriptableObject
{
    [SerializeField]
    public Animation[] animations;
#if UNITY_EDITOR  
    public void Save()
    {
        AssetDatabase.SaveAssetIfDirty(this);
    }
    [OnOpenAsset]
    public static bool Open(int a, int b, int c)
    {
        string path = AssetDatabase.GetAssetPath(a);
        path = path.Remove(0, 17);
        path = path.Remove(path.Length - 6, 6);
        Debug.Log(path);
        AnimationSO asset = Resources.Load(path) as AnimationSO;

        //CustomAnimationEditor editor = EditorWindow.CreateInstance<CustomAnimationEditor>();
        Debug.Log(asset);
        editor.Show();
        editor.OpenAnimator(asset);
        return true;
    }
#endif
}
*/