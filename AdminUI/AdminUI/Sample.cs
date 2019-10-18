using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
	public List<GameObject> objectsToSwitch;
	public void SetActiveExclusive(int id) {
		for(int i= 0; i< objectsToSwitch.Count; i++) {
			objectsToSwitch[i].SetActive(false);
		}
		objectsToSwitch[id].SetActive(true);
	}
}
