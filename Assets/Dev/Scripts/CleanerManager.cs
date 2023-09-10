using UnityEngine;

public class CleanerManager : MonoBehaviour
{
    [SerializeField] private int widthFactor;
    [SerializeField] private int heightFactor;

    private Texture2D _newBrush;
    private Vector2Int _lastPaintPixelPosition;
    private Ray _ray;

    private void Awake()
    {
        _newBrush = new Texture2D(20, 20);
    }

    private void Update()
    {
        SpaceKey();
        _ray = new Ray(transform.position, Vector3.down);
        if (!Physics.Raycast(_ray, out var raycastHit)) return;
        if (!raycastHit.collider.TryGetComponent(out CleanableObj cleanableObj)) return;
        
        var textureCoord = raycastHit.textureCoord;

        var pixelX = (int)(textureCoord.x * cleanableObj.DirtMaskTexture.width);
        var pixelY = (int)(textureCoord.y * cleanableObj.DirtMaskTexture.height);

        var paintPixelPosition = new Vector2Int(pixelX, pixelY);

        var paintPixelDistance = Mathf.Abs(paintPixelPosition.x - _lastPaintPixelPosition.x) +
                                 Mathf.Abs(paintPixelPosition.y - _lastPaintPixelPosition.y);

        const int maxPaintDistance = 7;
        if (paintPixelDistance < maxPaintDistance) return;


        _lastPaintPixelPosition = paintPixelPosition;

        var pixelXOffset = pixelX - (_newBrush.width / 2);
        var pixelYOffset = pixelY - (_newBrush.height / 2);

        for (var x = 0; x < _newBrush.width; x++)
        {
            for (var y = 0; y < _newBrush.height; y++)
            {
                // var pixelDirt = _newBrush.GetPixel(x, y);
                var pixelDirtMask = cleanableObj.DirtMaskTexture.GetPixel(pixelXOffset + x, pixelYOffset + y);

                // var removedAmount = pixelDirtMask.g * (1 - pixelDirt.g);
                var removedAmount = pixelDirtMask.g;
                cleanableObj.DirtAmount -= removedAmount;

                cleanableObj.DirtMaskTexture.SetPixel(
                    pixelXOffset + x,
                    pixelYOffset + y,
                    // new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                    Color.black
                );
            }
        }
        cleanableObj.DirtMaskTexture.Apply();
        
        if (Mathf.RoundToInt(cleanableObj.DirtAmount / cleanableObj.DirtAmountTotal * 100f) <= 41)
        {
            raycastHit.collider.gameObject.SetActive(false);
        }
    }

    private void SpaceKey()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        print($"Width {_newBrush.width}, Height {_newBrush.height}");
        _newBrush.Reinitialize(_newBrush.width + widthFactor, _newBrush.height + heightFactor);
        print($"Width {_newBrush.width}, Height {_newBrush.height}");
    }
}