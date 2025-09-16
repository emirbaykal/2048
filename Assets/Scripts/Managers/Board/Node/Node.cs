using Tile;
using UnityEngine;

namespace Managers.Board.Node
{
    public class Node : MonoBehaviour
    {
        public Vector2 Pos => transform.position;
    
        public TileController OccupiedTile;
    
    }
}