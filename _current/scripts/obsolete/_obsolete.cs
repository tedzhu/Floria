using UnityEngine;
using System.Collections;

public class _obsolete : MonoBehaviour {
}



/*
 
 		
 		
 		
 		
	public IEnumerator animateSubtitle(string text,float unitDelay, float delay)
	{
		if(lowerleftInUse)
			yield return null;
		lowerleftInUse=true;		
		lowerleftResetTime=Time.time+delay;
		for(int i=0;i<=text.Length;i++){
			lowerleftString=text.Substring(0,i);

			yield return new WaitForSeconds(unitDelay);
		}
		lowerleftInUse=false;
	} 		
 		
 		
 		
 		
 		
 		
 		
 		
 		
 		
 		// enemy random appear
		if (isFiring) {
			if (Random.Range (1, 200) < 3) {
				int count = Random.Range (1, 2);
				for (int i=0; i<count; i++) {				
					Vector3 v = new Vector3 ();
					float margin = 2f;
					bool fromLeft=Random.Range (1, 3)==1;
					v.x = fromLeft? -margin : Screen.width + margin;
					v.y = Random.Range (Screen.height*0.2f, Screen.height);
					v.z = -Camera.main.transform.position.z;
					v = Camera.main.ScreenToWorldPoint (v);
				
					GameObject goEnemy=(GameObject)Instantiate (preEnemy, v, Quaternion.identity);
					Enemy enemy=goEnemy.GetComponent<Enemy>();					
					
					float girlMoveSpeed=g.girl.girlMoveSpeed;
					if(fromLeft){
						enemy.moveSpeed=girlMoveSpeed*Random.Range(1.5f,2f);						
					}else{
						enemy.moveSpeed=girlMoveSpeed*Random.Range(0.2f,0.4f);
					}
				}
			}
		} 
 
 
 
 
 
 
 	float upv=0;
	float upvStart=0.2f;
	float gravity=1;
	float jumpY;
 
 
 
 
 
 
 
 // keyboard
		if(keyboardControl){
			if(Input.GetKeyDown(KeyCode.W)){			
				if(grounded){
					grounded=false;
					upv=upvStart;
					jumpY=girlPos.y;
				}
			}
			
			if(Input.GetKeyDown(KeyCode.A)){
				if(move==IDLE){
					anim.Play("run-start");				
					currentAnim=anim.PlayQueued("run");
				}
				move+=MOVING;
				face=LEFT;			
			}		
			if(Input.GetKeyDown(KeyCode.D)){
				if(move==IDLE){
					anim.Play("run-start");				
					currentAnim=anim.PlayQueued("run");
				}
				move+=MOVING;
				face=RIGHT;
			}		
			
			if(Input.GetKeyUp(KeyCode.A)){
				move-=MOVING;
				if(move==IDLE){				
					anim.Play("run-end");
					anim.PlayQueued("idle").speed=2;
				}
			}		
			if(Input.GetKeyUp(KeyCode.D)){
				move-=MOVING;
				if(move==IDLE){				
					anim.Play("run-end");
					anim.PlayQueued("idle").speed=2;
				}
			}
			if(Input.GetKey(KeyCode.A)){			
				girlPos+=Vector3.left*girlMoveSpeed*Time.deltaTime;			
			}		
			if(Input.GetKey(KeyCode.D)){			
				girlPos+=Vector3.right*girlMoveSpeed*Time.deltaTime;			
			}
		}
 
 
 
 
 
 
 			GameObject [] goEnemies = GameObject.FindGameObjectsWithTag ("Enemy");		
			foreach (GameObject goEnemy in goEnemies) {
				Vector3 veb = transform.position - goEnemy.transform.position;
				if (veb.magnitude < bladeRange) {					
					// able to attack one
					
					// call two events
					goEnemy.GetComponent<Enemy> ().onHitBlade ();
					g.girl.onAttack ();					
					
					int acIndex;
					// cont attack
					if(Time.time-lastAttackTime<contAttackInterval){						
						if(contCount<contMax){							
							contCount++;						
						}else{
							g.cameraControl.shake (0.2f, 0.3f);	
						}
					}else{
						acIndex=0;
						contCount=0;
					}									
										
					
					StartCoroutine(delayPlay(contCount));
					
					if (isMoveGood) {
						psmw.SetRumble (moveId, 5);
						rumbleStopTime = Time.time + 0.2f;
					}
					
					//
					lastAttackTime=Time.time;
				}
			}
 
 
 
 
 	float lastAttackTime=0;
	float contAttackInterval=1f;
	int contCount;
	int contMax=6;
	
	
	
		public IEnumerator fadeCenter(string text,float unitDelay, float delay)
	{
		if(centerInUse)
			yield return null;
		centerInUse=true;
		int step=20;
		centerString=text;
		for(int i=0;i<=step;i++){
			clCenterIn.a=(float)i/step;
			clCenterOut.a=(float)i/step;
			yield return new WaitForSeconds(unitDelay);
		}
		yield return new WaitForSeconds(delay);
		for(int i=0;i<=step;i++){
			clCenterIn.a=1-(float)i/step;
			clCenterOut.a=1-(float)i/step;
			yield return new WaitForSeconds(unitDelay);
		}		
		centerInUse=false;
	}	
 */


						
//			dd=1;
//			int w=wct.width;
//			for(int i=0;i<wct.width;i++){
//				for(int j=0;j<wct.height;j++){
//					
//					for(int dx=-dd;dx<=dd;dx++){
//						for(int dy=-dd;dy<=dd;dy++){
//							int x=i+dx;
//							int y=j+dy;
//							if(x<0 || x>=wct.width)
//								continue;
//							if(y<0 || y>=wct.height)
//								continue;
//							cmatNew[y*w+i]+=cmat[j*w+i];
//						}
//					}					
//				}
//			}
			
//			for(int i=0;i<wct.width;i++){
//				for(int j=0;j<wct.height;j++){
//					texCamMask.SetPixel(i,j,cmat[j*wct.width+i]);
//				}
//			}