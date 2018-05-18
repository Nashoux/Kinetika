using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDestruction : MonoBehaviour {

	[FMODUnity.EventRef]
	public string destructionSound;

	FMOD.Studio.EventInstance destructionSound_event;

	float OneObjectDown = 0;

	void Start ()
	{
		destructionSound_event = FMODUnity.RuntimeManager.CreateInstance (destructionSound);
		//destructionSound_event.start ();
		
	}
	
	// Update is called once per frame
	/*void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "objetMobile") 
		{
			destructionSound_event.setParameterValue ("CoefDestruction", 1);
					
		}
	}*/

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag == "objetMobile") 
		{
			Debug.Log ("Yolooooooo");
			FMODUnity.RuntimeManager.PlayOneShot ("event:/DestructionEtrange", gameObject.transform.position);
			//destructionSound_event.start ();
			/*
			OneObjectDown += 0.05f;
			destructionSound_event.setParameterValue ("CoefDestruction", OneObjectDown);
*/
			//destructionSound_event.GetPa ("CoefDestruction", 1);
		}
	}
}
