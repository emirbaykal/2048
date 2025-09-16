using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Installers.Tile.SO;
using Signals;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Node = Managers.Board.Node.Node;

namespace Tile
{
    public class TileController : MonoBehaviour
    {
        private SignalBus _bus;
        public Vector2 Pos => transform.position;
        public Node Node;
        public TileController MergingTile;
        public bool Merging;

        private Dictionary<int, TilesData> _tilesLookup;
        private TileView _tileView;
        private TileModel _tileModel;

        public int Value => _tileModel.Value;
        
        
        [Inject]
        public void Construct(SignalBus bus, Dictionary<int, TilesData> tilesLookup ,TileModel tileModel)
        {
            _bus = bus;
            _tilesLookup = tilesLookup;
            _tileModel = tileModel;
            
            _tileView = GetComponentInChildren<TileView>();
        }

        //MERGE
        public void ApplyMerge(int Value)
        {
            if (_tileModel.WinCheck(_tilesLookup, Value)) _bus.Fire(new Win());
    
            _tileModel.GetNextData(_tilesLookup, Value);
            _tileView.UpdateTileView(_tileModel.GetNextData(_tilesLookup,Value));
            MergingTile = null;
            Merging = false;
        }
        public bool CanMerge(int value)
        {
            return _tileModel.CanMerge(value, Merging, MergingTile);
        }
        
        public void MergeTile(TileController tileToMergeWith)
        {
            MergingTile = tileToMergeWith;
            
            Node.OccupiedTile = null;
            tileToMergeWith.Merging = true;
        }
        
        //
        
        //DATA UPDATE
        public void SetNodeData(Node node)
        {
            if (Node != null)
            {
                Node.OccupiedTile = null;
            }
            Node = node;
            Node.OccupiedTile = this;
        }
        
       public void UpdateBeginnerTileData(int value)
       {
           _tileView.UpdateTileView(_tileModel.GetCurrentData(_tilesLookup,value));
       }
        
        
        public class Pool : MonoMemoryPool<TileController>
        {
            protected override void Reinitialize(TileController item)
            {
                item.gameObject.SetActive(true);
                item._tileModel = new TileModel();
            }

            protected override void OnDespawned(TileController item)
            {
                item.gameObject.SetActive(false);
                item.MergingTile = null;
                item.Node = null;
                item.Merging = false;
                item.DOKill();
            }
        }
    }
}