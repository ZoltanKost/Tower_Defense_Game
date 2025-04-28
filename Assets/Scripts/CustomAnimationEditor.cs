/*using UnityEditor;
using UnityEngine;
using System;


public class CustomAnimationEditor : EditorWindow
{
    static Animation[] animations;
    static string[] strings;
    static void InitAnimations()
    {
        animations = new Animation[1];
        animations[0].data = new AnimationData[1];
        animations[0].data[0].sprites = new Sprite[1][];
        animations[0].data[0].sprites[0] = new Sprite[1];
        animations[0].data[0].frames = new KeyFrame[1][];
        animations[0].data[0].frames[0] = new KeyFrame[1];

        Debug.Log(animations.Length);
        int l = animations == null ? 1 : animations.Length;
        strings = new string[l];
        for (int i = 0; i < l; i++)
        {
            strings[i] = i.ToString();
        }
        Debug.Log(l);
    }

    string t = "Fuck guys";
    [SerializeField] AnimationSO animator;
    Vector2 keyFrame_view;
    int currentClip = 0;
    int currentDirection = 0;
    public void OnEnable()
    {
        InitAnimations();
    }
    public void OnGUI()
    {
        
        float window_width = position.width;
        GUILayoutOption target_width = GUILayout.Width(window_width / 6);
        GUILayoutOption frame_width = GUILayout.Width(window_width * 2 / 3);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        //Debug.Log(strings);
        currentClip = EditorGUILayout.Popup(currentClip, strings, target_width, GUILayout.Height(30));
        currentDirection = EditorGUILayout.Popup(currentDirection, strings,frame_width, GUILayout.Height(30));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Vector2 pos = Vector2.zero;
        pos.y = keyFrame_view.y;
        EditorGUILayout.BeginScrollView(pos, target_width);
        AnimationData currentData = animations[currentClip].data[currentDirection];
        for (int i =0; i < currentData.sprites.Length; i++)
        {
            if (GUILayout.Button(t, target_width, GUILayout.Height(30))) 
            {
                Array.Resize(ref currentData.sprites[i], currentData.sprites[i].Length + 1);
                Debug.Log(currentData.sprites[i].Length);
            }
        }
        for (int i = 0; i < currentData.frames.Length; i++)
        {
            
            if (GUILayout.Button(t, target_width, GUILayout.Height(30)))
            {
                Array.Resize(ref currentData.frames[i], currentData.frames[i].Length + 1);
                Debug.Log(currentData.frames.Length);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
        keyFrame_view = EditorGUILayout.BeginScrollView(keyFrame_view, frame_width);
        EditorGUILayout.BeginVertical();
        float button_width = 30;
        var frameStyle = GUILayout.Width(button_width);
        for (int i = 0; i < currentData.sprites.Length; i++)
        {
            
            //("", GUILayout.Width(400), GUILayout.Height(30));
            //GUI.backgroundColor = i % 2 == 0? Color.gray : Color.blue;
            //GUI.backgroundColor = Color.gray;z
            EditorGUILayout.BeginHorizontal();
            for (int l = 0; l < currentData.sprites[i].Length; l++)
            {
                if (GUILayout.Button("", frameStyle, GUILayout.Height(32)))
                {
                    
                    Debug.Log("Keyframe Added!");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < currentData.frames.Length; i++)
        {

            //("", GUILayout.Width(400), GUILayout.Height(30));
            //GUI.backgroundColor = i % 2 == 0? Color.gray : Color.blue;
            //GUI.backgroundColor = Color.gray;z

            

            EditorGUILayout.BeginHorizontal();
            for (int l = 0; l < currentData.frames[i].Length; l++)
            {
                var rect = new Rect(0, 0, 75, 50);
                //GUILayout.BeginArea(rect);
                Bounds value = new Bounds(currentData.frames[i][l].position,
                    currentData.frames[i][l].position);
                value = EditorGUILayout.BoundsField(value, GUILayout.MinWidth(200), GUILayout.Height(50)) ;
                currentData.frames[i][l].position = value.center;
                currentData.frames[i][l].rotation = value.extents;
                *//*                if (GUILayout.Button("", frameStyle, GUILayout.Height(32)))
                                {
                *//*
                //Debug.Log("Keyframe Added!");
                //}
                //GUILayout.EndArea();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add Boxed", GUILayout.Width(100), GUILayout.Height(100)))
        {
            if (animations != null)
            {
                Array.Resize(ref animations, animations.Length + 1);
                animations[^1] = new Animation();
                Array.Resize(ref strings, strings.Length + 1);
                strings[^1] = (strings.Length - 1).ToString();
            }
            else animations = new Animation[1];
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
    public void OpenAnimator(AnimationSO _animator) 
    {
        if (animator != null) { 
            animator.animations = animations;
            animator.Save();
        }
        animator = _animator;
        animations = _animator.animations;
        if(animations == null || animations.Length == 0)
        {
            Debug.Log("Animations are empty");
            InitAnimations();
        }
        
    }
    public void OnDisable()
    {
        animator.animations = animations;
        animator.Save();
    }
}
*/