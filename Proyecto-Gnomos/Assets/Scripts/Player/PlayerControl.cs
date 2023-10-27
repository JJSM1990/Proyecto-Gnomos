using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    //Para controllar el movimiento del jugador usare el enum PlayerState.
    //La funcion FindPlayerState() mira que esta haciendo el jugador y cambia _currentPlayerState.
    //Si queremos añadir nuevas cosas es mucho mas sencillo hacerlo asi y mantenemos el codigo limpio (por ejemplo slide, sprint, etc)
    private enum PlayerState
    {
        grounded, falling, jumping

    }
    private PlayerState _currentPlayerState;


    public Vector2                                      _flatMoveInputV2 { get; private set; }
    private Vector3                                     _playerMovement;
    [Header("Player Movement Variables")]
    [SerializeField] private float                      _playerSpeed;
    [SerializeField] private float                      _jumpingSpeed;
    [SerializeField] private float                      _fallingSpeed;
    [Header("Components")]
    [SerializeField] private InputActions               _inputActions;
    [SerializeField] private CharacterController        m_characterController;


    // Todo esto son eventos. No estoy seguro que esta ocurriendo aqui.
    // Leyre te lo explico en persona. Pero basicamente, estoy usando el nuevo sistema de Input de Unity para controlar el jugador sin estar usando el Update como haciamos antes.

    private void OnEnable()
    {
        _inputActions = new InputActions();
        _inputActions.GnomeKingLand.Enable();
        _inputActions.GnomeKingLand.Jump.started += Jump;
        _inputActions.GnomeKingLand.Movement.performed += CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled += CaptureMovementInput;        
    }
    private void OnDisable()

    { 
        _inputActions.GnomeKingLand.Jump.started -= Jump;
        _inputActions.GnomeKingLand.Movement.performed -= CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled -= CaptureMovementInput;
        _inputActions.GnomeKingLand.Disable();
    }

    private void Update()
    {
        FindPlayerState();
        m_characterController.Move(MovementControl() * Time.deltaTime);
        Debug.Log(_playerMovement.y);
    }
  
    
    private void CaptureMovementInput(InputAction.CallbackContext context)
    {
        _flatMoveInputV2 = context.ReadValue<Vector2>();
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (_currentPlayerState==PlayerState.grounded)
        {
            _currentPlayerState = PlayerState.jumping;
            StartCoroutine(EndJump());
        }
    }


    private Vector3 MovementControl()
    {
        //Segun _currentPlayerState vamos a ir rotando por lo que el jugador puede hacer o no

        switch (_currentPlayerState)
        {
            case PlayerState.grounded:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = -0.5f;
                break;
            case PlayerState.jumping:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = _jumpingSpeed;
                _currentPlayerState = PlayerState.falling;
                break;
            case PlayerState.falling:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y -= _fallingSpeed * Time.deltaTime;
                break;
            default:
                break;
        }
        return _playerMovement;
    }
    private void TranslatingHorizontalInputToMovement(Vector2 horizontalInput)
    {
        _playerMovement.x = horizontalInput.x * _playerSpeed;
        _playerMovement.z = horizontalInput.y * _playerSpeed;
    }
    private void FindPlayerState()
    {
        if (_currentPlayerState!=PlayerState.jumping)
        {
            if (m_characterController.isGrounded)
            {
                _currentPlayerState = PlayerState.grounded;
            } else
            {
                _currentPlayerState = PlayerState.falling;
            }
        }
        
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(0.1f);
        _currentPlayerState= PlayerState.falling;
    }
}
