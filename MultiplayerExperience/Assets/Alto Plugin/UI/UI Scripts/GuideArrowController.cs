using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this script allows the player to see where the island is if they go too far out to sea
/// </summary>
public class GuideArrowController : MonoBehaviour {

    /*

    public GameObject island;
    private GameObject arrow;

    private void Start()
    {
        arrow = gameObject.transform.GetChild(0).gameObject;
        arrow.SetActive(false);
    }

    private void RotateArrow()
    {
        //rotate arrow to face direction of island
        Quaternion arrowRotation = Quaternion.LookRotation((island.transform.position - transform.position).normalized);
        transform.rotation = arrowRotation;
    }

    void Update ()
    {
        float distance = Vector3.Distance(transform.position, island.transform.position);
        if (distance >= 1250f)
        {
            if (!arrow.activeSelf) { arrow.SetActive(true); }
            RotateArrow();
        }
        else
        {
            if (arrow.activeSelf) { arrow.SetActive(false); }
        }
	}
    */
}
