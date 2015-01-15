using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ChromaPackAtlasMaker : EditorWindow
{
	UIAtlas baseAtlas;
	Texture textureCP;
	UIAtlas newAtlas;
	string help;

	[MenuItem ("NGUI/Open ChromaPack Atlas Maker")]
	public static void OpenChromaPackAtlasMaker() {
		EditorWindow.GetWindow<ChromaPackAtlasMaker>(false, "ChromaPack Atlas Maker", true).Show();
	}
	
	void Execute()
	{
		if ( newAtlas == null ) {
			string base_atlas_path = AssetDatabase.GetAssetPath( baseAtlas );
			string new_atlas_path = base_atlas_path.Replace( ".prefab", "_CP.prefab" );
			AssetDatabase.CopyAsset( base_atlas_path, new_atlas_path );
			AssetDatabase.Refresh();
			newAtlas = AssetDatabase.LoadAssetAtPath( new_atlas_path, typeof(UIAtlas) ) as UIAtlas;

			string base_material_path = AssetDatabase.GetAssetPath( baseAtlas.spriteMaterial );
			string new_material_path = base_material_path.Replace( ".mat", "_CP.mat" );
			AssetDatabase.CopyAsset( base_material_path, new_material_path );
			AssetDatabase.Refresh();
			Material mat = AssetDatabase.LoadAssetAtPath( new_material_path, typeof(Material) ) as Material;

			mat.shader = Shader.Find( "ChromaPack/Cutout" );
			newAtlas.spriteMaterial = mat;
		}
		newAtlas.spriteMaterial.mainTexture = textureCP;

		float ratio_x = (float)textureCP.width / (float)baseAtlas.spriteMaterial.mainTexture.width;
		float ratio_y = (float)textureCP.height / (float)baseAtlas.spriteMaterial.mainTexture.height;
		List<UISpriteData> spriteList = new List<UISpriteData>();
		foreach ( UISpriteData sprite in baseAtlas.spriteList ) {
			UISpriteData new_sprite = new UISpriteData();
			new_sprite.CopyFrom( sprite );
			new_sprite.SetRect( (int)(sprite.x * ratio_x), (int)(sprite.y * ratio_y), (int)(sprite.width * ratio_x), (int)(sprite.height * ratio_y) );
			spriteList.Add( new_sprite );
		}
		newAtlas.spriteList = spriteList;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	void OnGUI ()
	{
		baseAtlas = EditorGUILayout.ObjectField( "Base Atlas", baseAtlas, typeof(UIAtlas), false ) as UIAtlas;
		textureCP = EditorGUILayout.ObjectField( "ChromaPack Texture", textureCP, typeof(Texture), false ) as Texture;
		newAtlas =  EditorGUILayout.ObjectField( "New Atlas", newAtlas, typeof(UIAtlas), false ) as UIAtlas;
		string button_text = newAtlas == null ? "Create" : "Update";
		if ( GUILayout.Button( button_text, GUILayout.Width( 100f ) ) ) {
			if ( baseAtlas == null ) {
				help = "Base Atlas is null";
			} else if ( textureCP == null ) {
				help = "ChromaPack Texture is null";
			} else {
				help = "";
				this.Execute();
			}
		}
		if ( !string.IsNullOrEmpty( help ) ) {
			EditorGUILayout.HelpBox( help, MessageType.Error );
		}
	}
}