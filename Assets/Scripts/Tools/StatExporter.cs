using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
public static class StatExporter
{
#if UNITY_EDITOR
    [Serializable]
    private class StatExportFile
    {
        public string exportedAt = DateTime.Now.ToString();
        public List<StatExportEntry> stats = new List<StatExportEntry>();
    }
    [Serializable]
    private class StatExportEntry
    {
        public string statId;
        public string displayName;
        public StatCategory category;
        public Color color;
        public bool isPercentage;
    }
    [MenuItem("Tools/Export Stat Types to JSON")]
    public static void ExportStatTypesToJson()
    {
        string[] guids = AssetDatabase.FindAssets("t:StatType");
        var export = new StatExportFile
        {
            exportedAt = DateTime.Now.ToString()
        };

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            StatType statType = AssetDatabase.LoadAssetAtPath<StatType>(assetPath);
            if (statType == null) continue;

            var entry = new StatExportEntry
            {
                statId = statType.StatID,
                displayName = statType.displayName,
                category = statType.category,
                color = statType.displayColor,
                isPercentage = statType.isPercentage
            };

            export.stats.Add(entry);
        }

        string json = JsonUtility.ToJson(export, true);
        string outputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..","Exports","ExportedStatTypes.json"));
        File.WriteAllText(outputPath, json);
        Debug.Log($"Exported {export.stats.Count} Stat Types to {outputPath}");
    }

#endif
}
