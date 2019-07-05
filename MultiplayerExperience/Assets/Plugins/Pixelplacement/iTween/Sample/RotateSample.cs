using UnityEngine;
using System.Collections;

public class RotateSample : MonoBehaviour
{	
	void Start(){
		iTween.RotateTo(gameObject, iTween.Hash("x", 90, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
	}
}

