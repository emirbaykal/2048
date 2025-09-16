using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Installers.Tile.SO
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjects/TileData")]
    public class TilesData : ScriptableObject
    {
        [field: SerializeField] public int value { get; private set; } 
        [field: SerializeField] public Color backgroundColor { get; private set; } 
        [field: SerializeField] public Color textColor { get; private set; } 
    }
    [CreateAssetMenu(menuName = "Installers/TilesInstaller")]
    public class SOTileInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private List<TilesData> tilesData;
        public override void InstallBindings()
        {
            var tilesLookUp = tilesData.ToDictionary(tile => tile.value, tile => tile);
            Container.BindInstance(tilesLookUp).AsSingle();
        }
    }
}