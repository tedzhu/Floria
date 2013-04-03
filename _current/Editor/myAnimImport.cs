using UnityEditor;
using UnityEngine;
using System.Collections;

public class myAnimImport : AssetPostprocessor
{
	void OnPreprocessModel ()
	{
		ModelImporter modelImporter = assetImporter as ModelImporter;		
		Debug.Log (assetPath);
		if (assetPath.Contains ("Fairy")) {
			//modelImporter.globalScale = 1; 
			modelImporter.splitAnimations = true;
			modelImporter.generateAnimations = ModelImporterGenerateAnimations.InRoot;
			int numAnimations = 20;
			ModelImporterClipAnimation[] micas = new ModelImporterClipAnimation[numAnimations];
			micas [0] = SetClipAnimation ("idle", 0, 90, true);
			micas [1] = SetClipAnimation ("castAttack", 90, 158, false);
			micas [2] = SetClipAnimation ("wipeAttack", 158, 215, false);
			micas [3] = SetClipAnimation ("runStart", 215, 238, false);
			micas [4] = SetClipAnimation ("run", 238, 328, true);
			micas [5] = SetClipAnimation ("runEnd", 328, 355, false);
			micas [6] = SetClipAnimation ("jump", 355, 421, false);
			micas [7] = SetClipAnimation ("fall", 421, 455, false);
			micas [8] = SetClipAnimation ("swimStart", 455, 594, false);
			micas [9] = SetClipAnimation ("swim", 562, 594, true);
			micas [10] = SetClipAnimation ("swim-run", 594, 600, false);
			micas [11] = SetClipAnimation ("wipeChangeColor", 600, 650, false);
			micas [12] = SetClipAnimation ("disappoint", 650, 840, false);
			micas [13] = SetClipAnimation ("blind", 850, 889, false);
			micas [14] = SetClipAnimation ("blindFly", 889, 909, true);
			micas [15] = SetClipAnimation ("blind-run", 910, 930, false);
			
			micas [16] = SetClipAnimation ("enterDown", 455, 486, false);
			micas [17] = SetClipAnimation ("enterUp", 486, 522, false);
			micas [18] = SetClipAnimation ("enterFinish", 522, 594, false);
			micas [19] = SetClipAnimation ("die", 829, 830, true);
			
			//micas [19] = SetClipAnimation ("die", 829, 830, true);
						
			modelImporter.clipAnimations = micas;
		}
	}

	private ModelImporterClipAnimation SetClipAnimation (string name, int firstFrame, int lastFrame, bool loop)
	{
		ModelImporterClipAnimation mica = new ModelImporterClipAnimation ();
		mica.name = name;
		mica.firstFrame = firstFrame;
		mica.lastFrame = lastFrame;
		mica.loop = loop;
		if(loop)
			mica.wrapMode=WrapMode.Loop;		
		return mica;
	}	
}
