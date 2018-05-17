using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityStandardAssets.CrossPlatformInput;
//using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;


	[RequireComponent(typeof (Rigidbody))]
    public class FirstPersonController : MonoBehaviour
    {
	[FMODUnity.EventRef]
	public string jump;

	FMOD.Studio.EventInstance jump_Event;

	[FMODUnity.EventRef]
	public string jump_Down;

	FMOD.Studio.EventInstance jump_Down_Event;

	[SerializeField] private float speed;
	
    public MouseLook m_MouseLook;
		
	[SerializeField] float jumpSpeed = 120f;
	private Camera m_Camera;
	private bool m_Jump = false;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
	private Rigidbody rb;
	bool collided = false;

	public bool grounded = false;
	[SerializeField] CineticGunV2 myGun;
	[SerializeField] Vector3 newVelocity;

	[SerializeField] Image myImage;
	[SerializeField] Image tittle;
	[SerializeField] float timerChangeScene;
	[SerializeField] float timer;
	[SerializeField] string sceneName;

	bool isChangingScene = false;

	public static Vector3 positionSpawn; //= new Vector3(367f,141f,-572.1f);
	public Vector3 basePosition = new Vector3(367f,141f,-572.1f);


	IEnumerator ChangeSceneEnd(){
		music_Event.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		jingle2_Event.start ();
		for (int i = 0; i < timer; i++) {
			myImage.color = new Color(myImage.color.r,myImage.color.g,myImage.color.b, myImage.color.a+ 1 / timer);
			yield return new WaitForEndOfFrame ();

		}

		for (int i = 0; i < timer/2; i++) {
			tittle.color = new Color(tittle.color.r,tittle.color.g,tittle.color.b, tittle.color.a+ 1 / (timer/2));
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitForSeconds(timerChangeScene);

		for (int i = 0; i < timer/2; i++) {
			tittle.color = new Color(tittle.color.r,tittle.color.g,tittle.color.b, tittle.color.a- 1 / (timer/2));
			yield return new WaitForEndOfFrame ();
		}

		SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);
		yield return null;
	}
	IEnumerator justChangeScene(){
		for (int i = 0; i < timer; i++) {
			timer+=Time.deltaTime;
			myImage.color = new Color(myImage.color.r,myImage.color.g,myImage.color.b, myImage.color.a- 1 / timer);
			yield return new WaitForEndOfFrame ();
		}
		music_Event.start ();
		yield return null;
	}
	IEnumerator ChangeSceneReload(){
		for (float i = 0; i < 1; ) {
			i+=Time.deltaTime;
			myImage.color = new Color(myImage.color.r,myImage.color.g,myImage.color.b, i);
			yield return new WaitForEndOfFrame ();
		}
		music_Event.start ();
		SceneManager.LoadSceneAsync (SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		yield return null;
	}

	bool dead = false;

	[FMODUnity.EventRef]
	public string jingle;

	FMOD.Studio.EventInstance jingle_Event;

	[FMODUnity.EventRef]
	public string jingle2;

	FMOD.Studio.EventInstance jingle2_Event;

	[FMODUnity.EventRef]
	public string music;

	FMOD.Studio.EventInstance music_Event;


        // Use this for initialization
        private void Start(){

		jingle_Event = FMODUnity.RuntimeManager.CreateInstance (jingle);
		jingle2_Event = FMODUnity.RuntimeManager.CreateInstance (jingle2);
		music_Event = FMODUnity.RuntimeManager.CreateInstance (music);
		jingle_Event.start ();


		StartCoroutine ("justChangeScene");
		jump_Event = FMODUnity.RuntimeManager.CreateInstance (jump);
		jump_Down_Event = FMODUnity.RuntimeManager.CreateInstance (jump_Down);
		
			rb = GetComponent<Rigidbody> ();

            m_Camera = Camera.main;
			m_MouseLook.Init(transform , m_Camera.transform);
			//transform.position = positionSpawn; //pour spawn sur checkpoint
        }



        private void Update(){


            RotateView();

			GetInput();

			Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;


			m_MoveDir.x = desiredMove.x*speed*Time.deltaTime*85;
			m_MoveDir.z = desiredMove.z*speed*Time.deltaTime*85;
		if ((!grounded /*&& !myGun.isLock */) ) {
			rb.useGravity = true;
			//m_MoveDir.y = -Time.deltaTime * 4;
		} else  {
			//
			rb.useGravity = false;
			grounded = false;
		}
		if (myGun.blockLock != null) {
			m_MoveDir = new Vector3 (0, 0, 0);
		}

		if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space)) && !m_Jump ){
			jump_Event.start ();
			m_Jump = true;
			transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
			rb.AddForce(0,jumpSpeed,0,ForceMode.Impulse);
		}
		if(myGun.blockLock != null){
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			transform.position = myGun.blockLock.transform.position - myGun.blockLockDistanceBase;
		}else{
			rb.useGravity = true;
			rb.velocity = new Vector3(m_MoveDir.x+newVelocity.x, rb.velocity.y+newVelocity.y/100, m_MoveDir.z+newVelocity.z );
		}
/*
		if(!dead){
			Collider[] myObjectCollision = Physics.OverlapCapsule(transform.position+new Vector3(0,1f,0),transform.position+new Vector3(0,-0.5f,0), 0.5f) ;
			Vector3[] myObjectPosition = new Vector3[myObjectCollision.Length]; 
			for( int i = 0; i < myObjectCollision.Length; i ++){
				myObjectPosition [i] = myObjectCollision[i].transform.position - transform.position;
			}

			for (  int i = 0; i < myObjectCollision.Length; i ++){
				if (!myObjectCollision[i].isTrigger){
					if(myObjectPosition[i].x < 0 ){
						for (  int y = i+1; y < myObjectCollision.Length; y ++){
							if(myObjectPosition[y].x > 0 && !myObjectCollision[y].isTrigger){
								StartCoroutine("ChangeSceneReload");
								dead = true;
							}
						}
					}

					if(myObjectPosition[i].z < 0 ){
						for (  int y = i+1; y < myObjectCollision.Length; y ++){
							if(myObjectPosition[y].z > 0 && !myObjectCollision[y].isTrigger){
								StartCoroutine("ChangeSceneReload");
								dead = true;
							}
						}
					}

					if(myObjectPosition[i].y > 0 ){
						for (  int y = i+1; y < myObjectCollision.Length; y ++){
							if(myObjectPosition[y].y < 0 && !myObjectCollision[y].isTrigger){
								StartCoroutine("ChangeSceneReload");
								dead = true;
							}
						}
					}
				}
			}
		}
		Debug.Log(dead);
*/
	}
        private void FixedUpdate(){
		     m_MouseLook.UpdateCursorLock();
        }

	//je l'ai passé en public pour la choper dans CineticGunV2

	public void GetInput(){
         // Read input
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
        // set the desired speed to be walking or running
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1){
            m_Input.Normalize();
        }           
    }
    private void RotateView(){
        m_MouseLook.LookRotation (transform, m_Camera.transform);
    }

	void OnCollisionEnter(Collision col){
		if (col.contacts [0].normal.y > -0.1f) {
			if (!collided) {
				jump_Down_Event.start ();
				collided = true;
			}



		}
	}


	void OnCollisionExit (Collision col)
	{
		if (col.gameObject.tag == "wall") 
		{
			m_Jump = false;
		}
	}

	void OnTriggerEnter(Collider col){
		Debug.Log(0);
		if (!isChangingScene && col.tag == "ChangeSceneEnd") {
			jingle2_Event.start ();  
			isChangingScene = true;
			StartCoroutine ("ChangeSceneEnd");
		}
		if (!isChangingScene && col.tag == "ChangeScene") {
			jingle2_Event.start ();  
			isChangingScene = true;
			StartCoroutine ("ChangeScene");
		}
		if (col.tag == "death"){
			Debug.Log(1);
			StartCoroutine ("ChangeSceneReload");
		}
		if (col.tag == "checkpoint"){
			positionSpawn = col.transform.position;
		}
	}

	void OnCollisionStay(Collision col){

		if (col.contacts [0].normal.y > -0.1f) {
			m_Jump = false;
			grounded = true;
		}



		if (col.gameObject.tag == "wall") 
		{
			m_Jump = true;
		}

		if (col.gameObject.GetComponent<BlockAlreadyMovingV2> ()) {
			newVelocity = Vector3.Normalize( col.gameObject.GetComponent<BlockAlreadyMovingV2> ().direction)/1.5f * Time.deltaTime * col.gameObject.GetComponent<BlockAlreadyMovingV2> ().energie;
		} else {
			newVelocity = new Vector3 (0, 0, 0);
		}
	}

	void OnCollisionExit(){
		collided = false;
		newVelocity = new Vector3 (0, 0, 0);
		grounded = false;
	}
}