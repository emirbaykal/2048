using Signals;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputHandler : MonoBehaviour
{
    private BoardInputs _boardInputs;
    private SignalBus _bus;

    [Inject]
    public void Construct(BoardInputs boardInputs, SignalBus bus)
    {
        _bus = bus;
        _boardInputs = boardInputs;
    }
    
    private void OnEnable()
    {
        _boardInputs.Board.MoveTiles.performed += OnMove;

        _bus.Subscribe<GenerateLevel>(InputActive);
        _bus.Subscribe<Win>(InputDeactive);
        _bus.Subscribe<RestartGame>(InputDeactive);
    }
    
    private void OnDisable()
    {
        _boardInputs.Board.MoveTiles.performed -= OnMove;

        _bus.Unsubscribe<GenerateLevel>(InputActive);
        _bus.Unsubscribe<Win>(InputDeactive);
        _bus.Unsubscribe<RestartGame>(InputDeactive);

    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        Vector2Int dir;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // Delta büyükse swipe kabul et
            if (input.magnitude > 50f) // threshold
            {
                Debug.Log(input.magnitude);
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    dir = input.x > 0 ? Vector2Int.right : Vector2Int.left;
                else
                    dir = input.y > 0 ? Vector2Int.up : Vector2Int.down;
            }
            else
            {
                return; // çok küçük kaydırma → ignore
            }
        }
        else
        {
            // Klavye girdisi zaten round’lanabilir
            dir = Vector2Int.RoundToInt(input);
        }
        

        _bus.Fire(new Moving
            {
                direction = dir
            }
            );
    }
    
    private void InputActive()
    {
        _boardInputs.Board.Enable();
    }
    
    private void InputDeactive()
    {
        _boardInputs.Board.Disable();
    }
}
