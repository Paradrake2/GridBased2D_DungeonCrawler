using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DamageNumber : MonoBehaviour
{
    private TextMeshPro tmp;
    private float floatSpeed = 1.2f;
    private float lifetime = 1.4f;
    private float fadeDelay = 0.7f;
    private float elapsed;
    private Color baseColor;

    public void Setup(float damage, DamageNumberType type)
    {
        tmp = GetComponent<TextMeshPro>();
        int rounded = Mathf.Max(1, Mathf.RoundToInt(damage));

        switch (type)
        {
            case DamageNumberType.Normal:
                tmp.text = rounded.ToString();
                tmp.color = Color.white;
                tmp.fontSize = 3f;
                break;

            case DamageNumberType.Spell:
                tmp.text = rounded.ToString();
                tmp.color = new Color(0.75f, 0.25f, 1f); // purple
                tmp.fontSize = 3.5f;
                floatSpeed = 1.4f;
                break;

            case DamageNumberType.Weakness:
                tmp.text = rounded.ToString() + "!";
                tmp.color = new Color(1f, 0.55f, 0f); // orange
                tmp.fontSize = 4f;
                floatSpeed = 1.5f;
                break;

            case DamageNumberType.SpellWeakness:
                tmp.text = rounded.ToString() + "!!";
                tmp.color = new Color(1f, 0.85f, 0f); // gold
                tmp.fontSize = 4.5f;
                floatSpeed = 1.6f;
                break;
        }

        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 100;
        baseColor = tmp.color;
    }

    public void SetupSpellIndicator(string label, Color color)
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.text = label;
        tmp.color = color;
        tmp.fontSize = 2.5f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 100;
        baseColor = color;
        floatSpeed = 0.8f;
        lifetime = 1.0f;
        fadeDelay = 0.4f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (elapsed >= fadeDelay)
        {
            float t = (elapsed - fadeDelay) / Mathf.Max(0.001f, lifetime - fadeDelay);
            Color c = baseColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            tmp.color = c;
        }

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}
