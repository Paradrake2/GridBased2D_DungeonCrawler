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
        public string statId;
        public string displayName;
        public StatCategory category;
        public float value;
    }

    [Serializable]
    private class ExportedAttribute
    {
        public string damageAttributeName;
        public float damageAttributeValue;
        public string defenseAttributeName;
        public float defenseAttributeValue;
    }

    [Serializable]
    private class EnemyStatsExportEntry
    {
        public string name;
        public string assetPath;
        public string resourcesPath;
        public List<ExportedStat> stats = new List<ExportedStat>();
        public List<ExportedAttribute> enemyAttributesList = new List<ExportedAttribute>();
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
                dropTable = enemyStatsHolder.dropTable,
                goldDropped = enemyStatsHolder.goldDropped,
                experienceDropped = enemyStatsHolder.experienceDropped
            };

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

            if (enemyStatsHolder.enemyAttributesList != null)
            {
                foreach (var attribute in enemyStatsHolder.enemyAttributesList)
                {
                    entry.stats.Add(new ExportedStat
                    {
                        statId = attribute.attackAttribute != null ? attribute.attackAttribute.StatID : "",
                        displayName = attribute.attackAttribute != null && !string.IsNullOrWhiteSpace(attribute.attackAttribute.displayName)
                            ? attribute.attackAttribute.displayName
                            : (attribute.attackAttribute != null ? attribute.attackAttribute.name : "Unknown Stat"),
                        category = attribute.attackAttribute != null ? attribute.attackAttribute.category : default,
                        value = attribute.attackAttributeValue
                    });
                    entry.stats.Add(new ExportedStat
                    {
                        statId = attribute.defenseAttribute != null ? attribute.defenseAttribute.StatID : "",
                        displayName = attribute.defenseAttribute != null && !string.IsNullOrWhiteSpace(attribute.defenseAttribute.displayName)
                            ? attribute.defenseAttribute.displayName
                            : (attribute.defenseAttribute != null ? attribute.defenseAttribute.name : "Unknown Stat"),
                        category = attribute.defenseAttribute != null ? attribute.defenseAttribute.category : default,
                        value = attribute.defenseAttributeValue
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
