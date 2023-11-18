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
        falling, jumping, moving, executingStack, stacking
    }
    [SerializeField] private PlayerState _currentPlayerState;


    public Vector2                                      _flatMoveInputV2 { get; private set; }
    private Vector3                                     _playerMovement;


    [Header("Player Movement Variables")]
    [SerializeField] private float                      _playerSpeed;
    [SerializeField] private float                      _jumpingSpeed;
    [SerializeField] private float                      _fallingSpeed;
    [SerializeField] private float                      _playerWeight;

    private Vector3                                     _lastRotation;

    [Header("Stack Variables")]
    [SerializeField] private float                      _playerSpeedStacked;
    [SerializeField] private bool                       _stackBool;
    [SerializeField] private float                      _stackCountTarget;
    [SerializeField] private int                        _stackAmount;
    [SerializeField] private float                      _stackMultiplier;
    [SerializeField] private float                      _timeToExecute;
     public List<GameObject>                            _stackList;
    [SerializeField] private float                      _playerHeightDifference;
    private GameObject                                  m_stackListParent;


    [Header("Components")]
    [SerializeField] private CharacterController        m_characterController;
    public List<GameObject>                             _activatedGnomes;
    private InputActions                                m_inputActions;
    private GameObject                                  m_gnomeModel;


    // Todo esto son eventos. No estoy seguro que esta ocurriendo aqui.
    // Leyre te lo explico en persona. Pero basicamente, estoy usando el nuevo sistema de Input de Unity para controlar el jugador sin estar usando el Update como haciamos antes.

    #region Events
    private void OnEnable()
    {
        m_inputActions = new InputActions();
        m_inputActions.GnomeKingLand.Enable();
        m_inputActions.GnomeKingLand.Jump.started += Jump;
        m_inputActions.GnomeKingLand.Movement.performed += CaptureMovementInput;
        m_inputActions.GnomeKingLand.Movement.canceled += CaptureMovementInput;    
        m_inputActions.GnomeKingLand.StackGnomes.performed += StartStackCount;
        m_inputActions.GnomeKingLand.StackGnomes.canceled += EndStackCount;
    }
    private void OnDisable()

    { 
        m_inputActions.GnomeKingLand.Jump.started -= Jump;
        m_inputActions.GnomeKingLand.Movement.performed -= CaptureMovementInput;
        m_inputActions.GnomeKingLand.Movement.canceled -= CaptureMovementInput;
        m_inputActions.GnomeKingLand.StackGnomes.performed -= StartStackCount;
        m_inputActions.GnomeKingLand.StackGnomes.canceled -= EndStackCount;
        m_inputActions.GnomeKingLand.Disable();
    }
    #endregion
    private void Start()
    {
        _lastRotation = transform.rotation.eulerAngles;
        m_gnomeModel=transform.GetChild(0).gameObject;
        m_stackListParent=transform.GetChild(1).gameObject;
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
        Vector3 playerMovement;
        switch (_currentPlayerState)
        {
            case PlayerState.executingStack:
                break;
            case PlayerState.stacking:
                playerMovement = MovementControl();
                m_characterController.Move(playerMovement * Time.deltaTime);
                break;
            default:
                playerMovement = MovementControl();
                m_characterController.Move(playerMovement * (Time.deltaTime* _playerSpeed));
                break;
        }
    }

    private void CharacterRotation()
    {
        Vector3 lookAt = _playerMovement.magnitude > 0.5f ? _playerMovement: _lastRotation;
        lookAt.y = 0;
        switch (_currentPlayerState)
        {
            case PlayerState.falling:
            case PlayerState.jumping:
                if (lookAt.magnitude>0f)
                {
                    transform.rotation = Quaternion.LookRotation(lookAt);
                }
                break;
            default:
                transform.rotation=Quaternion.LookRotation(lookAt);
                break;
        }

        _lastRotation= lookAt;
    }

    private void CaptureMovementInput(InputAction.CallbackContext context)
    {
        _flatMoveInputV2 = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        switch (_currentPlayerState)
        {
            case PlayerState.falling:
                break;
            case PlayerState.jumping:
                break;
            case PlayerState.moving:
                _currentPlayerState = PlayerState.jumping;
                StartCoroutine(EndJump());
                break;
            case PlayerState.executingStack:
                break;
            case PlayerState.stacking:
                CancelStack();
                _currentPlayerState = PlayerState.jumping;
                StartCoroutine(EndJump());
                break;
        }
    }

    private Vector3 MovementControl()
    {
        //Segun _currentPlayerState vamos a ir rotando por lo que el jugador puede hacer o no

        switch (_currentPlayerState)
        {
            case PlayerState.executingStack:
                _playerMovement = Vector3.zero;
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
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = -0.5f;
                break;
        }
        return _playerMovement;
    }
    private void TranslatingHorizontalInputToMovement(Vector2 horizontalInput)
    {
        if (horizontalInput.magnitude<1)
        {
            horizontalInput.Normalize();
        }
        _playerMovement.x = horizontalInput.x;
        _playerMovement.z = horizontalInput.y;
    }
    private void CheckFalling()
    {
            switch (_currentPlayerState)
            {
                case PlayerState.falling:
                if (m_characterController.isGrounded)
                {
                    _currentPlayerState = PlayerState.moving;
                }
                    break;
                default:
                if (!m_characterController.isGrounded)
                {
                    switch (_currentPlayerState)
                    {
                        case PlayerState.jumping:
                        case PlayerState.falling:
                            break;
                        case PlayerState.stacking:
                            CancelStack();
                            _currentPlayerState = PlayerState.falling;
                            break;
                        default:
                            _currentPlayerState = PlayerState.falling;
                            break;
                    }
                }
                    break;
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

    private void StartStackCount(InputAction.CallbackContext context)
    {
        if(_currentPlayerState== PlayerState.moving)
        {
            _stackCountTarget = 0;
            _currentPlayerState = PlayerState.executingStack;
            _stackBool = true;
        }
    }

    private void EndStackCount(InputAction.CallbackContext context)
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
            SpawnEmptiesAndPlaceCharacters(_stackList.Count);
        }
    }

    private bool CheckGnomesVSStack()
    {
        if (_stackAmount == 0 || _activatedGnomes.Count==0) return false;
        if (_stackAmount < _activatedGnomes.Count) return true;
        _stackAmount = _activatedGnomes.Count;
        return true;
    }

    private void  addGnomesToStackList(int stack)
    {
        while (stack > 0)
        {
            _stackList.Add(_activatedGnomes[0]);
            _activatedGnomes.Remove(_activatedGnomes[0]);
            stack--;
        }
    }

    private void SpawnEmptiesAndPlaceCharacters(int AmountGnomes)
    {
        Vector3 lastPoint= transform.position;
        for (int i = 0; i < AmountGnomes; i++)
        {
            var currentGnome = _stackList[i];
            GameObject newStackEmpty= Instantiate(new GameObject(), m_stackListParent.transform);
            newStackEmpty.transform.position = lastPoint;
            currentGnome.GetComponent<GnomeBrain>().ExecuteStack(newStackEmpty, _timeToExecute);
            lastPoint.y += currentGnome.GetComponent<CapsuleCollider>().height+0.1f;
        }
        Debug.Log(lastPoint);
        _playerHeightDifference= lastPoint.y-transform.position.y;
        StartCoroutine(PlacePlayer(lastPoint, _timeToExecute));
        Debug.Log(_playerHeightDifference);
    }
    
    private void CancelStack ()
    {
        while (_stackAmount>0)
        {
            _stackList[0].GetComponent<GnomeBrain>().CancelStack();
            _activatedGnomes.Add(_stackList[0]);
            _stackList.Remove(_stackList[0]);
            _stackAmount--;
        }
        m_gnomeModel.transform.position = transform.position;
        m_characterController.Move(new Vector3(0, _playerHeightDifference,0));      
    }
    private IEnumerator PlacePlayer(Vector3 position, float timeToExecute)
    {
        float timer = 0;
        while(timer< timeToExecute)
        {
            m_gnomeModel.transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, position.y, transform.position.z), timer/timeToExecute);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_gnomeModel.transform.position=position;
        _currentPlayerState = PlayerState.stacking;
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
