using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tuto_Text_Debug2 : MonoBehaviour {
	
	public Text MonTuto;
	string phrase;
	float alpha;

	void Start () 
	{
		phrase = MonTuto.GetComponent<Text> ().text;
		alpha = MonTuto.GetComponent<Text> ().color.a;
	}
	

	void Update () 
	{
		
		MonTuto.GetComponent <Text> ().text = phrase;
		MonTuto.GetComponent <Text> ().color = 
			new Color (MonTuto.GetComponent <Text> ().color.r,
			MonTuto.GetComponent <Text> ().color.g,
			MonTuto.GetComponent <Text> ().color.g,
			alpha);
		
	}

	void OnTriggerEnter (Collider col)
	{
		
		if (col.tag == "BlocTuto")
		{
			//alpha = TutoText.color.a;
			//alpha = 0;
			//MonTuto.text = col.GetComponent<Text> ().text;
			phrase = col.GetComponent<Text>().text;
			//GetComponent<Text> ().text = MonTuto.text;
			StartCoroutine (FadeIn (MonTuto));

		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.tag == "BlocTuto")
		{
			alpha = 1;
			//TutoText.text = col.GetComponent<Text> ().text;
			//GetComponent<Text> ().text = TutoText.text;
			StartCoroutine (FadeOut (MonTuto));

		}
	}


	IEnumerator FadeIn (Text yolo)
	{
		alpha = 0;

		while (alpha < 1) 
		{
			alpha += 0.01f * Time.deltaTime;
		}
		yield return new WaitForSeconds (0);
	}


	IEnumerator FadeOut (Text yolo)
	{
		alpha = 1;

		while (alpha > 0.01f) 
		{
			alpha -= 0.01f * Time.deltaTime;
		}
		yield return new WaitForSeconds (0);
	}

}