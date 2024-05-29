using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BuffSO))]
public class BuffSOEditor : Editor
{
    private BuffSO ownerSO;

    private ReorderableList normalBuffList;
    private ReorderableList specialBuffList;
    private ReorderableList stackBuffList;
    private void OnEnable()
    {
        ownerSO = (BuffSO)target;
        {
            normalBuffList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("statBuffs"),
                    true, true, true, true);
            normalBuffList.elementHeight *= 2;
            normalBuffList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = normalBuffList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fieldRect,
                    element.FindPropertyRelative("type"), GUIContent.none);
                fieldRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.33f, fieldRect.height);
                SerializedProperty sp = element.FindPropertyRelative("values").Copy();
                if (sp.arraySize <= 0)
                {
                    for (int i = 0; i < 3; i++)
                        sp.InsertArrayElementAtIndex(i);
                }
                for (int i = 0; i < 3; i++)
                {
                    EditorGUI.PropertyField(fieldRect,
                        sp.GetArrayElementAtIndex(i), GUIContent.none);
                    fieldRect.x += fieldRect.width + 2;
                }
            };
            normalBuffList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Stat Buff");
            };
        }
        {
            stackBuffList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("stackBuffs"),
                true, true, true, true);
            stackBuffList.elementHeight *= 2;
            stackBuffList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = stackBuffList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fieldRect,
                    element.FindPropertyRelative("type"), GUIContent.none);
                fieldRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.33f, fieldRect.height);
                SerializedProperty sp = element.FindPropertyRelative("values").Copy();
                if (sp.arraySize <= 0)
                {
                    for (int i = 0; i < 3; i++)
                        sp.InsertArrayElementAtIndex(i);
                }
                for (int i = 0; i < 3; i++)
                {
                    EditorGUI.PropertyField(fieldRect,
                        sp.GetArrayElementAtIndex(i), GUIContent.none);
                    fieldRect.x += fieldRect.width + 2;
                }
            };
            stackBuffList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Stat Buff");
            };
        }
        specialBuffList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("specialBuffs"),
                true, true, true, true);
        specialBuffList.multiSelect = true;
        specialBuffList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = specialBuffList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - rect.x, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none, true);
            ////EditorGUI.PropertyField(
            //    new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
            //    element.FindPropertyRelative("value"), GUIContent.none);
        };
        specialBuffList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            var types = TypeCache.GetTypesDerivedFrom<SpecialBuff>();
            foreach (var t in types)
            {
                menu.AddItem(new GUIContent("SpecialBuff/" + t.Name),
                false, HandlerSpecialBuffAdd, t);
            }
            menu.ShowAsContext();
        };
        specialBuffList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.selectedIndices.Count > 0)
            {
                int minus = 0;
                foreach (var i in l.selectedIndices)
                {
                    Undo.DestroyObjectImmediate(ownerSO.specialBuffs[i - minus]);
                    AssetDatabase.SaveAssets();
                    ownerSO.specialBuffs.RemoveAt(i - minus);
                    minus++;

                    EditorUtility.SetDirty(ownerSO);
                }
            }
            else
            {
                Undo.DestroyObjectImmediate(ownerSO.specialBuffs[ownerSO.specialBuffs.Count - 1]);
                ownerSO.specialBuffs.RemoveAt(ownerSO.specialBuffs.Count - 1);
                AssetDatabase.SaveAssets();
            }
        };
        specialBuffList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Special Buff");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        normalBuffList.DoLayoutList();
        specialBuffList.DoLayoutList();
        stackBuffList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
    private void HandlerSpecialBuffAdd(object target)
    {
        Type type = (Type)target;
        Debug.Log(type.Name);
        var index = specialBuffList.serializedProperty.arraySize;
        specialBuffList.serializedProperty.arraySize++;
        specialBuffList.index = index;
        var element = specialBuffList.serializedProperty.GetArrayElementAtIndex(index);
        SpecialBuff buff = ScriptableObject.CreateInstance(type) as SpecialBuff;
        buff.name = type.Name;

        AssetDatabase.AddObjectToAsset(buff, ownerSO);
        AssetDatabase.SaveAssets();

        element.objectReferenceValue = buff;
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(ownerSO);
        EditorUtility.SetDirty(buff);
    }
}
