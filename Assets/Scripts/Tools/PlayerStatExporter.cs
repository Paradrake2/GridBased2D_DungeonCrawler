using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
public static class PlayerStatExporter
{
    [Serializable]
    private class PlayerStatsExportFile
    {
        public string exportedAt = DateTime.Now.ToString();
        public List<ExportedStat> playerStats = new List<ExportedStat>();
        public List<ExportedAttribute> playerAttributes = new List<ExportedAttribute>();
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
        public string attributeName;
        public float attributeValue;
    }
    [MenuItem("Tools/Export Player Stats to JSON")]
    public static void ExportPlayerStatsToJson()
    {
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("No Player object found in the scene.");
            return;
        }
        var export = new PlayerStatsExportFile
        {
            exportedAt = DateTime.Now.ToString()
        };
        StatCollection playerStats = player.GetStats();
        foreach (var statEntry in playerStats.Stats)
        {
            var exportedStat = new ExportedStat
            {
                statId = statEntry.StatType.StatID,
                displayName = statEntry.StatType.displayName,
                category = statEntry.StatType.category,
                value = statEntry.Value
            };
            export.playerStats.Add(exportedStat);
        }
        PlayerAttributeSet attributeSet = player.GetAttributeSet();
        foreach (var attribute in attributeSet.GetAttackAttributes())
        {
            var exportedAttribute = new ExportedAttribute
            {
                attributeName = attribute.attackAttribute.displayName,
                attributeValue = attribute.attackAttributeValue
            };
            export.playerAttributes.Add(exportedAttribute);
        }
        foreach (var attribute in attributeSet.GetDefenseAttributes())
        {
            var exportedAttribute = new ExportedAttribute
            {
                attributeName = attribute.defenseAttribute.displayName,
                attributeValue = attribute.defenseAttributeValue
            };
            export.playerAttributes.Add(exportedAttribute);
        }
        string json = JsonUtility.ToJson(export, true);
        System.IO.File.WriteAllText("PlayerStatsExport.json", json);
        Debug.Log("Player stats exported to PlayerStatsExport.json");
        string defaultName = $"PlayerStatsExport.json";
        string exportDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Exports"));
        Directory.CreateDirectory(exportDir);

        string path = EditorUtility.SaveFilePanel("Save Player Stats JSON", exportDir, defaultName, "json");
        if (string.IsNullOrWhiteSpace(path)) return;

        File.WriteAllText(path, json);
        Debug.Log($"Exported {export.playerStats.Count} PlayerStatsHolder assets to {path}");
        EditorUtility.RevealInFinder(path);
    }
}
