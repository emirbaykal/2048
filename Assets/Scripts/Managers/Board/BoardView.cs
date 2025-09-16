using DG.Tweening;
using Tile;
using UnityEngine;

namespace Managers.Board
{
    public class BoardView : MonoBehaviour
    {
        public void SpawnTileAnim(TileController tile)
        {
            tile.transform.localScale = Vector3.zero;
            tile.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
        public void MoveTileAnim(Vector2 movePoint, TileController tile, Sequence sequence)
        {
            sequence.Insert(0, 
                tile.transform.DOMove(movePoint, 0.15f)
                    .SetEase(Ease.OutQuart));
        }

        public void FocusCameraOnBoard(Vector3 boardCenterPos)
        {
            if (Camera.main != null) 
                Camera.main.transform.position 
                    = new Vector3(boardCenterPos.x, boardCenterPos.y, -10);
        }
        
    }
}