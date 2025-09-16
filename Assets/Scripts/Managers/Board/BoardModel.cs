using System.Collections.Generic;
using System.Linq;
using Tile;
using UnityEngine;

namespace Managers.Board
{
    public class BoardModel
    {
        //Values of tiles to be spawned
        public Node.Node GetNodeAtPosition(Vector2 pos, List<Node.Node> nodes)
        {
            return nodes.FirstOrDefault(node => node.Pos == pos);
        }

        //After the left, right, up, and down inputs
        //it calculates the position where the tiles will go on the board.
        public Vector2 MovePoint(TileController tile)
        {
            return tile.MergingTile != null ? 
                tile.MergingTile.Node.Pos 
                : tile.Node.Pos;
        }

        public Vector3 BoardCenterPosition(int width, int height)
        {
            return new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        }
    }
}