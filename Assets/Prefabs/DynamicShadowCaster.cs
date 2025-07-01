using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer), typeof(ShadowCaster2D))]
public class DynamicShadowCaster : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ShadowCaster2D shadowCaster;
    private Sprite lastSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowCaster = GetComponent<ShadowCaster2D>();
        UpdateShadowShape(spriteRenderer.sprite);
    }

    void LateUpdate()
    {
        if (spriteRenderer.sprite != lastSprite)
        {
            UpdateShadowShape(spriteRenderer.sprite);
            lastSprite = spriteRenderer.sprite;
        }
    }

    private void UpdateShadowShape(Sprite sprite)
    {
        if (sprite == null) return;

        Vector3[] newShape = new Vector3[sprite.vertices.Length];
        for (int i = 0; i < newShape.Length; i++)
            newShape[i] = sprite.vertices[i];

        // Access internal fields of ShadowCaster2D using reflection
        var shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        var meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);

        shapePathField?.SetValue(shadowCaster, newShape);
        meshField?.SetValue(shadowCaster, null); // force rebuild

        MethodInfo generateShadowMesh = typeof(ShadowCaster2D).GetMethod("GenerateShadowMesh", BindingFlags.NonPublic | BindingFlags.Instance);
        generateShadowMesh?.Invoke(shadowCaster, null);
    }
}
