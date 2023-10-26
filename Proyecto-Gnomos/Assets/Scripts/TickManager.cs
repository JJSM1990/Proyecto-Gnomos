using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [SerializeField] private List<TickGroup> _tickGrouping = new List<TickGroup>();
    [SerializeField] private float _maximumTimeBetweenFrames;
    [SerializeField] private float _timeBetweenTicks;
    private Coroutine _runningCoroutine=null;

    private void Start()
    {
        _runningCoroutine = StartCoroutine(TickUpdate());
        _tickGrouping.Add(new TickGroup());
        _tickGrouping.Add(new TickGroup());
    }
    private void Update()
    {
        if (Time.deltaTime > _maximumTimeBetweenFrames)
        {
            CreateNewGroup();
        }

        if (_runningCoroutine == null && _tickGrouping.Count > 0)
        {
            _runningCoroutine = StartCoroutine(TickUpdate());
        } 
    }

    public void AddObjectToAGroup(GameObject me)
    {
        CheckIfRunningCoroutine();
        TickGroup targetGroup = null;
        if (_tickGrouping.Count == 0 )
        {
            targetGroup = new TickGroup();
            _tickGrouping.Add( targetGroup );
        }
        else if (_tickGrouping.Count==1)
        {
            targetGroup = _tickGrouping[0];
        } else
        {
            _tickGrouping = _tickGrouping.OrderBy(o => o._members.Count).ToList();
            targetGroup = _tickGrouping[0];
        }
        targetGroup.AddMember(me);
        Debug.Log(_tickGrouping.Count);
        //int i = 0;
        //foreach (TickGroup group in _tickGrouping)
        //{
        //    i++;
        //    Debug.Log("Group " + i + ": " + group._members.Count);
        //}
    }

    private void CheckIfRunningCoroutine()
    {
        if (_runningCoroutine != null)
        {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
        }
    }
    public void DeleteGroupFromManager(TickGroup group)
    {
        CheckIfRunningCoroutine();
        _tickGrouping.Remove(group);
    }
    private void CreateNewGroup()
    {
        CheckIfRunningCoroutine();
        TickGroup newGroup= new TickGroup();
        foreach (TickGroup group in _tickGrouping)
        {
            if (group._members.Count>0)
            {
                newGroup.AddMember(group._members[0]);
                group._members.RemoveAt(0);
                
            }
        }
        _tickGrouping.Add(newGroup);
    }


    IEnumerator TickUpdate()
    {
        int i = 0;
        foreach(TickGroup group in _tickGrouping)
        {
            if (group._members.Count > 0)
            {
                foreach (GameObject member in group._members)
                {
                    member.GetComponent<UpdateThroughTick>()?.UpdateTick();
                }
                i++;
                Debug.Log("group "+i+" updated, "+ group._members.Count+" members");
                yield return new WaitForSeconds(_timeBetweenTicks);
            }
        }
        _runningCoroutine = null;
    }
}
