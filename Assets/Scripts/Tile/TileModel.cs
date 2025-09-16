using System.Collections.Generic;
using System.Linq;
using Installers.Tile.SO;
using UnityEngine;

namespace Tile
{
    public class TileModel
    {
        public int Value { get; private set; }
        //Since we only get 2 and 4 for spawn, we take it this way.

        public TilesData GetNextData(Dictionary<int, TilesData> tilesData, int oldValue)
        {
            int nextDataKey = oldValue * 2;

            // Next tile data
            if (tilesData.TryGetValue(nextDataKey, out var data))
            {
                Value = data.value;
                return data;
            }
            
            return null;
        }

        public TilesData GetCurrentData(Dictionary<int, TilesData> tilesData, int currentValue)
        {
            if (tilesData.TryGetValue(currentValue,out var data))
            {
                Value = data.value;
                return data;
            }

            return null;
        }

        public bool CanMerge(int value, bool Merging, TileController MergingTile)
        {
            return value == Value && !Merging && MergingTile == null;
        }

        
        //The key of the last data we created from ScriptableObject Installer is retrieved.
        //After merging, it is checked whether the resulting value is equal to the last data.     
        public bool WinCheck(Dictionary<int, TilesData> data, int mergeValue)
        {
            return data.Keys.Last() == mergeValue;
        }
        
    }
}