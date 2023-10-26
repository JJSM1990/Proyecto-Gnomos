using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickGroup
{
    [SerializeField] public List<GameObject>  _members { get; private set; } = new List<GameObject>();

    public void AddMember(GameObject newMember)
    {
        _members.Add(newMember);
    }

    public void RemoveMember(GameObject newMember)
    {
        _members.Remove(newMember);
    }
}
