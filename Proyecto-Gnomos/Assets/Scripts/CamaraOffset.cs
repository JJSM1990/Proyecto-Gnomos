using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Prueba1 : MonoBehaviour
{
    [SerializeField] Vector3 _offset = new Vector3(0, 10, -10);
    [SerializeField] float _smoothTime = 0.25f;
    private Vector3 _velocity = Vector3.zero;

    private Coroutine _coroutine;


    [SerializeField] private Transform target;

    [SerializeField] float duration = 1f;



    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);

        //if(_coroutine == null)
        //{
        //    _coroutine = StartCoroutine(Shaking());
        //}
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.transform.position + Random.insideUnitSphere;
        targetPosition.y = 5;
        float currentTime = 0;

        while (currentTime < duration)
        {
            Debug.Log("asd");
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, currentTime/duration);
            yield return null;
        }

        _coroutine = null;
    }
}
