#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class DataflowTestWindow : EditorWindow
{
    private DFProgramAsset program;
    private StatType damageStatOverride;
    private Player caster;
    private Enemy target;
    private bool verbose;

    private DFEvaluationResult lastResult;
    private Vector2 scroll;

    [MenuItem("Tools/Dataflow/Test Window")]
    public static void Open()
    {
        GetWindow<DataflowTestWindow>("Dataflow Test");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Dataflow Evaluator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Use a DF Program asset to run dataflow without the in-game crafter. " +
            "Create one via Assets/Create/Spells/Dataflow/Program.",
            MessageType.Info);

        program = (DFProgramAsset)EditorGUILayout.ObjectField("Program", program, typeof(DFProgramAsset), false);
        damageStatOverride = (StatType)EditorGUILayout.ObjectField("Damage Stat (optional)", damageStatOverride, typeof(StatType), false);
        caster = (Player)EditorGUILayout.ObjectField("Caster (optional)", caster, typeof(Player), true);
        target = (Enemy)EditorGUILayout.ObjectField("Target (optional)", target, typeof(Enemy), true);
        verbose = EditorGUILayout.Toggle("Verbose Logs", verbose);

        using (new EditorGUI.DisabledScope(program == null))
        {
            if (GUILayout.Button("Evaluate"))
                Evaluate();
        }

        EditorGUILayout.Space(10);
        DrawResults();
    }

    private void Evaluate()
    {
        lastResult = null;

        if (program == null)
            return;

        SpellComposition runtime = DFCompositionUtils.ClonePlacedGrid(program.composition);

        var context = new DFContext(caster, target)
        {
            damageStatType = damageStatOverride != null
                ? damageStatOverride
                : (StatDatabase.Instance != null ? StatDatabase.Instance.GetStat("Damage") : null),
            verbose = verbose
        };

        lastResult = DFEvaluator.Evaluate(runtime, context);

        if (lastResult != null)
            Debug.Log("[DataflowTest] flatDamage=" + lastResult.flatDamage + ", stats=" + (lastResult.spellStats?.Stats?.Count ?? 0));
    }

    private void DrawResults()
    {
        if (lastResult == null)
        {
            EditorGUILayout.LabelField("No result yet.");
            return;
        }

        EditorGUILayout.LabelField("Result", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("flatDamage", lastResult.flatDamage.ToString());

        var sb = new StringBuilder();
        if (lastResult.spellStats != null && lastResult.spellStats.Stats != null)
        {
            foreach (var stat in lastResult.spellStats.Stats)
            {
                if (stat == null || stat.StatType == null) continue;
                sb.Append(stat.StatType.displayName);
                sb.Append(" (" + stat.StatType.StatID + ")");
                sb.Append(": ");
                sb.AppendLine(stat.Value.ToString());
            }
        }

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(160));
        EditorGUILayout.TextArea(sb.Length == 0 ? "(none)" : sb.ToString());
        EditorGUILayout.EndScrollView();
    }
}
#endif
