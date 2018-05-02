using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CineticGunV2 : MonoBehaviour {

	#region sons

	[FMODUnity.EventRef]
	public string Gun_Max_Energie;

	FMOD.Studio.EventInstance Gun_Max_Energie_Event;

	[FMODUnity.EventRef]
	public string Gun_Min_Energie;

	FMOD.Studio.EventInstance Gun_Min_Energie_Event;


	[FMODUnity.EventRef]
	public string Gun_Lock;

	FMOD.Studio.EventInstance Gun_Lock_Event;

	[FMODUnity.EventRef]
	public string Gun_Unlock;

	FMOD.Studio.EventInstance Gun_Unlock_Event;

	[FMODUnity.EventRef]
	public string Gun_Absorbe_Energie;

	FMOD.Studio.EventInstance Gun_Absorbe_Energie_Event;

	[FMODUnity.EventRef]
	public string Gun_Don_Energie;

	FMOD.Studio.EventInstance Gun_Don_Energie_Event;

	[FMODUnity.EventRef]
	public string Gun_Absorbe_Direction;

	FMOD.Studio.EventInstance Gun_Absorbe_Direction_Event;

	[FMODUnity.EventRef]
	public string Gun_Don_Direction;

	FMOD.Studio.EventInstance Gun_Don_Direction_Event;

	#endregion

	#region trucs
	[SerializeField] GameObject ParticulesAspiration;
	[SerializeField] GameObject ParticulesDirection;

	[SerializeField] GameObject myGameObbject;

	[SerializeField] private FirstPersonController fpc;

	float myEnergie = 0;
	[SerializeField] float myEnergieMax = 200;
	bool stocked = false;
	LayerMask myMask;

	bool energiseGift = false;
	float energiseGiftTimer = 0.8f;
	bool energiseTake = false;
	float energiseTakeTimer = 0.8f;


	//Ma variable correspondant au nombre de palier à pour pousser un objet jusqu'à soi
	float step = 0;

	public BlockAlreadyMovingV2 FreezedBlock;


	/// /////// /////// /////// /////// /////// /////// /////// ////


	public BlockAlreadyMovingV2 blockLock;
	bool ObjectFreezed = false;
	public Vector3 blockLockDistanceBase = new Vector3(0,0,0);

	GameObject lastParticuleAspiration;
	GameObject lastParticuleAspirationGive;

	GameObject lastPlateformSeen = null;
	GameObject lastParticuleDirection;

	#endregion 

	[SerializeField] Image[] viseurObjects;
	[SerializeField] float[] viseurFill;
	float viseurMax = 0;

	float lastInputTrigger = 0;

	[SerializeField] float castSize = 0.5f;

	[SerializeField] GameObject TurnUi;


	void Start () {
		

		//myForces = new BlockMove.Force (new Vector3(1,0,0), new Vector3( transform.rotation.x, transform.rotation.y, transform.rotation.z) , 0.5f);
		myMask = 5;
		Gun_Absorbe_Energie_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Absorbe_Energie);
		Gun_Don_Energie_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Don_Energie);
		Gun_Absorbe_Direction_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Absorbe_Direction);
		Gun_Don_Direction_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Don_Direction);
		Gun_Lock_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Lock);
		Gun_Unlock_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Unlock);
		Gun_Max_Energie_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Max_Energie);
		Gun_Min_Energie_Event = FMODUnity.RuntimeManager.CreateInstance (Gun_Min_Energie);
	}

	void Update ()	
	{
		



		viseurObjects [0].fillAmount = viseurFill[0] + myEnergie / myEnergieMax*4;
		viseurObjects [1].fillAmount = viseurFill [1] + myEnergie / myEnergieMax*4 - 1;
		viseurObjects [2].fillAmount = viseurFill [2] + myEnergie / myEnergieMax*4 - 2;
		viseurObjects [3].fillAmount = viseurFill [3] + myEnergie / myEnergieMax*4 - 3;		


		//Ici du nouveau code pour indiquer comment gagner de l'énergie en appuyant sur une touche ou de façon continue 

		StartCoroutine (recharge (myEnergie));

		if (Input.GetKey(KeyCode.Y)) 
		{
			StartCoroutine (recharge (myEnergie));

		} 
		else if (Input.GetKeyUp (KeyCode.Y)) 
		{
			StopCoroutine(recharge (myEnergie));
		}
		///////////////////////////////////


		RaycastHit hita; 
		if (Physics.SphereCast (transform.position,castSize,Camera.main.transform.TransformDirection (Vector3.forward), out hita, Mathf.Infinity, myMask) && hita.collider.GetComponent<BlockAlreadyMovingV2> () && lastPlateformSeen != hita.collider.gameObject && hita.collider.GetComponent<BlockAlreadyMovingV2> ().direction != new Vector3(0,0,0)) { 
			if(lastParticuleDirection!= null){
				lastParticuleDirection.GetComponent<ParticleSystem>().Stop();
				Destroy(lastParticuleDirection,6);
			}
			lastPlateformSeen = hita.collider.gameObject;
			lastParticuleDirection = Instantiate<GameObject>(ParticulesDirection);
			lastParticuleDirection.transform.position = lastPlateformSeen.transform.position;
			lastParticuleDirection.transform.LookAt (lastParticuleDirection.transform.position+hita.collider.GetComponent<BlockAlreadyMovingV2>().direction);
			lastParticuleDirection.transform.parent = lastPlateformSeen.transform;
		}else if ( !Physics.SphereCast (transform.position,castSize, Camera.main.transform.TransformDirection (Vector3.forward), out hita, Mathf.Infinity, myMask )){
			lastPlateformSeen = null;
			if(lastParticuleDirection != null){
				lastParticuleDirection.GetComponent<ParticleSystem>().Stop();
				Destroy(lastParticuleDirection,4);
			}
		}else{
			lastPlateformSeen = hita.collider.gameObject;
		}

		#region direction
		//donner une force	
		float triger2 = Input.GetAxis ("trigger2");

		if (( triger2 > 0.2f  || Input.GetMouseButtonDown (0)) && !stocked) {
			stocked = true;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, Camera.main.transform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, myMask) &&  hit.collider.GetComponent<BlockAlreadyMovingV2>()  ) {
				BlockAlreadyMovingV2 myBlock = hit.collider.GetComponent<BlockAlreadyMovingV2>();
				Gun_Don_Direction_Event.start();
				myBlock.direction = Camera.main.transform.forward; 
				Debug.Log(lastParticuleDirection.name);


				//Ici je rajoute le code qui fait ralentir l'objet une fois qu'il est en mouvement


				StartCoroutine (Ralentissement(myBlock));




				if(hit.collider.GetComponent<BlockAlreadyMovingV2>().move){
					lastParticuleDirection.transform.LookAt(myBlock.gameObject.transform.position+myBlock.direction);	
				}				
				myBlock.ApplyTheVelocity();
			}
		} else 
		{
			stocked = false;

		}

		//inverse
		if(Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetMouseButtonDown(1))
		{
			RaycastHit hit;
			if (Physics.Raycast (transform.position, Camera.main.transform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, myMask) &&  hit.collider.GetComponent<BlockAlreadyMovingV2>()  ) {
				BlockAlreadyMovingV2 myBlock = hit.collider.GetComponent<BlockAlreadyMovingV2>();
				Gun_Don_Direction_Event.start();


				//myBlock.direction = -Camera.main.transform.forward ;

				//C'est ici que je vais effectuer une frappe chirurgicale dans le but d'attier les objets à nous.

				StartCoroutine (StepMovement(myBlock));

				////////////////////////////:

				if(hit.collider.GetComponent<BlockAlreadyMovingV2>().move)
				{
					lastParticuleDirection.transform.LookAt(myBlock.gameObject.transform.position+myBlock.direction);	
				}					
				myBlock.ApplyTheVelocity();
			}
		}
		#endregion


		#region Energise
		//take
		bool isTackingEnergie = false;
		if ( Input.GetKey (KeyCode.Joystick1Button4)|| Input.GetKey (KeyCode.A) ) {
			RaycastHit energiseHit;
			if (Physics.SphereCast (transform.position, castSize, Camera.main.transform.TransformDirection (Vector3.forward), out energiseHit, Mathf.Infinity, myMask) && energiseHit.collider.GetComponent<BlockAlreadyMovingV2> ()  ) {
				BlockAlreadyMovingV2 myBlock = energiseHit.collider.GetComponent<BlockAlreadyMovingV2> ();
				if(myEnergie < myEnergieMax)
				{
					if(myBlock.energie>0)
					{							
						if (energiseTake && ( Input.GetKeyDown (KeyCode.Joystick1Button4) || Input.GetKeyDown (KeyCode.A) )) 
						{
							float myAddedEnergie = myEnergieMax - myEnergie;
							if(myAddedEnergie <= myBlock.energie){
								myEnergie += myAddedEnergie;
								myBlock.energie -= myAddedEnergie;
							}
							else
							{
								myEnergie += myBlock.energie;
								myBlock.energie = 0;
							}
							lastParticuleAspiration.GetComponent<ParticleSystem>().Emit((int)myEnergie/3);
							lastParticuleAspiration.GetComponent<ParticleSystem>().Stop();
							Destroy(lastParticuleAspiration.gameObject,2f);
							isTackingEnergie = false;
						} 
						else 
						{
							if(Input.GetKeyDown (KeyCode.Joystick1Button4)|| Input.GetKeyDown (KeyCode.A) || lastPlateformSeen != energiseHit.collider.gameObject){
								Gun_Absorbe_Energie_Event.start();
								lastParticuleAspiration = Instantiate<GameObject>(ParticulesAspiration);
								lastParticuleAspiration.GetComponent<particleAttractorLinear>().target = this.transform;
								lastParticuleAspiration.transform.parent = energiseHit.collider.transform;
								lastParticuleAspiration.transform.position = energiseHit.collider.transform.position;
								Destroy(lastParticuleAspiration.gameObject,8);
								energiseTake = true;
								energiseTakeTimer = 0.8f;
							}
							isTackingEnergie = true;
							myBlock.energie -= 3;
							myEnergie += 3;
						}

						if (myBlock.energie < 0f) {
							myBlock.energie = 0f;
						}
						myBlock.ApplyTheVelocity();		
					}
				}else{
					if(Input.GetKeyDown (KeyCode.Joystick1Button4) || Input.GetKeyDown (KeyCode.A)){
						Gun_Min_Energie_Event.start();
					}
				}
				myBlock.ApplyTheVelocity();
			}else if (energiseHit.collider != null && energiseHit.transform.tag == "destructible"){
				if(myEnergie < myEnergieMax){
					if(energiseHit.rigidbody.velocity.magnitude >0){

						if (energiseTake && ( Input.GetKeyDown (KeyCode.Joystick1Button4) || Input.GetKeyDown (KeyCode.A) )) {
							float myAddedEnergie = myEnergieMax - myEnergie;
							if(myAddedEnergie <= energiseHit.rigidbody.velocity.magnitude){
								myEnergie += myAddedEnergie;
								energiseHit.rigidbody.velocity = new Vector3(0,0,0);
							}else{
								myEnergie += energiseHit.rigidbody.velocity.magnitude;
								Debug.Log(energiseHit.rigidbody.velocity.magnitude);
								energiseHit.rigidbody.velocity = new Vector3(0,0,0);
							}
							energiseHit.rigidbody.angularVelocity = new Vector3(0,0,0);
							lastParticuleAspiration.GetComponent<ParticleSystem>().Emit((int)myEnergie/3);
							lastParticuleAspiration.GetComponent<ParticleSystem>().Stop();
							Destroy(lastParticuleAspiration.gameObject,2f);
							isTackingEnergie = false;
						} else {
							if(Input.GetKeyDown (KeyCode.Joystick1Button4)|| Input.GetKeyDown (KeyCode.A) || lastPlateformSeen != energiseHit.collider.gameObject){
								Gun_Absorbe_Energie_Event.start();
								lastParticuleAspiration = Instantiate<GameObject>(ParticulesAspiration);
								lastParticuleAspiration.GetComponent<particleAttractorLinear>().target = this.transform;
								lastParticuleAspiration.transform.parent = energiseHit.collider.transform;
								lastParticuleAspiration.transform.position = energiseHit.collider.transform.position;
								Destroy(lastParticuleAspiration.gameObject,8);
								energiseTake = true;
								energiseTakeTimer = 0.8f;
							}
							isTackingEnergie = true;
							energiseHit.rigidbody.velocity -= new Vector3(1,1,1) ;
							energiseHit.rigidbody.angularVelocity -= new Vector3(1,1,1);
							if(energiseHit.rigidbody.velocity.x < 0){
								energiseHit.rigidbody.velocity = new Vector3 (0, energiseHit.rigidbody.velocity.y, energiseHit.rigidbody.velocity.z);
							}if(energiseHit.rigidbody.velocity.y < 0){
								energiseHit.rigidbody.velocity = new Vector3 (energiseHit.rigidbody.velocity.x, 0, energiseHit.rigidbody.velocity.z);
							}if(energiseHit.rigidbody.velocity.z < 0){
								energiseHit.rigidbody.velocity = new Vector3 (energiseHit.rigidbody.velocity.z, energiseHit.rigidbody.velocity.y, 0);
							}if(energiseHit.rigidbody.angularVelocity.x < 0){
								energiseHit.rigidbody.angularVelocity = new Vector3 (0, energiseHit.rigidbody.angularVelocity.y, energiseHit.rigidbody.angularVelocity.z);
							}if(energiseHit.rigidbody.angularVelocity.y < 0){
								energiseHit.rigidbody.angularVelocity = new Vector3 (energiseHit.rigidbody.angularVelocity.x, 0, energiseHit.rigidbody.angularVelocity.z);
							}if(energiseHit.rigidbody.angularVelocity.z < 0){
								energiseHit.rigidbody.angularVelocity = new Vector3 (energiseHit.rigidbody.angularVelocity.z, energiseHit.rigidbody.angularVelocity.y, 0);
							}
							myEnergie += 3;
						}				
					}
				}else{
					if(Input.GetKeyDown (KeyCode.Joystick1Button4) || Input.GetKeyDown (KeyCode.A)){
						Gun_Min_Energie_Event.start();
					}
				}
			}
		}
		if(energiseTakeTimer > 0){
			energiseTakeTimer -=Time.deltaTime;
		}else{
			energiseTake = false;
		}
		if(isTackingEnergie == false && lastParticuleAspiration != null){
			lastParticuleAspiration.GetComponent<ParticleSystem>().Stop();
			Destroy(lastParticuleAspiration.gameObject,10);

		}


		//give
		float triger1 = Input.GetAxis ("trigger1");
		bool isTackingEnergieGive = false;
		if ( triger1 > 0.2f  || Input.GetKey (KeyCode.E)) {
			RaycastHit hit;
			if (Physics.SphereCast (transform.position,castSize, Camera.main.transform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, myMask) && hit.collider.GetComponent<BlockAlreadyMovingV2> ()) {
				BlockAlreadyMovingV2 myBlock = hit.collider.GetComponent<BlockAlreadyMovingV2> ();
				if(myEnergie>=3){					
					Debug.Log(triger1);
					Debug.Log(lastInputTrigger);
					if (energiseGift &&  (lastInputTrigger <= 0.09f  || Input.GetKeyDown (KeyCode.E))) {
						lastParticuleAspirationGive.GetComponent<ParticleSystem>().Emit((int)myEnergie/1);
						lastParticuleAspirationGive.GetComponent<ParticleSystem>().Stop();
						Destroy(lastParticuleAspirationGive.gameObject,2.5f);
						isTackingEnergieGive = false;
						myBlock.energie += myEnergie;
						myEnergie = 0;
					} else {
						if (lastInputTrigger <= 0.09f  || Input.GetKeyDown (KeyCode.E) || lastPlateformSeen != hit.collider.gameObject){
							Gun_Don_Energie_Event.start();
							lastParticuleAspirationGive = Instantiate<GameObject>(ParticulesAspiration);
							lastParticuleAspirationGive.GetComponent<particleAttractorLinear>().target = hit.collider.transform;
							lastParticuleAspirationGive.GetComponent<particleAttractorLinear>().speed = 5;
							lastParticuleAspirationGive.transform.parent = this.transform;
							lastParticuleAspirationGive.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y -3f, this.transform.position.z);
							Destroy(lastParticuleAspirationGive.gameObject,5);
							energiseGift = true;
							energiseGiftTimer = 0.8f;
						}
						myBlock.energie += 3;
						myEnergie -= 3;
						lastParticuleAspirationGive.GetComponent<ParticleSystem>().Emit((int)myEnergie/3);
					}

					if (myBlock.energie < 0f) {
						myBlock.energie = 0f;
					}
					myBlock.ApplyTheVelocity();
				}else{
					if(lastInputTrigger <= 0.09f  || Input.GetKeyDown (KeyCode.E)){
						Gun_Min_Energie_Event.start();
					}
				}
				myBlock.ApplyTheVelocity();
			}
		}
		if(energiseGiftTimer > 0){
			energiseGiftTimer -=Time.deltaTime;
		}else{
			energiseGift = false;
		}
		if(isTackingEnergieGive == false && lastParticuleAspirationGive != null){
			lastParticuleAspirationGive.GetComponent<ParticleSystem>().Stop();
			Destroy(lastParticuleAspiration.gameObject,10);

		}
		lastInputTrigger = triger1;


		#endregion

		#region Lock
		// Système de lock

		//bool OneObjectFreeze = false;

		//Gun_Lock_Event.start ();
		//FMODUnity.RuntimeManager.PlayOneShot ("Gun_Lock_Event", gameObject.transform.position);


		//Ici, le but est de mettre un objet en enfant du RigidbodyPrefab

		if (Input.GetMouseButton (2) || Input.GetKey(KeyCode.JoystickButton9) ) 
		{
			RaycastHit hit; 
			if (Physics.SphereCast (transform.position,castSize, 
				Camera.main.transform.TransformDirection (Vector3.forward), out hit, 
				Mathf.Infinity, myMask) && hit.collider.GetComponent<BlockAlreadyMovingV2> () 
				&& !ObjectFreezed )
			{

				ObjectFreezed = true;
				if (ObjectFreezed)
				{
				Debug.Log ("récupère moi qu'une fois");
			
				FreezedBlock = hit.collider.GetComponent<BlockAlreadyMovingV2> ();
				FreezedBlock.GetComponent<BlockAlreadyMovingV2>().enabled = false;
				FreezedBlock.rb.velocity = new Vector3 (0,0,0);
				FreezedBlock.energie = 0;
				//FreezedBlock = null;
								
				FreezedBlock = hit.collider.GetComponent<BlockAlreadyMovingV2> ();
				StartCoroutine (Enfant (FreezedBlock));

					// Les deux coroutines sont à la fin du code

				}
					//OneObjectFreeze = true;
			}
		}
		if (Input.GetMouseButtonUp (2) || Input.GetKeyUp (KeyCode.JoystickButton9) ) 
		{				
				StartCoroutine (StopEnfant (FreezedBlock));
				FreezedBlock.GetComponent<BlockAlreadyMovingV2>().enabled = true;
				FreezedBlock.rb.velocity = new Vector3 (0,0,0);
				
				//OneObjectFreeze = false;
				ObjectFreezed = false;
			Debug.Log ("tu peux me récupérer");
		}

		#endregion
	}

	/*
	void OnCollisionEnter()
	{
		delock ();
	}

	void delock(){
		if (blockLock != null) {
			Gun_Unlock_Event.start();
			StartCoroutine("turnRightUi");
			blockLock = null;
		}
	}

	IEnumerator turnLeftUi(){
		for (int i = 0; i < 8; i++) {
			TurnUi.transform.Rotate (0, 0, 10 + i*2);
			yield return new WaitForEndOfFrame ();
		}
		yield return null;
	}

	IEnumerator turnRightUi(){
		for (int i = 0; i < 8; i++) {
			TurnUi.transform.Rotate (0, 0, -10 - i*2);
			yield return new WaitForEndOfFrame ();
		}
		yield return null;
	}*/



	IEnumerator StepMovement (BlockAlreadyMovingV2 CeBloc)
	{
		Vector3 ActualPosition = transform.position;
		step = 0;

		while (Vector3.Distance(CeBloc.transform.position, ActualPosition) > (3f + CeBloc.transform.localScale.x))
		{
			step += Time.deltaTime;
			CeBloc.transform.position = Vector3.MoveTowards (CeBloc.transform.position, transform.position, step);
			yield return null;
		}

		if (CeBloc.energie <= (3f + CeBloc.transform.localScale.x))
			{
				CeBloc.ApplyTheVelocity ();
				CeBloc.energie = 0;
			}


	}

	IEnumerator recharge (float bob)
	{
		float speed = 1;
		bob += Time.deltaTime * speed;
		bool Rechargement = true;
			
		if (Rechargement) 
		{
			myEnergie = bob;
		}
			
		if (bob >= myEnergieMax)
		{
			Rechargement = false;
			bob = 0;

		}
		yield return new WaitForSeconds (0.1f);
	}


	IEnumerator Ralentissement (BlockAlreadyMovingV2 CeBloc)
	{
		float speed = 200;
		CeBloc.energie -= Time.deltaTime * speed;
		//CeBloc.ApplyTheVelocity ();
		if (CeBloc.energie == 0) {
			CeBloc.NoVelocity ();
		}
		yield return new WaitForSeconds (0.1f);
	}

	IEnumerator Enfant (BlockAlreadyMovingV2 CeBloc)
	{
		CeBloc.transform.parent = this.transform;
		yield return new WaitForSeconds (0.1f);

	}
	IEnumerator StopEnfant (BlockAlreadyMovingV2 CeBloc)
	{
		Debug.Log ("Pourtant je rentre bien là");
		CeBloc.transform.parent = null;
		yield return new WaitForSeconds (0.1f);
	}


}