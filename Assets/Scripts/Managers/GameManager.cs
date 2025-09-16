using Managers.Board;
using Signals;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private SignalBus _bus;
        private BoardController _boardController;
        private BoardInputs _boardInputs;

        [Inject]
        public void Construct(SignalBus bus, BoardController boardController)
        {
            _bus = bus;
            _boardController = boardController;
        
            _bus.Subscribe<RestartGame>(OnRestart);
        
        }
        private void OnDisable()
        {
            _bus.Unsubscribe<RestartGame>(OnRestart);
        }

        private void Start()
        {
            _bus.Fire(new GenerateLevel());
        }

    
        //Game Restart
        private void OnRestart()
        {
            _boardController.RestartBoard();
        }
    }
}