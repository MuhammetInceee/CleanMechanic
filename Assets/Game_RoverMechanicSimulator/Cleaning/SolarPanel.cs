using CodeMonkey.Utils;
using UnityEngine;
using TMPro;

public class SolarPanel : MonoBehaviour
{
    [SerializeField] private Texture2D dirtMaskTextureBase;
    [SerializeField] private Texture2D dirtBrush;
    [SerializeField] private Material material;
    [SerializeField] private TextMeshProUGUI uiText;

    private Texture2D _dirtMaskTexture;
    private bool _isFlipped;
    private Animation _solarAnimation;
    private float _dirtAmountTotal;
    private float _dirtAmount;
    private Vector2Int _lastPaintPixelPosition;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        _dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        _dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        _dirtMaskTexture.Apply();
        material.SetTexture("_DirtMask", _dirtMaskTexture);

        _solarAnimation = GetComponent<Animation>();

        _dirtAmountTotal = 0f;
        for (var x = 0; x < dirtMaskTextureBase.width; x++)
        {
            for (var y = 0; y < dirtMaskTextureBase.height; y++)
            {
                _dirtAmountTotal += dirtMaskTextureBase.GetPixel(x, y).g;
            }
        }

        _dirtAmount = _dirtAmountTotal;

        FunctionPeriodic.Create(() => { uiText.text = Mathf.RoundToInt(GetDirtAmount() * 100f) + "%"; }, .03f);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var raycastHit))
            {
                var textureCoord = raycastHit.textureCoord;

                var pixelX = (int)(textureCoord.x * _dirtMaskTexture.width);
                var pixelY = (int)(textureCoord.y * _dirtMaskTexture.height);

                var paintPixelPosition = new Vector2Int(pixelX, pixelY);

                var paintPixelDistance = Mathf.Abs(paintPixelPosition.x - _lastPaintPixelPosition.x) +
                                         Mathf.Abs(paintPixelPosition.y - _lastPaintPixelPosition.y);
                const int maxPaintDistance = 7;
                if (paintPixelDistance < maxPaintDistance)
                {
                    // Painting too close to last position
                    return;
                }

                _lastPaintPixelPosition = paintPixelPosition;

                var pixelXOffset = pixelX - (dirtBrush.width / 2);
                var pixelYOffset = pixelY - (dirtBrush.height / 2);

                for (var x = 0; x < dirtBrush.width; x++)
                {
                    for (var y = 0; y < dirtBrush.height; y++)
                    {
                        var pixelDirt = dirtBrush.GetPixel(x, y);
                        var pixelDirtMask = _dirtMaskTexture.GetPixel(pixelXOffset + x, pixelYOffset + y);

                        var removedAmount = pixelDirtMask.g - (pixelDirtMask.g * pixelDirt.g);
                        _dirtAmount -= removedAmount;

                        _dirtMaskTexture.SetPixel(
                            pixelXOffset + x,
                            pixelYOffset + y,
                            new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                        );
                    }
                }
                _dirtMaskTexture.Apply();
            }
        }

        if (!Input.GetKeyDown(KeyCode.Space)) return;
        _isFlipped = !_isFlipped;
        _solarAnimation.Play(_isFlipped ? "SolarPanelFlip" : "SolarPanelFlipBack");
    }

    private float GetDirtAmount()
    {
        return _dirtAmount / _dirtAmountTotal;
    }
}