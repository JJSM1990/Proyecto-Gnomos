using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GnomeBrain : MonoBehaviour, IUpdateThroughTick
{
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private GameObject m_player;
    [SerializeField] private TickManager m_tickManager;
    [SerializeField] private float _minimumDistanceToFollowPlayer;
    [SerializeField] private NavMeshAgent m_navAgent;
    [SerializeField] private Collider m_collider;
    [SerializeField] private Rigidbody m_rb;
    private GameObject m_stackPosition;
    private bool m_inRangeOfStack;
    [SerializeField] private Animator m_anim;

    private Vector3 m_scaleChange = new Vector3(1,0.1f,1);

    private enum GnomeState
    {
        inactive, followingPlayer, Stopped, InStack, MovingToStack, Falling
    }
    private GnomeState _currentGnomeState = GnomeState.inactive;
    private void Start()
    {

        m_gameManager = GameManager.Instance;
        m_player = m_gameManager.ReturnPlayer();
        m_tickManager = m_gameManager.ReturnTickManager();
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        switch (_currentGnomeState)
        {
            case GnomeState.InStack:
                transform.position = m_stackPosition.transform.position;
                transform.rotation = m_stackPosition.transform.rotation;
                m_anim.SetFloat("AnimationSpeed", 0);
                break;
            case GnomeState.Falling:
                limitRBvelocity();
                m_anim.SetFloat("AnimationSpeed", 0);
                break;
            case GnomeState.followingPlayer:
                float AnimSpeed=(m_navAgent.velocity.magnitude / m_navAgent.speed)*4;
                m_anim.SetFloat("AnimationSpeed", AnimSpeed);
                break;
            default:
                m_anim.SetFloat("AnimationSpeed", 0);
                break;
        }
    }

    // Update is called once per frame

    public void UpdateTick()
    {
        float distanceToPlayer = Vector3.Distance(m_player.transform.position, this.gameObject.transform.position);
        switch (_currentGnomeState)
        {
            case GnomeState.followingPlayer:
                if (distanceToPlayer > _minimumDistanceToFollowPlayer && NavMesh.SamplePosition(m_player.transform.position, out NavMeshHit hitFollow, 2f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    m_navAgent.CalculatePath(hitFollow.position, path);
                    m_navAgent.SetPath(path);
                }
                else
                {
                    _currentGnomeState = GnomeState.Stopped;
                    m_navAgent.isStopped = true;
                }
                break;

            case GnomeState.Stopped:
                if (distanceToPlayer > _minimumDistanceToFollowPlayer)
                {
                    m_navAgent.isStopped = false;
                    _currentGnomeState = GnomeState.followingPlayer;
                }
                break;

            default:
                break;
        }
    }
    private void limitRBvelocity()
    {
        if (m_rb.velocity.magnitude > 5)
        {
            m_rb.velocity = Vector3.ClampMagnitude(m_rb.velocity, 5f);
        }
    }
    private void SwitchNavMeshAgent(bool onOff)
    {
        m_navAgent.enabled = onOff;
    }
    public void Activate()
    {
        if (_currentGnomeState == GnomeState.inactive)
        {
            m_navAgent.enabled = true;
            transform.localScale= new Vector3(1,1,1);   
            m_player.GetComponent<PlayerControl>()?.AddGnomeToFollowerList(this.gameObject);
            _currentGnomeState = GnomeState.followingPlayer;
            m_tickManager.AddObjectToAGroup(this.gameObject);
            SoundManager.Instance.PlayFx(AudioFX.GetGnome, SoundManager.Instance.clipAudioSource);
        }
    }

    public void Deactivate()
    {
        if (_currentGnomeState != GnomeState.inactive)
        {
            m_player.GetComponent<PlayerControl>().RemoveGnomeFromFollowerList(this.gameObject);
            m_navAgent.enabled = false;
            _currentGnomeState = GnomeState.inactive;
        }
    }
    public bool HasPathToPlayer()
    {
        if (m_player == null) return false;
        NavMeshPath path = new NavMeshPath();
        m_navAgent.CalculatePath(m_player.transform.position, path);
        var status = path.status;
        if (status == NavMeshPathStatus.PathComplete) return true;
        return false;

    }

    public void RunnedOver()
    {
        if(_currentGnomeState != GnomeState.inactive)
        {
            transform.localScale = m_scaleChange;
            Deactivate();
        }

    }
    public void ChangeInRangeOfStackCall(bool value)
    {
        m_inRangeOfStack = value;
    }

    public bool ReturnIfInRangeOfStackCall()
    {
        return m_inRangeOfStack;
    }
    public void ExecuteStack(GameObject position, float timeToExecute)
    {
        SwitchNavMeshAgent(false);
        m_collider.enabled= false;
        m_stackPosition = position;
        _currentGnomeState = GnomeState.MovingToStack;
        StartCoroutine(Stack(m_stackPosition.transform.position, timeToExecute));
    }
    public void CancelStack()
    {
        SwitchToFalling();
        Vector3 pushVector = new Vector3(Random.Range(-1f, 1f), 3, -2);
        pushVector = Vector3.Scale(pushVector, transform.forward)*20;
        m_rb.AddForce(pushVector, ForceMode.Impulse);
        StartCoroutine(turnOffOnCollider(0.05f));
        ChangeInRangeOfStackCall(false);
    }

    public void SwitchToFalling()
    {
        m_collider.enabled = true;
        m_rb.isKinematic = false;
        _currentGnomeState = GnomeState.Falling;
    }
    private IEnumerator Stack(Vector3 endingPosition, float timeToExecute)
    {
        float timer = 0f;
        Vector3 startPosition = transform.position;
        Debug.Log(timeToExecute);
        while (timer < timeToExecute)
        {
            transform.position = Vector3.Slerp(startPosition, endingPosition, timer / timeToExecute);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _currentGnomeState = GnomeState.InStack;
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (_currentGnomeState)
        {

            case GnomeState.Falling:
                if (other.collider.gameObject.tag=="Enviroment")
                {
                    m_rb.isKinematic = true;
                    SwitchNavMeshAgent(true);
                    _currentGnomeState = GnomeState.followingPlayer;
                }
                break;
            default:
                break;
        }
    }
    private IEnumerator turnOffOnCollider(float time)
    {
        m_collider.enabled= false;
        yield return new WaitForSeconds(time);
        m_collider.enabled= true;
    }

}
