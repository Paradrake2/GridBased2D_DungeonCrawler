using UnityEngine;

/// <summary>
/// Singleton that spawns floating damage numbers and spell indicators in world space.
/// Attach this to a persistent GameObject in your scene and assign a DamageNumber prefab.
/// 
/// Prefab setup:
///   1. Create a new empty GameObject, add a TextMeshPro (world space) component and a DamageNumber script.
///   2. Save it as a prefab and assign it to the 'damageNumberPrefab' field on this component.
/// </summary>
public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    [SerializeField] private GameObject damageNumberPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Spawns a floating damage number at the given world position.
    /// </summary>
    public void SpawnDamageNumber(Vector3 worldPos, float damage, DamageNumberType type)
    {
        if (damageNumberPrefab == null)
        {
            Debug.LogWarning("DamageNumberSpawner: damageNumberPrefab is not assigned.");
            return;
        }

        Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), 0.3f, 0f);
        GameObject go = Instantiate(damageNumberPrefab, worldPos + offset, Quaternion.identity);
        DamageNumber dn = go.GetComponent<DamageNumber>();
        if (dn != null)
            dn.Setup(damage, type);
    }

    /// <summary>
    /// Spawns a brief floating label (e.g. spell element name) above the damage number position.
    /// </summary>
    public void SpawnSpellIndicator(Vector3 worldPos, string label, Color color)
    {
        if (damageNumberPrefab == null) return;

        Vector3 offset = new Vector3(0f, 0.8f, 0f);
        GameObject go = Instantiate(damageNumberPrefab, worldPos + offset, Quaternion.identity);
        DamageNumber dn = go.GetComponent<DamageNumber>();
        if (dn != null)
            dn.SetupSpellIndicator(label, color);
    }
}
