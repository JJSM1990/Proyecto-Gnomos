using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentoMaterial : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;   
    [SerializeField] private Material material;
    [SerializeField] private Material materialTransparent;
    [SerializeField] private GameObject player;
    private Color baseColor = Color.red;
    private Color transparentColor= new Color(0.4f, 0.9f, 0.7f, 0.5f);

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.ReturnPlayer();
    }
    private void Update()
    {
        if (player.transform.position.z>transform.position.z)
        {
            GetComponent<MeshRenderer>().material = materialTransparent;
        } else
        {
            GetComponent<MeshRenderer>().material = material;
        }
    }
}
