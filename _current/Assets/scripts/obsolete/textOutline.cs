using UnityEngine;
using System.Collections;

public class textOutline : MonoBehaviour {
	public Material outlineMat;
	// Use this for initialization
	void outline(float dx,float dy)
	{
		GameObject go=(GameObject)Instantiate(this.gameObject,transform.position+new Vector3(dx,dy,0.1f),transform.rotation);		
		go.name="_titleOutline";
		go.renderer.material=outlineMat;
	}
	
	void Start () {
		if(name=="_title"){
			float d=0.1f;
			outline (d,d);
			outline (-d,d);
			outline (d,-d);
			outline (-d,-d);			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
