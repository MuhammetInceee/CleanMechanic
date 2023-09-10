using UnityEngine;

public class CleanableObj : MonoBehaviour
{
    [SerializeField] private Texture2D dirtMaskTextureBase;
    private Material _material;

    internal Texture2D DirtMaskTexture;
    
    internal float DirtAmountTotal;
    internal float DirtAmount;
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        
        DirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        DirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        DirtMaskTexture.Apply();
        _material.SetTexture("_DirtMask", DirtMaskTexture);

        DirtAmountTotal = 0f;
        for (var x = 0; x < dirtMaskTextureBase.width; x++)
        {
            for (var y = 0; y < dirtMaskTextureBase.height; y++)
            {
                DirtAmountTotal += dirtMaskTextureBase.GetPixel(x, y).g;
            }
        }
        DirtAmount = DirtAmountTotal;
    }
}