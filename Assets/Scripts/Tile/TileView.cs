using Installers.Tile.SO;
using TMPro;
using UnityEngine;

namespace Tile
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _valueText;

        public void UpdateTileView(TilesData data)
        {
            _spriteRenderer.color = data.backgroundColor;
            _valueText.text = data.value.ToString();
            _valueText.color = data.textColor;
        }
    }
}