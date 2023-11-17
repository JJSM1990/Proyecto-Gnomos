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
        falling, jumping, moving, stopped, stacking
    }
    private PlayerState _currentPlayerState;


    public Vector2 _flatMoveInputV2 { get; private set; }
    private Vector3 _playerMovement;


    [Header("Player Movement Variables")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _jumpingSpeed;
    [SerializeField] private float _fallingSpeed;
    [SerializeField] private float _playerWeight;

    private Vector3 _lastRotation;

    [Header("Stack Variables")]
    [SerializeField] private bool _stackBool;
    [SerializeField] private float _stackCountTarget;
    [SerializeField] private int _stackAmount;
    [SerializeField] private float _stackMultiplier;
    [SerializeField] private List<GameObject> _stackList;
    [SerializeField] private GameObject m_stackListParent;

    [Header("Components")]
    [SerializeField] private InputActions _inputActions;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private List<GameObject> _activatedGnomes;



    // Todo esto son eventos. No estoy seguro que esta ocurriendo aqui.
    // Leyre te lo explico en persona. Pero basicamente, estoy usando el nuevo sistema de Input de Unity para controlar el jugador sin estar usando el Update como haciamos antes.

    private void OnEnable()
    {
        _inputActions = new InputActions();
        _inputActions.GnomeKingLand.Enable();
        _inputActions.GnomeKingLand.Jump.started += Jump;
        _inputActions.GnomeKingLand.Movement.performed += PlayerIsMoving;
        _inputActions.GnomeKingLand.Movement.performed += CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled += CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled += PlayerIsStopped;
        _inputActions.GnomeKingLand.StackGnomes.performed += StartStack;
        _inputActions.GnomeKingLand.StackGnomes.canceled += EndStack;
    }
    private void OnDisable()

    {
        _inputActions.GnomeKingLand.Jump.started -= Jump;
        _inputActions.GnomeKingLand.Movement.started -= PlayerIsMoving;
        _inputActions.GnomeKingLand.Movement.performed -= CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled -= CaptureMovementInput;
        _inputActions.GnomeKingLand.Movement.canceled -= PlayerIsStopped;
        _inputActions.GnomeKingLand.StackGnomes.performed -= StartStack;
        _inputActions.GnomeKingLand.StackGnomes.canceled -= EndStack;
        _inputActions.GnomeKingLand.Disable();
    }

    private void Start()
    {
        _lastRotation = transform.rotation.eulerAngles;
    }
    private void Update()
    {
        if (_stackBool)
        {
            _stackCountTarget += (Time.deltaTime * _stackMultiplier);
        }
        CheckFalling();
        CharacterMovement();
        CharacterRotation();
    }

    #region Movement
    private void CharacterMovement()
    {
        Vector3 playerMovement = MovementControl();
        m_characterController.Move(playerMovement * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        Vector3 lookAt = _playerMovement.magnitude > 0.5f ? _playerMovement : _lastRotation;
        switch (_currentPlayerState)
        {
            case PlayerState.falling:
            case PlayerState.jumping:
                lookAt.y = 0f;
                if (lookAt.magnitude > 0f)
                {
                    transform.rotation = Quaternion.LookRotation(lookAt);
                }
                break;
            case PlayerState.moving:
                transform.rotation = Quaternion.LookRotation(lookAt);
                break;
            default:
                break;
        }

        _lastRotation = lookAt;
    }

    private void CaptureMovementInput(InputAction.CallbackContext context)
    {
        _flatMoveInputV2 = context.ReadValue<Vector2>();
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (_currentPlayerState == PlayerState.stopped || _currentPlayerState == PlayerState.moving)
        {
            _currentPlayerState = PlayerState.jumping;
            StartCoroutine(EndJump());
        }
    }
    private void PlayerIsMoving(InputAction.CallbackContext context)
    {
        if (m_characterController.isGrounded)
        {
            _currentPlayerState = PlayerState.moving;
        }
    }

    private void PlayerIsStopped(InputAction.CallbackContext context)
    {
        if (m_characterController.isGrounded)
        {
            _currentPlayerState = PlayerState.stopped;
        }
    }
    private Vector3 MovementControl()
    {
        //Segun _currentPlayerState vamos a ir rotando por lo que el jugador puede hacer o no

        switch (_currentPlayerState)
        {
            case PlayerState.moving:
            case PlayerState.stopped:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = -0.5f;
                break;
            case PlayerState.jumping:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = _jumpingSpeed;
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
        if (horizontalInput.magnitude < 1)
        {
            horizontalInput.Normalize();
        }
        _playerMovement.x = horizontalInput.x * _playerSpeed;
        _playerMovement.z = horizontalInput.y * _playerSpeed;
    }
    private void CheckFalling()
    {
        if (m_characterController.isGrounded && !(_currentPlayerState == PlayerState.moving || _currentPlayerState == PlayerState.stopped || _currentPlayerState == PlayerState.jumping))
        {
            _currentPlayerState = PlayerState.moving;
        }
        else if (!m_characterController.isGrounded && _currentPlayerState != PlayerState.falling)
        {
            _currentPlayerState = PlayerState.falling;
        }
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(0.1f);
        _currentPlayerState = PlayerState.falling;
    }


    #endregion


    #region GnomeManagement
    public void AddGnomeToFollowerList(GameObject gnome)
    {
        _activatedGnomes.Add(gnome);
    }

    public void RemoveGnomeFromFollowerList(GameObject Gnome)
    {
        _activatedGnomes.Remove(Gnome);
    }
    #endregion

    #region Stacking

    private void StartStack(InputAction.CallbackContext context)
    {
        _stackCountTarget = 0;
        _currentPlayerState = PlayerState.stacking;
        _stackBool = true;
    }

    private void EndStack(InputAction.CallbackContext context)
    {
        _stackBool = false;
        _stackAmount = Mathf.FloorToInt(_stackCountTarget);
        ExecuteStack();

    }

    private void ExecuteStack()
    {
        Debug.Log(CheckGnomesVSStack());
        if (CheckGnomesVSStack())
        {
            addGnomesToStackList(_stackAmount);
            SpawnEmpties(_stackList.Count);
        }
    }

    private bool CheckGnomesVSStack()
    {
        if (_stackAmount == 0 || _activatedGnomes.Count == 0) return false;
        if (_stackAmount < _activatedGnomes.Count) return true;
        _stackAmount = _activatedGnomes.Count;
        return true;
    }

    private void addGnomesToStackList(int stack)
    {
        while (stack > 0)
        {
            _stackList.Add(_activatedGnomes[0]);
            _activatedGnomes.Remove(_activatedGnomes[0]);
            stack--;
        }
    }

    private void SpawnEmpties(int AmountGnomes)
    {
        Vector3 lastPoint = transform.position;
        for (int i = 0; i < AmountGnomes; i++)
        {
            GameObject newStackEmpty = Instantiate(new GameObject(), m_stackListParent.transform);
            newStackEmpty.transform.position = lastPoint;
            lastPoint.y += _stackList[i].GetComponent<CapsuleCollider>().height;
        }
    }

    #endregion
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var pushInterface = hit.collider.GetComponent<IPushableByPlayer>();
        if (pushInterface != null && hit.moveDirection.y > -0.6f)
        {
            Vector3 pushStrength = _playerWeight * _playerMovement;
            pushInterface.Push(pushStrength);
        }

    }
}
