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
    }
    [Serializable]
    private class ExportedStat
    {
        public string statId;
        public string displayName;
        public StatCategory category;
        public float value;
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
        if (playerStats == null)
        {
            Debug.LogError("player.GetStats() returned null (stats not initialized yet in play mode?).");
            return;
        }

        if (playerStats.Stats == null)
        {
            Debug.LogError("playerStats.Stats is null (stats list not initialized).");
            return;
        }

        int i = 0;
        foreach (var statEntry in playerStats.Stats)
        {
            if (statEntry == null)
            {
                Debug.LogError($"playerStats.Stats contains a null entry at index {i}.");
                i++;
                continue;
            }

            if (statEntry.StatType == null)
            {
                Debug.LogError($"Stat entry at index {i} has null StatType.");
                i++;
                continue;
            }

            var exportedStat = new ExportedStat
            {
                statId = statEntry.StatType.StatID,
                displayName = statEntry.StatType.displayName,
                category = statEntry.StatType.category,
                value = statEntry.Value
            };

            export.playerStats.Add(exportedStat);
            i++;
        }

        PlayerAttributeSet attributeSet = player.GetAttributeSet();
        foreach (var attribute in attributeSet.GetAttackAttributes())
        {
            var exportedAttribute = new ExportedStat
            {
                statId = attribute.attackAttribute.StatID,
                displayName = attribute.attackAttribute.displayName,
                category = attribute.attackAttribute.category,
                value = attribute.attackAttributeValue
            };
            export.playerStats.Add(exportedAttribute);
        }
        foreach (var attribute in attributeSet.GetDefenseAttributes())
        {
            var exportedAttribute = new ExportedStat
            {
                statId = attribute.defenseAttribute.StatID,
                displayName = attribute.defenseAttribute.displayName,
                category = attribute.defenseAttribute.category,
                value = attribute.defenseAttributeValue
            };
            export.playerStats.Add(exportedAttribute);
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
