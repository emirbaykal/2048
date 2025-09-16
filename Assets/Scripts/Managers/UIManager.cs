using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("In Game UI Elements")]
        [SerializeField] 
        private Button RestartButton;

        [Header("Panels")] 
        [SerializeField] 
        GameObject WinPanel;
        
        //Zenject Binds
        private SignalBus _bus;

        [Inject]
        public void Construct(SignalBus bus)
        {
            _bus = bus;
        }

        private void Awake()
        {
            RestartButton.onClick.AddListener(() => _bus.Fire(new RestartGame()));
            
            _bus.Subscribe<RestartGame>(RestartUIPanels);
            _bus.Subscribe<Win>(Win);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<RestartGame>(RestartUIPanels);
            _bus.Unsubscribe<Win>(Win);
        }

        private void RestartUIPanels()
        {
            WinPanel.SetActive(false);
        }

        private void Win()
        {
            WinPanel.SetActive(true);
        }
        
        
    }
}