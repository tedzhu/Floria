using UnityEditor;
using UnityEngine;
using System.Collections;

public class myAudioImport : AssetPostprocessor 
{
	void OnPreprocessAudio()
	{
		AudioImporter importer = (AudioImporter)assetImporter;		

		if (assetPath.Contains("3d"))
			importer.threeD = true;
		else
			importer.threeD = false;
		
		if (assetPath.Contains("comp"))
			importer.format = AudioImporterFormat.Compressed;

		if (assetPath.Contains("loop"))
			importer.loopable = true;
	}
}
