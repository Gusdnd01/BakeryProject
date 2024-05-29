using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(menuName = "SO/BreadRecipeTable")]
#endif

public class BreadRecipeTable : LoadableData
{
    public List<Data> DataList => generateData;

    public string[] GetIngredientNamesByCakeName(string cakeName)
    {
        foreach (var data in generateData)
        {
            if (data.str[0] == cakeName)
            {
                string[] ingredientNames = new string[3]
                {
                    data.str[1],
                    data.str[2],
                    data.str[3]
                };

                return ingredientNames;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BreadRecipeTable))]
    public class EpisodeLoader : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BreadRecipeTable episodeData = (BreadRecipeTable)target;
            LoadableData ld = episodeData;

            if (GUILayout.Button("DataGenerate"))
            {
                Debug.Log("DataGenerate Start . . .");
                ld.Generate();
            }
        }
    }
#endif
}
