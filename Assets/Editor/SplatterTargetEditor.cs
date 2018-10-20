using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplatterTarget))]
public class SplatterTargetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var inspectedItem = target as SplatterTarget;

        inspectedItem.color = EditorGUILayout.ColorField("Color", inspectedItem.color);
        inspectedItem.detail = EditorGUILayout.IntSlider("Detail Level", inspectedItem.detail, 2, 12);
        EditorGUILayout.LabelField("Splatter Resolution", string.Format("{0}x{0}", inspectedItem.pow2Size));
        inspectedItem.size = EditorGUILayout.FloatField("Object Size", inspectedItem.size);
    }
}
