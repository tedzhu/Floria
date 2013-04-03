#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;

public class PSAttractor : MonoBehaviour {
	GameObject go0;
	Global g;
	
	Particle[] p;
	ParticleEmitter pe;
	public float attractionRate=2f;
	public float strongAttractionRange=2f;
	public float strongAttractionRate=4f;
	void Start () {
		go0 = GameObject.Find ("0");
		g = go0.GetComponent<Global> ();		
		
		pe=GetComponentInChildren<ParticleEmitter>();
		
	}
	
	// Update is called once per frame
	void Update () {
		p=pe.particles;		
		for(int i=0;i<pe.particles.Length;i++){
			Vector3 vpt=transform.position-pe.particles[i].position;
			float d;
			if(vpt.magnitude<strongAttractionRange){
				d=Time.deltaTime*strongAttractionRate;
			}else{
				d=Time.deltaTime*attractionRate;
			}
			p[i].position=Vector3.Lerp(pe.particles[i].position,transform.position,d);
		}
		pe.particles=p;
	}
}
