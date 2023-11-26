using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static GnomeBrain;
using static UIManager;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    //Para controllar el movimiento del jugador usare el enum PlayerState.
    //La funcion FindPlayerState() mira que esta haciendo el jugador y cambia _currentPlayerState.
    //Si queremos añadir nuevas cosas es mucho mas sencillo hacerlo asi y mantenemos el codigo limpio (por ejemplo slide, sprint, etc)
    private enum PlayerState
    {
        Falling, Jumping, Moving, ExecutingStack, Stacking, Dead
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
    [SerializeField] private float                      _callToStackRange;
    private int                                         _gnomesInRangeCounter;
    [SerializeField] private float                      _playerSpeedStacked;
    [SerializeField] private bool                       _stackBool;
    [SerializeField] private float                      _stackTargerCounter;
    [SerializeField] private int                        _stackAmount;
    [SerializeField] private float                      _stackMultiplier;
    [SerializeField] private float                      _timeToExecute;

    [SerializeField] private float                      _playerHeightDifference;
    private GameObject                                  m_stackParent;


    [Header("Components")]
    [SerializeField] private CharacterController        m_characterController;
    private InputActions                                m_inputActions;
    private GameObject                                  m_gnomeModel;
    private Rigidbody                                   m_rb;
    [SerializeField] private Animator                   m_anim;

    [Header("GnomeLists")]
    [SerializeField] private Transform                  m_activatedGnomesList;
    [SerializeField] private Transform                  m_inactiveGnomeList;
    [SerializeField] private Transform                  m_stackGnomeList;

    // Todo esto son eventos. No estoy seguro que esta ocurriendo aqui.
    // Leyre te lo explico en persona. Pero basicamente, estoy usando el nuevo sistema de Input de Unity para controlar el jugador sin estar usando el Update como haciamos antes.

    private void Awake()
    {
        m_rb= GetComponent<Rigidbody>();    
        m_rb.isKinematic = true;
    }
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
        m_inputActions.GnomeKingLand.Pause.performed += Pause;
    }
    private void OnDisable()

    { 
        m_inputActions.GnomeKingLand.Jump.started -= Jump;
        m_inputActions.GnomeKingLand.Movement.performed -= CaptureMovementInput;
        m_inputActions.GnomeKingLand.Movement.canceled -= CaptureMovementInput;
        m_inputActions.GnomeKingLand.StackGnomes.performed -= StartStackCount;
        m_inputActions.GnomeKingLand.StackGnomes.canceled -= EndStackCount;
        m_inputActions.GnomeKingLand.Pause.performed -= Pause;
        m_inputActions.GnomeKingLand.Disable();
    }
    #endregion
    private void Start()
    {
        _lastRotation = transform.rotation.eulerAngles;
        m_gnomeModel=transform.GetChild(0).gameObject;
        m_stackParent=transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        if (_stackBool)
        {
            _stackTargerCounter += (Time.deltaTime * _stackMultiplier);
            GameManager.Instance.UpdateStackUI(_stackTargerCounter);
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
            case PlayerState.Dead:
                m_anim.SetFloat("AnimationSpeed", 0);
                break;
            case PlayerState.ExecutingStack:
                break;
            case PlayerState.Stacking:
                playerMovement = MovementControl();
                m_characterController.Move(playerMovement * Time.deltaTime*_playerSpeedStacked);
                break;
            case PlayerState.Moving:
                playerMovement = MovementControl();
                if (playerMovement.magnitude> 0.5f) m_anim.SetFloat("AnimationSpeed", 3);
                    else m_anim.SetFloat("AnimationSpeed", 0);
                m_characterController.Move(playerMovement * (Time.deltaTime * _playerSpeed));
                break;
            default:
                playerMovement = MovementControl();
                m_anim.SetFloat("AnimationSpeed", 0);
                m_characterController.Move(playerMovement * (Time.deltaTime* _playerSpeed));
                break;
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        GameManager.Instance.PauseGame();
    }
    private void CharacterRotation()
    {
        Vector3 lookAt = _playerMovement.magnitude > 0.5f ? _playerMovement: _lastRotation;
        lookAt.y = 0;
        switch (_currentPlayerState)
        {
            case PlayerState.Dead:
                break;
            case PlayerState.Falling:
            case PlayerState.Jumping:
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
            case PlayerState.Falling:
                break;
            case PlayerState.Jumping:
                break;
            case PlayerState.Moving:
                _currentPlayerState = PlayerState.Jumping;
                SoundManager.Instance.PlayFx(AudioFX.Jump, SoundManager.Instance.clipAudioSource);
                StartCoroutine(EndJump());
                break;
            case PlayerState.ExecutingStack:
                break;
            case PlayerState.Stacking:
                CancelStack();
                _currentPlayerState = PlayerState.Jumping;
                StartCoroutine(EndJump());
                break;
        }
    }

    private Vector3 MovementControl()
    {
        //Segun _currentPlayerState vamos a ir rotando por lo que el jugador puede hacer o no
        switch (_currentPlayerState)
        {
            case PlayerState.Dead:
                break;
            case PlayerState.ExecutingStack:
                _playerMovement = Vector3.zero;
                break;
            case PlayerState.Jumping:
                TranslatingHorizontalInputToMovement(_flatMoveInputV2);
                _playerMovement.y = _jumpingSpeed;
                break;
            case PlayerState.Falling:
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
            
                case PlayerState.Falling:
                if (m_characterController.isGrounded)
                {
                    _currentPlayerState = PlayerState.Moving;
                }
                    break;
                default:
                if (!m_characterController.isGrounded)
                {
                    switch (_currentPlayerState)
                    {
                        case PlayerState.Jumping:
                        case PlayerState.Falling:
                            break;
                        case PlayerState.Stacking:
                            CancelStack();                            
                            _currentPlayerState = PlayerState.Falling;
                            break;
                        default:
                            _currentPlayerState = PlayerState.Falling;
                            break;
                    }
                }
                    break;
            }
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(0.1f);
        _currentPlayerState = PlayerState.Falling;
    }


    #endregion


    #region GnomeManagement
    public void AddGnomeToFollowerList(GameObject gnome)
    {
        gnome.transform.SetParent(m_activatedGnomesList.transform);
    }

    public void RemoveGnomeFromFollowerList(GameObject Gnome)
    {
        Gnome.transform.SetParent(m_inactiveGnomeList.transform);
    }

    public int ReturnTotalGnomeFollowers()
    {
        int count= m_activatedGnomesList.childCount+m_stackGnomeList.childCount;
        return count;
    }
    #endregion

    #region Stacking

    private void StartStackCount(InputAction.CallbackContext context)
    {
        if(_currentPlayerState== PlayerState.Moving)
        {
            CheckGnomesInRange();
            _stackTargerCounter = 0;
            _currentPlayerState = PlayerState.ExecutingStack;
            _stackBool = true;
            GameManager.Instance.StartStackUI(_gnomesInRangeCounter);
        }

    }

    private void EndStackCount(InputAction.CallbackContext context)
    {
        if (_currentPlayerState==PlayerState.ExecutingStack)
        {
            _stackBool = false;
            _stackAmount = Mathf.FloorToInt(_stackTargerCounter);
            ExecuteStack();
            GameManager.Instance.EndStackUI();
        }
    }

    private void ExecuteStack()
    {
        
        if (CheckAvailableGnomesVSStackCount())
        {
            addGnomesToStackList(_stackAmount);
            SoundManager.Instance.PlayFx(AudioFX.Tower, SoundManager.Instance.clipAudioSource);
            SpawnEmptiesAndPlaceCharacters(m_stackGnomeList.childCount);
        } else
        {
            _currentPlayerState = PlayerState.Moving;
        }
    }
    private void CheckGnomesInRange()
    {
        _gnomesInRangeCounter = 0;
        float distanceToPlayer;
        GameObject gnome;
        for (int i= 0; i< m_activatedGnomesList.childCount; i++)
        {
            gnome = m_activatedGnomesList.GetChild(i).gameObject;
            distanceToPlayer = Vector3.Distance(gnome.transform.position, transform.position);
            if (distanceToPlayer<_callToStackRange && gnome.GetComponent<GnomeBrain>().HasPathToPlayer())
            {
                gnome.GetComponent<GnomeBrain>().ChangeInRangeOfStackCall(true);
                _gnomesInRangeCounter++;
            }
        }
        Debug.Log(_gnomesInRangeCounter);
    }
    private bool CheckAvailableGnomesVSStackCount()
    {
        if (_stackAmount == 0 || _gnomesInRangeCounter == 0) return false;
        if(_stackAmount>_gnomesInRangeCounter) _stackAmount=_gnomesInRangeCounter; ;
        return true;
    }

    private void  addGnomesToStackList(int stack)
    {
        float RemainingToStackTarget = stack;
        int currentGnome = 0;
        Debug.Log(RemainingToStackTarget);
        GameObject gnome;
        while (RemainingToStackTarget>0)
        {
            Debug.Log(RemainingToStackTarget);
            if (m_activatedGnomesList.childCount == 0) break;
            gnome = m_activatedGnomesList.GetChild(currentGnome)?.gameObject;
            Debug.Log((gnome != null) + " " + RemainingToStackTarget);
            if (gnome!=null&&gnome.GetComponent<GnomeBrain>().ReturnIfInRangeOfStackCall())
            {
                Debug.Log("gnome " + RemainingToStackTarget);
                gnome.transform.SetParent(m_stackGnomeList);
                RemainingToStackTarget--;
            } else
            {
                currentGnome++;
            }
        }
    }

    private void SpawnEmptiesAndPlaceCharacters(int AmountGnomes)
    {
        Vector3 lastPoint= transform.position;
        for (int i = 0; i < AmountGnomes; i++)
        {
            var currentGnome = m_stackGnomeList.GetChild(i);
            GameObject newStackEmpty= Instantiate(new GameObject(), m_stackParent.transform);
            newStackEmpty.name = "StackEmpty";
            newStackEmpty.transform.position = lastPoint;
            currentGnome.GetComponent<GnomeBrain>().ExecuteStack(newStackEmpty, _timeToExecute);
            lastPoint.y += currentGnome.GetComponent<CapsuleCollider>().height+0.1f;
        }
        _playerHeightDifference= lastPoint.y-transform.position.y;
        m_characterController.height= _playerHeightDifference;
        m_characterController.center = new Vector3(0,_playerHeightDifference / 2,0);
        StartCoroutine(PlacePlayer(lastPoint, _timeToExecute));
    }
    
    public void CancelStack ()
    {
        if (_currentPlayerState == PlayerState.Stacking)
        {
            Transform gnome;
            while (m_stackGnomeList.childCount > 0)
            {
                gnome = m_stackGnomeList.GetChild(0);
                gnome.gameObject.GetComponent<GnomeBrain>()?.CancelStack();
                gnome.SetParent(m_activatedGnomesList);
            }
            m_gnomeModel.transform.position = transform.position;
            m_characterController.height = 0.9f;
            m_characterController.center = new Vector3(0, 0.45f, 0);
            m_characterController.Move(new Vector3(0, _playerHeightDifference, 0));
            for (int i = 0; i < m_stackParent.transform.childCount; i++)
            {
                Destroy(m_stackParent.transform.GetChild(m_stackParent.transform.childCount - i-1).gameObject);
                SoundManager.Instance.PlayFx(AudioFX.Jump, SoundManager.Instance.clipAudioSource);
            }
        }
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
        _currentPlayerState = PlayerState.Stacking;
    }
    #endregion

    #region PlayerDeath
    public void Kill(CauseOfDeath causeofDeath)
    {
        //GameManager.Instance;
        m_rb.isKinematic= false;
        m_characterController.enabled = false;
        CancelStack();
        _currentPlayerState = PlayerState.Dead;
        m_rb.AddForce(transform.up * 10 + transform.forward * -1, ForceMode.Impulse);
        GameManager.Instance.BeginRespawn(causeofDeath);
    }

    public void Respawn()
    {
        Debug.Log("PlayerRevived");
        m_rb.isKinematic = true;
        m_characterController.enabled = true;
        SoundManager.Instance.PlayAudioSource(SoundManager.Instance.musicAudioSource);
        _currentPlayerState = PlayerState.Falling;
    }
#endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var pushInterface = hit.collider.GetComponent<IPushableByPlayer>();
        if (pushInterface != null && hit.moveDirection.y > -0.6f)
        {
            Vector3 pushStrength;
            switch (_currentPlayerState)
            {
                case PlayerState.Stacking:
                    pushStrength = _playerWeight * _playerMovement * m_stackGnomeList.childCount;
                    pushInterface.Push(pushStrength, m_stackGnomeList.childCount);
                    break;
                default:
                    pushStrength = _playerWeight * _playerMovement;
                    pushInterface.Push(pushStrength,1);
                    break;
            }

        }
    }
}
