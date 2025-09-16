using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Signals;
using Tile;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Managers.Board
{
    public class BoardController : MonoBehaviour
    {
        //Zenject BIND
        private DiContainer _container;
        private SignalBus _bus;
        private TileController.Pool _pool;
    
        //MVC
        private BoardModel _boardModel;
        private BoardView _boardView;

        [Header("Board Generator")]
        [SerializeField] 
        private int _gridWidth = 4;
        [SerializeField] 
        private int _gridHeight = 4;
        [SerializeField] 
        private Node.Node _nodePrefab;
    
        private List<Node.Node> _nodes;
        private List<TileController> _tiles;
    
        //For DoTween
        private Sequence _sequence;
    
        
        [Inject]
        public void Construct(SignalBus bus, TileController.Pool pool ,BoardModel boardModel, DiContainer container)
        {
            _container = container;
            _bus = bus;
            _pool = pool;
            _boardModel = boardModel;
        }

        private void OnEnable()
        {
            _bus.Subscribe<Moving>(HandleMove);
            _bus.Subscribe<GenerateLevel>(GenerateGrid);

            _boardView = GetComponent<BoardView>();
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<Moving>(HandleMove);
            _bus.Unsubscribe<GenerateLevel>(GenerateGrid);
        }

        public void TileSpawner(int amount)
        {
            var freeNodes = _nodes.Where(node => node.OccupiedTile == null) //Find empty nodes
                .OrderBy(b => Random.value) //Shuffle
                .ToList();
        
            //To avoid checking whether they are empty or not,
            //the empty ones are collected in a list and selected from there in the “amount” quantity.   
            foreach (var node in freeNodes.Take(amount))
            {
                SpawnTile(node, Random.value > 0.8f ? 4 : 2);
            }
        }

        private void SpawnTile(Node.Node node, int value)
        {
            var tile = _pool.Spawn();
        
            //Scale bounds effect
            _boardView.SpawnTileAnim(tile);
        
            //Node position
            tile.transform.position = node.Pos;
            tile.transform.rotation = Quaternion.identity;
        
            tile.UpdateBeginnerTileData(value);
        
            tile.SetNodeData(node);
        
            //All active threads on the board are kept in this list
            _tiles.Add(tile);
        }
        
        private void MergeTile(TileController baseTile, TileController mergingTile)
        {
            //Every time we merge, we replace the tile's data with the new one to avoid spawning a new tile. 
        
            //The process of sending one of the two merged files to the pool and updating the data of the other is performed here.
            RemoveTile(mergingTile);
        
            //Update next value 2->4->8...
            baseTile.ApplyMerge(baseTile.Value);
        }

        public void RemoveTile(TileController tile)
        {
            _tiles.Remove(tile);
            _pool.Despawn(tile);
        }

        public void RestartBoard()
        {
            foreach (var tile in _tiles.ToList())
            {
                RemoveTile(tile);
            }

            foreach (var node in _nodes.ToList())
            {
                node.OccupiedTile = null;
            } 
            TileSpawner(2);
        }
    
        //TRIGGER BY "MOVE" SIGNAL
        public void HandleMove(Moving signal)
        {

            if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying())
            {
                _sequence.Complete();
            }
        
            _sequence = DOTween.Sequence();
        
            foreach (var tile in GetOrderedTiles(signal.direction))
            {
                CalculateTileMovement(tile,signal.direction);
                _boardView.MoveTileAnim(_boardModel.MovePoint(tile), tile, _sequence);
            }

            _sequence.OnComplete(() =>
            {
                foreach (var tile in GetOrderedTiles(signal.direction).Where(tile => tile.MergingTile != null))
                {
                    MergeTile(tile.MergingTile, tile);
                }
            
                CheckLoseCondition();
                //After each move, one tile is spawned randomly.
            });
        }

        private void CalculateTileMovement(TileController tile, Vector2Int direction)
        {
            //The location where the tile will go is calculated.
            var next = tile.Node;
            do
            {
                tile.SetNodeData(next);

                var possibleNode = _boardModel.GetNodeAtPosition(next.Pos + direction, _nodes);
                if (possibleNode is not null)
                {
                    //We know a node is present
                    //If its possible to merge set merge
                    if (possibleNode.OccupiedTile is not null && possibleNode.OccupiedTile.CanMerge(tile.Value)) {
                        tile.MergeTile(possibleNode.OccupiedTile);
                    }
                    //Otherwise can we move to this spot
                    else if (possibleNode.OccupiedTile is null)
                    {
                        next = possibleNode; 
                    }
                }
            } while (next != tile.Node);
        }
        private List<TileController> GetOrderedTiles(Vector2Int direction)
        {
            //To prevent conflicts in the tiles,
            //we need to move from the end to the beginning,
            //which is why we are doing this sort. For left and down 
            var orderTiles = _tiles.OrderBy(tile => tile.Pos.x)
                .ThenBy(tile => tile.Pos.y)
                .ToList();
        
            //if it's right or up, reverse it
            if (direction == Vector2Int.right || direction == Vector2Int.up) orderTiles.Reverse();

            return orderTiles;
        }
    
        private void CheckLoseCondition()
        {
            var freeNodes = _nodes.Where(node => node.OccupiedTile == null).ToList();

            if (freeNodes.Count == 0)
            {
                _bus.Fire(new RestartGame());
            }
            else
            {
                TileSpawner(1);
            }
        }

    
        //TRIGGER BY "GENERATELEVEL" SIGNAL
        private void GenerateGrid()
        {
            _nodes = new List<Node.Node>();
            _tiles = new List<TileController>();

            //Board background size is being adjusted
            gameObject.transform.position = _boardModel.BoardCenterPosition(_gridWidth, _gridHeight);
        
            GetComponent<SpriteRenderer>().size = new Vector2(_gridWidth, _gridHeight);
        
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    var node = _container.InstantiatePrefab(_nodePrefab, new Vector3(x, y, 0), Quaternion.identity,
                        gameObject.transform);
                
                    _nodes.Add(node.GetComponent<Node.Node>());
                }
            }

            //When the game starts, 2 tiles are generated.
            TileSpawner(2);

            //The camera position is adjusted according to the board. 
            _boardView.FocusCameraOnBoard(_boardModel.BoardCenterPosition(_gridWidth, _gridHeight));

        }
    }
}