using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof (UIAnimation))]
public class UIAnimationEditor : Editor
{
    private int actualIndex = 0;
    private float newKeyTime = 0f;
    private float simulate = 0f;
    private float simulateLastFrame = 0f;

    public override void OnInspectorGUI()
    {
        UIAnimation t = (UIAnimation)target;

        EditorGUILayout.LabelField((actualIndex + 1).ToString() + " / " + (t.curves.Length).ToString());
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("<"))
        {
            if (actualIndex > 0)
                actualIndex--;
        }
        if (GUILayout.Button(">"))
        {
            if (actualIndex < t.curves.Length - 1)
                actualIndex++;
        }
        if (GUILayout.Button("+"))
        {
            System.Array.Resize(ref t.curves, t.curves.Length + 1);
            t.curves[t.curves.Length - 1] = new UIAnimation.Curves();
        }
        if (GUILayout.Button("-"))
        {
            if (t.curves.Length > 1)
                System.Array.Resize(ref t.curves, t.curves.Length - 1);
            if (actualIndex > t.curves.Length - 1)
                actualIndex = t.curves.Length - 1;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 50f;
        t.curves[actualIndex].name = EditorGUILayout.TextField("Name: ", t.curves[actualIndex].name);
        EditorGUIUtility.labelWidth = 40f;
        t.curves[actualIndex].loop = EditorGUILayout.Toggle("Loop: ", t.curves[actualIndex].loop, GUILayout.MaxWidth (60f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 100f;
        newKeyTime = EditorGUILayout.FloatField("New Key Time: ", newKeyTime, GUILayout.MaxWidth (150f));
        if (GUILayout.Button("Set Key", GUILayout.MaxWidth (100f)))
        {
            t.SetKey(newKeyTime, actualIndex, Handles.GetMainGameViewSize());
        }
        EditorGUILayout.EndHorizontal();

        if (t.curves[actualIndex].rot.length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Simulate", GUILayout.MaxWidth(75f));
            simulate = GUILayout.HorizontalSlider(
                simulate,
                t.curves[actualIndex].rot.keys[0].time,
                t.curves[actualIndex].rot.keys[t.curves[actualIndex].rot.length - 1].time);
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10f));
            EditorGUILayout.EndHorizontal();

            if (simulate != simulateLastFrame)
            {
                t.UpdateAnimationState(actualIndex, simulate);
                simulateLastFrame = simulate;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 20f;
        EditorGUILayout.CurveField("X: ", t.curves[actualIndex].x);
        EditorGUILayout.CurveField("Y: ", t.curves[actualIndex].y);
        EditorGUIUtility.labelWidth = 30f;
        EditorGUILayout.CurveField("Rot: ", t.curves[actualIndex].rot);
        EditorGUILayout.EndHorizontal();

        int deleteKey = -1;
        EditorGUIUtility.labelWidth = Screen.width - 125f;
        for (int i = 0; i < t.curves[actualIndex].rot.length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "T: " + t.curves[actualIndex].rot.keys[i].time + 
                ", X: " + t.curves[actualIndex].x.Evaluate(t.curves[actualIndex].x.keys[i].time).ToString("000") +
                ", Y: " + t.curves[actualIndex].y.Evaluate(t.curves[actualIndex].y.keys[i].time).ToString("000") +
                ", ROT: " + t.curves[actualIndex].rot.Evaluate(t.curves[actualIndex].rot.keys[i].time).ToString("000"));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.MaxWidth(20f)))
            {
                deleteKey = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (deleteKey != -1)
        {
            t.curves[actualIndex].x.RemoveKey(deleteKey);
            t.curves[actualIndex].y.RemoveKey(deleteKey);
            t.curves[actualIndex].rot.RemoveKey(deleteKey);
        }

        Undo.RecordObject(t, t.name + " Animation");
        EditorUtility.SetDirty(t);
    }
}
