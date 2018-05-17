using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockAlreadyMovingV2 : MonoBehaviour {

public bool move = true;

public float maxEnergie = 200;

public float energie = 1000;
public Vector3 direction = new Vector3(0,0,0);

public Rigidbody rb;

public Material[] myMat;

public 	float _BoundsUp;
public	float _BoundsDown;

void Start(){

	direction = Vector3.Normalize(direction);
	rb = GetComponent<Rigidbody>();
	myMat = GetComponent<MeshRenderer> ().materials;

	ApplyTheVelocity();
}

public void ApplyTheVelocity(){
	if (move){
		if (energie < 0f) {
			energie = 0f;
		}
		float energieNew = energie / (maxEnergie*8);
		//energieNew= Mathf.Log10 (energieNew)*3f;
		if (energieNew < 0) {
			energieNew = 0;
		}
		for (int i = 0; i < myMat.Length; i++) {			
			myMat[i].SetFloat("_MKGlowTexStrength", energieNew);
		}	
		direction = Vector3.Normalize(direction);
		Vector3 newVelocity = direction * Time.deltaTime * energie;
		rb.velocity = newVelocity;
	}
		else
	{
		direction = Vector3.zero;
	}
}

	public void NoVelocity ()
	{
		if (energie > 0f) 
		{
			energie = 0f;
		}
	}


	void OnCollisionEnter(Collision col){		
			if (!col.gameObject.GetComponent<CineticGunV2> () && col.gameObject.tag != "destructible") {
				if(col.contacts[0].point.y-transform.position.y<-0.1f){
					direction = new Vector3(direction.x,-direction.y,direction.z);
				}else{
					direction = col.contacts [0].normal.normalized;
				}
				ApplyTheVelocity();
			}
			if (col.gameObject.tag == "destructible" || col.gameObject.GetComponent<BlockAlreadyMovingV2>()){
				if (energie > maxEnergie / 2f) {
					if(!col.gameObject.GetComponent<Rigidbody> ()){
						col.gameObject.AddComponent<Rigidbody>();
					}
					//col.gameObject.GetComponent<Rigidbody> ().mass = 10f;
					col.gameObject.GetComponent<Rigidbody> ().velocity = rb.velocity;
					if(col.gameObject.GetComponent<ScriptObjDestructible>()){
						col.gameObject.GetComponent<ScriptObjDestructible>().enabled = true;
					}
				} else {					
					direction = col.contacts [0].normal.normalized;
				}
			}
		
		ApplyTheVelocity();
	}

	void OnCollisionExit(Collision col){
		if (col.gameObject.tag == "destructible"){
			col.gameObject.GetComponent<Rigidbody> ().mass = 1;   //avant == 100000
		}	
	}

}