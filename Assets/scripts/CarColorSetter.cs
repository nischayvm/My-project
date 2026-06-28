using UnityEngine;

[ExecuteAlways]
public class CarColorSetter : MonoBehaviour
{
    [Header("Car Color Settings")]
    [Tooltip("Choose the color for this specific car.")]
    public Color carColor = Color.white;

    [Tooltip("The Renderer of the car's body. If empty, it will try to find one on this object.")]
    public Renderer carRenderer;

    [Tooltip("Check this to remove the underlying texture so your color doesn't mix with it.")]
    public bool removeBaseTexture = true;

    private MaterialPropertyBlock propBlock;

    void Update()
    {
        if (carRenderer == null)
        {
            carRenderer = GetComponentInChildren<Renderer>();
        }

        if (carRenderer != null)
        {
            if (propBlock == null)
            {
                propBlock = new MaterialPropertyBlock();
            }

            // Get the current properties
            carRenderer.GetPropertyBlock(propBlock);

            // Set both standard and URP color properties
            propBlock.SetColor("_Color", carColor);
            propBlock.SetColor("_BaseColor", carColor);

            if (removeBaseTexture)
            {
                // In URP, the main texture is usually "_BaseMap". Standard is "_MainTex".
                propBlock.SetTexture("_BaseMap", Texture2D.whiteTexture);
                propBlock.SetTexture("_MainTex", Texture2D.whiteTexture);
            }

            // Apply the properties only to this specific renderer (no material sharing issues!)
            carRenderer.SetPropertyBlock(propBlock);
        }
    }
}
