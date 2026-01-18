using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EnemyStatsExporter
{
#if UNITY_EDITOR

    [Serializable]
    private class EnemyStatsExportFile
    {
        public string exportedAt = DateTime.Now.ToString();
        public List<EnemyStatsExportEntry> enemyStats = new List<EnemyStatsExportEntry>();
    }

    [Serializable]
    private class ExportedStat
    {
        public string statId;        // StatType.name (your StatID)
        public string displayName;   // StatType.displayName
        public StatCategory category;
        public float value;
    }

    [Serializable]
    private class EnemyStatsExportEntry
    {
        public string name;
        public string assetPath;
        public string resourcesPath;

        // Replace StatCollection with an export-friendly list
        public List<ExportedStat> stats = new List<ExportedStat>();

        public List<EnemyAttributes> enemyAttributesList;
        public LootTable dropTable;
        public float goldDropped;
        public float experienceDropped;
    }

    [MenuItem("Tools/Export Enemy Stats to JSON")]
    public static void ExportEnemyStatsToJson()
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemyStatsHolder");
        var export = new EnemyStatsExportFile
        {
            exportedAt = DateTime.Now.ToString()
        };

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EnemyStatsHolder enemyStatsHolder = AssetDatabase.LoadAssetAtPath<EnemyStatsHolder>(assetPath);
            if (enemyStatsHolder == null) continue;

            var entry = new EnemyStatsExportEntry
            {
                name = enemyStatsHolder.name,
                assetPath = assetPath,
                resourcesPath = AssetDatabase.GetAssetPath(enemyStatsHolder),
                enemyAttributesList = enemyStatsHolder.enemyAttributesList,
                dropTable = enemyStatsHolder.dropTable,
                goldDropped = enemyStatsHolder.goldDropped,
                experienceDropped = enemyStatsHolder.experienceDropped
            };

            // Flatten StatType reference -> strings
            if (enemyStatsHolder.stats != null)
            {
                foreach (var statValue in enemyStatsHolder.stats.Stats)
                {
                    var statType = statValue.StatType;

                    entry.stats.Add(new ExportedStat
                    {
                        statId = statType != null ? statType.StatID : "",
                        displayName = statType != null && !string.IsNullOrWhiteSpace(statType.displayName)
                            ? statType.displayName
                            : (statType != null ? statType.name : "Unknown Stat"),
                        category = statType != null ? statType.category : default,
                        value = statValue.Value
                    });
                }
            }

            export.enemyStats.Add(entry);
        }

        string defaultName = $"EnemyStatsExport_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string exportDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Exports"));
        Directory.CreateDirectory(exportDir);

        string path = EditorUtility.SaveFilePanel("Save Enemy Stats JSON", exportDir, defaultName, "json");
        if (string.IsNullOrWhiteSpace(path)) return;

        string json = JsonUtility.ToJson(export, true);
        File.WriteAllText(path, json);
        Debug.Log($"Exported {export.enemyStats.Count} EnemyStatsHolder assets to {path}");
        EditorUtility.RevealInFinder(path);
    }

#endif
}
