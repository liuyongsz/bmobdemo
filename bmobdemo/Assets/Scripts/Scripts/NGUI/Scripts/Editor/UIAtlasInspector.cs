//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Inspector class used to edit the UIAtlas.
/// </summary>

[CustomEditor(typeof(UIAtlas))]
public class UIAtlasInspector : Editor
{
	static public UIAtlasInspector instance;

	enum AtlasType
	{
		Normal,
		Reference,
	}

	UIAtlas mAtlas;
	AtlasType mType = AtlasType.Normal;
	UIAtlas mReplacement = null;

	void OnEnable () { instance = this; }
	void OnDisable () { instance = null; }

	/// <summary>
	/// Convenience function -- mark all widgets using the sprite as changed.
	/// </summary>

	void MarkSpriteAsDirty ()
	{
		UISpriteData sprite = (mAtlas != null) ? mAtlas.GetSprite(NGUISettings.selectedSprite) : null;
		if (sprite == null) return;

		UISprite[] sprites = NGUITools.FindActive<UISprite>();

		foreach (UISprite sp in sprites)
		{
			if (UIAtlas.CheckIfRelated(sp.atlas, mAtlas) && sp.spriteName == sprite.name)
			{
				UIAtlas atl = sp.atlas;
				sp.atlas = null;
				sp.atlas = atl;
				EditorUtility.SetDirty(sp);
			}
		}

		UILabel[] labels = NGUITools.FindActive<UILabel>();

		foreach (UILabel lbl in labels)
		{
			if (lbl.bitmapFont != null && UIAtlas.CheckIfRelated(lbl.bitmapFont.atlas, mAtlas) && lbl.bitmapFont.UsesSprite(sprite.name))
			{
				UIFont font = lbl.bitmapFont;
				lbl.bitmapFont = null;
				lbl.bitmapFont = font;
				EditorUtility.SetDirty(lbl);
			}
		}
	}

	/// <summary>
	/// Replacement atlas selection callback.
	/// </summary>

	void OnSelectAtlas (Object obj)
	{
		if (mReplacement != obj)
		{
			// Undo doesn't work correctly in this case... so I won't bother.
			//NGUIEditorTools.RegisterUndo("Atlas Change");
			//NGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);

			mAtlas.replacement = obj as UIAtlas;
			mReplacement = mAtlas.replacement;
			NGUITools.SetDirty(mAtlas);
			if (mReplacement == null) mType = AtlasType.Normal;
		}
	}

    public static uint LittleEndianToBigEndian(uint value)
    {
        byte[] bytes = System.BitConverter.GetBytes(value);
        return System.BitConverter.ToUInt32(ReverseOrder(bytes), 0);
    }

    public static byte[] ReverseOrder(byte[] dt)
    {
        byte[] buffer = new byte[dt.Length];
        int num = 0;
        for (int i = dt.Length - 1; i >= 0; i--)
        {
            buffer[num++] = dt[i];
        }
        return buffer;
    }

	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);
		mAtlas = target as UIAtlas;

		UISpriteData sprite = (mAtlas != null) ? mAtlas.GetSprite(NGUISettings.selectedSprite) : null;

		GUILayout.Space(6f);

		if (mAtlas.replacement != null)
		{
			mType = AtlasType.Reference;
			mReplacement = mAtlas.replacement;
		}

		GUILayout.BeginHorizontal();
		AtlasType after = (AtlasType)EditorGUILayout.EnumPopup("Atlas Type", mType);
		GUILayout.Space(18f);
		GUILayout.EndHorizontal();

		if (mType != after)
		{
			if (after == AtlasType.Normal)
			{
				mType = AtlasType.Normal;
				OnSelectAtlas(null);
			}
			else
			{
				mType = AtlasType.Reference;
			}
		}

		if (mType == AtlasType.Reference)
		{
			ComponentSelector.Draw<UIAtlas>(mAtlas.replacement, OnSelectAtlas, true);

			GUILayout.Space(6f);
			EditorGUILayout.HelpBox("You can have one atlas simply point to " +
				"another one. This is useful if you want to be " +
				"able to quickly replace the contents of one " +
				"atlas with another one, for example for " +
				"swapping an SD atlas with an HD one, or " +
				"replacing an English atlas with a Chinese " +
				"one. All the sprites referencing this atlas " +
				"will update their references to the new one.", MessageType.Info);

			if (mReplacement != mAtlas && mAtlas.replacement != mReplacement)
			{
				NGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
				mAtlas.replacement = mReplacement;
				NGUITools.SetDirty(mAtlas);
			}
			return;
		}

		//GUILayout.Space(6f);
		Material mat = EditorGUILayout.ObjectField("Material", mAtlas.spriteMaterial, typeof(Material), false) as Material;

		if (mAtlas.spriteMaterial != mat)
		{
			NGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
			mAtlas.spriteMaterial = mat;

			// Ensure that this atlas has valid import settings
			if (mAtlas.texture != null) NGUIEditorTools.ImportTexture(mAtlas.texture, false, false, !mAtlas.premultipliedAlpha);

			mAtlas.MarkAsChanged();
		}

		if (mat != null)
		{
			TextAsset ta = EditorGUILayout.ObjectField("TP Import", null, typeof(TextAsset), false) as TextAsset;

			if (ta != null)
			{
				// Ensure that this atlas has valid import settings
				if (mAtlas.texture != null) NGUIEditorTools.ImportTexture(mAtlas.texture, false, false, !mAtlas.premultipliedAlpha);

				NGUIEditorTools.RegisterUndo("Import Sprites", mAtlas);
				NGUIJson.LoadSpriteData(mAtlas, ta);
				if (sprite != null) sprite = mAtlas.GetSprite(sprite.name);
				mAtlas.MarkAsChanged();
			}

			float pixelSize = EditorGUILayout.FloatField("Pixel Size", mAtlas.pixelSize, GUILayout.Width(120f));

			if (pixelSize != mAtlas.pixelSize)
			{
				NGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
				mAtlas.pixelSize = pixelSize;
			}

            if (NGUIEditorTools.DrawHeader("Scale Factor"))
            {
                NGUIEditorTools.BeginContents();
                {
                    EditorGUILayout.HelpBox(
                        "If the image size(pixel size in photoshop) is not equal to texture size(pixel size in unity), you should set the scale factor to help NGUI to adjust uv.", 
                        MessageType.Info, true
                    );

                    EditorGUILayout.HelpBox(
                        "You should click the the button below to recalculate scale factor for atlas when the atlas' texture is changed. For example, when you add sprite, remove sprite, set compressed format or change maxsize etc.",
                        MessageType.Warning, true
                    );

                    if (GUILayout.Button("Calculate Scale Factor Automatically"))
                    {
                        if (mAtlas.texture != null)
                        {
                            string path = AssetDatabase.GetAssetPath(mAtlas.texture);
                            string fullPath = Application.dataPath + path.Substring("Assets".Length);
                            if (File.Exists(fullPath))
                            {
                                MemoryStream memoryStream = null;
                                BinaryReader binaryReader = null;
                                try
                                {
                                    byte[] bytes = File.ReadAllBytes(fullPath);
                                    memoryStream = new MemoryStream(bytes);
                                    binaryReader = new BinaryReader(memoryStream);
                                    // Is PNG
                                    if (binaryReader.ReadByte() == 0x89 && binaryReader.ReadByte() == 0x50 && binaryReader.ReadByte() == 0x4E &&
                                        binaryReader.ReadByte() == 0x47 && binaryReader.ReadByte() == 0x0D && binaryReader.ReadByte() == 0x0A &&
                                        binaryReader.ReadByte() == 0x1A && binaryReader.ReadByte() == 0x0A)
                                    {
                                        binaryReader.ReadUInt32();
                                        binaryReader.ReadUInt32();
                                        uint width = LittleEndianToBigEndian(binaryReader.ReadUInt32());
                                        uint height = LittleEndianToBigEndian(binaryReader.ReadUInt32());
                                        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mAtlas.texture));

                                        System.Action<string, UIAtlas.SCALE_PLATFORM, BuildTarget> processScale = delegate(string platformName, UIAtlas.SCALE_PLATFORM platformAtlasEnum, BuildTarget platformEnum)
                                        {
                                            if (mAtlas.scales == null || mAtlas.scales.Length != (int)UIAtlas.SCALE_PLATFORM.SIZE * 2)
                                            {
                                                mAtlas.scales = new float[(int)UIAtlas.SCALE_PLATFORM.SIZE * 2] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                                            }

                                      //      TextureImportInstructions info = new TextureImportInstructions();
                                      //      textureImporter.ReadTextureImportInstructions(info, platformEnum);
                                            int maxSize;
                                            TextureImporterFormat format;
                                            if (textureImporter.GetPlatformTextureSettings(platformName, out maxSize, out format))
                                            {
                                                float len = width > height ? width : height;
                                                mAtlas.scales[(int)platformAtlasEnum * 2] = len / (float)width;
                                                mAtlas.scales[(int)platformAtlasEnum * 2 + 1] = len / (float)height;
                                            }
                                            else
                                            {
                                                mAtlas.scales[(int)platformAtlasEnum * 2] = 1;
                                                mAtlas.scales[(int)platformAtlasEnum * 2 + 1] = 1;
                                            }
                                        };

                                        processScale("Web", UIAtlas.SCALE_PLATFORM.WEB, BuildTarget.WebPlayer);
                                        processScale("Standalone", UIAtlas.SCALE_PLATFORM.STANDALONE, BuildTarget.StandaloneWindows);
                                        processScale("iPhone", UIAtlas.SCALE_PLATFORM.IPHONE, BuildTarget.iOS);
                                        processScale("Android", UIAtlas.SCALE_PLATFORM.ANDROID, BuildTarget.Android);

                                        NGUITools.SetDirty(mAtlas);
                                    }
                                }
                                catch (System.Exception exception)
                                {
                                    Debug.LogError(exception.StackTrace);
                                    UnityEditor.EditorUtility.DisplayDialog("Error", "Error:Set the scale factor mamually.", "ok");
                                }
                                finally
                                {
                                    if (memoryStream != null)
                                    {
                                        memoryStream.Close();
                                        memoryStream.Dispose();
                                        memoryStream = null;
                                    }
                                    if (binaryReader != null)
                                    {
                                        binaryReader.Close();
                                        binaryReader = null;
                                    }
                                }
                            }
                            else
                            {
                                UnityEditor.EditorUtility.DisplayDialog("Alert", "File is not exists\n" + fullPath, "ok");
                            }
                        }
                        else
                        {
                            UnityEditor.EditorUtility.DisplayDialog("Alert", "Null Texture", "ok");
                        }
                    }
                }
                NGUIEditorTools.EndContents();
            }
		}

		if (mAtlas.spriteMaterial != null)
		{
			Color blueColor = new Color(0f, 0.7f, 1f, 1f);
			Color greenColor = new Color(0.4f, 1f, 0f, 1f);

			if (sprite == null && mAtlas.spriteList.Count > 0)
			{
				string spriteName = NGUISettings.selectedSprite;
				if (!string.IsNullOrEmpty(spriteName)) sprite = mAtlas.GetSprite(spriteName);
				if (sprite == null) sprite = mAtlas.spriteList[0];
			}

			if (sprite != null)
			{
				if (sprite == null) return;
					
				Texture2D tex = mAtlas.spriteMaterial.mainTexture as Texture2D;

				if (tex != null)
				{
					if (!NGUIEditorTools.DrawHeader("Sprite Details")) return;

					NGUIEditorTools.BeginContents();

					GUILayout.Space(3f);
					NGUIEditorTools.DrawAdvancedSpriteField(mAtlas, sprite.name, SelectSprite, true);
					GUILayout.Space(6f);

					GUI.changed = false;

					GUI.backgroundColor = greenColor;
					NGUIEditorTools.IntVector sizeA = NGUIEditorTools.IntPair("Dimensions", "X", "Y", sprite.x, sprite.y);
					NGUIEditorTools.IntVector sizeB = NGUIEditorTools.IntPair(null, "Width", "Height", sprite.width, sprite.height);

					EditorGUILayout.Separator();
					GUI.backgroundColor = blueColor;
					NGUIEditorTools.IntVector borderA = NGUIEditorTools.IntPair("Border", "Left", "Right", sprite.borderLeft, sprite.borderRight);
					NGUIEditorTools.IntVector borderB = NGUIEditorTools.IntPair(null, "Bottom", "Top", sprite.borderBottom, sprite.borderTop);

					EditorGUILayout.Separator();
					GUI.backgroundColor = Color.white;
					NGUIEditorTools.IntVector padA = NGUIEditorTools.IntPair("Padding", "Left", "Right", sprite.paddingLeft, sprite.paddingRight);
					NGUIEditorTools.IntVector padB = NGUIEditorTools.IntPair(null, "Bottom", "Top", sprite.paddingBottom, sprite.paddingTop);

					if (GUI.changed)
					{
						NGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
						
						sprite.x = sizeA.x;
						sprite.y = sizeA.y;
						sprite.width = sizeB.x;
						sprite.height = sizeB.y;

						sprite.paddingLeft = padA.x;
						sprite.paddingRight = padA.y;
						sprite.paddingBottom = padB.x;
						sprite.paddingTop = padB.y;

						sprite.borderLeft = borderA.x;
						sprite.borderRight = borderA.y;
						sprite.borderBottom = borderB.x;
						sprite.borderTop = borderB.y;

						MarkSpriteAsDirty();
					}

					GUILayout.Space(3f);

					GUILayout.BeginHorizontal();

					if (GUILayout.Button("Duplicate"))
					{
						UIAtlasMaker.SpriteEntry se = UIAtlasMaker.DuplicateSprite(mAtlas, sprite.name);
						if (se != null) NGUISettings.selectedSprite = se.name;
					}

					if (GUILayout.Button("Save As..."))
					{
						string path = EditorUtility.SaveFilePanelInProject("Save As", sprite.name + ".png", "png", "Extract sprite into which file?");

						if (!string.IsNullOrEmpty(path))
						{
							UIAtlasMaker.SpriteEntry se = UIAtlasMaker.ExtractSprite(mAtlas, sprite.name);

							if (se != null)
							{
								byte[] bytes = se.tex.EncodeToPNG();
								File.WriteAllBytes(path, bytes);
								AssetDatabase.ImportAsset(path);
								if (se.temporaryTexture) DestroyImmediate(se.tex);
							}
						}
					}
					GUILayout.EndHorizontal();
					NGUIEditorTools.EndContents();
				}

				if (NGUIEditorTools.DrawHeader("Modify"))
				{
					NGUIEditorTools.BeginContents();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					EditorGUILayout.BeginVertical();

					NGUISettings.backgroundColor = EditorGUILayout.ColorField("Background", NGUISettings.backgroundColor);

					if (GUILayout.Button("Add a Shadow")) AddShadow(sprite);
					if (GUILayout.Button("Add a Soft Outline")) AddOutline(sprite);

					if (GUILayout.Button("Add a Transparent Border")) AddTransparentBorder(sprite);
					if (GUILayout.Button("Add a Clamped Border")) AddClampedBorder(sprite);
					if (GUILayout.Button("Add a Tiled Border")) AddTiledBorder(sprite);
					EditorGUI.BeginDisabledGroup(!sprite.hasBorder);
					if (GUILayout.Button("Crop Border")) CropBorder(sprite);
					EditorGUI.EndDisabledGroup();

					EditorGUILayout.EndVertical();
					GUILayout.Space(20f);
					EditorGUILayout.EndHorizontal();

					NGUIEditorTools.EndContents();
				}

				if (NGUIEditorTools.previousSelection != null)
				{
					GUILayout.Space(3f);
					GUI.backgroundColor = Color.green;

					if (GUILayout.Button("<< Return to " + NGUIEditorTools.previousSelection.name))
					{
						NGUIEditorTools.SelectPrevious();
					}
					GUI.backgroundColor = Color.white;
				}
			}
		}
	}

	/// <summary>
	/// Sprite selection callback.
	/// </summary>

	void SelectSprite (string spriteName)
	{
		NGUISettings.selectedSprite = spriteName;
		Repaint();
	}

	/// <summary>
	/// All widgets have a preview.
	/// </summary>

	public override bool HasPreviewGUI () { return true; }

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		UISpriteData sprite = (mAtlas != null) ? mAtlas.GetSprite(NGUISettings.selectedSprite) : null;
		if (sprite == null) return;

		Texture2D tex = mAtlas.texture as Texture2D;
        if (tex != null) NGUIEditorTools.DrawSprite(tex, rect, sprite, Color.white, 1.0f / mAtlas.scaleX, 1.0f / mAtlas.scaleY);
	}

	/// <summary>
	/// Add a transparent border around the sprite.
	/// </summary>

	void AddTransparentBorder (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width;
			int h1 = se.tex.height;

			int w2 = w1 + 2;
			int h2 = h1 + 2;

			Color32[] c2 = NGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			++se.borderLeft;
			++se.borderRight;
			++se.borderTop;
			++se.borderBottom;

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}

	/// <summary>
	/// Add a border around the sprite that extends the existing edge pixels.
	/// </summary>

	void AddClampedBorder (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width - se.borderLeft - se.borderRight;
			int h1 = se.tex.height - se.borderBottom - se.borderTop;

			int w2 = se.tex.width + 2;
			int h2 = se.tex.height + 2;

			Color32[] c1 = se.tex.GetPixels32();
			Color32[] c2 = new Color32[w2 * h2];

			for (int y2 = 0; y2 < h2; ++y2)
			{
				int y1 = se.borderBottom + NGUIMath.ClampIndex(y2 - se.borderBottom - 1, h1);

				for (int x2 = 0; x2 < w2; ++x2)
				{
					int x1 = se.borderLeft + NGUIMath.ClampIndex(x2 - se.borderLeft - 1, w1);
					c2[x2 + y2 * w2] = c1[x1 + y1 * se.tex.width];
				}
			}

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			++se.borderLeft;
			++se.borderRight;
			++se.borderTop;
			++se.borderBottom;

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}

	/// <summary>
	/// Add a border around the sprite that copies the pixels from the opposite side, making it possible for the sprite to tile without seams.
	/// </summary>

	void AddTiledBorder (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width - se.borderLeft - se.borderRight;
			int h1 = se.tex.height - se.borderBottom - se.borderTop;

			int w2 = se.tex.width + 2;
			int h2 = se.tex.height + 2;

			Color32[] c1 = se.tex.GetPixels32();
			Color32[] c2 = new Color32[w2 * h2];

			for (int y2 = 0; y2 < h2; ++y2)
			{
				int y1 = se.borderBottom + NGUIMath.RepeatIndex(y2 - se.borderBottom - 1, h1);

				for (int x2 = 0; x2 < w2; ++x2)
				{
					int x1 = se.borderLeft + NGUIMath.RepeatIndex(x2 - se.borderLeft - 1, w1);
					c2[x2 + y2 * w2] = c1[x1 + y1 * se.tex.width];
				}
			}

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			++se.borderLeft;
			++se.borderRight;
			++se.borderTop;
			++se.borderBottom;

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}

	/// <summary>
	/// Crop the border pixels around the sprite.
	/// </summary>

	void CropBorder (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width;
			int h1 = se.tex.height;

			int w2 = w1 - se.borderLeft - se.borderRight;
			int h2 = h1 - se.borderTop - se.borderBottom;

			Color32[] c1 = se.tex.GetPixels32();
			Color32[] c2 = new Color32[w2 * h2];

			for (int y2 = 0; y2 < h2; ++y2)
			{
				int y1 = y2 + se.borderBottom;

				for (int x2 = 0; x2 < w2; ++x2)
				{
					int x1 = x2 + se.borderLeft;
					c2[x2 + y2 * w2] = c1[x1 + y1 * w1];
				}
			}

			se.borderLeft = 0;
			se.borderRight = 0;
			se.borderTop = 0;
			se.borderBottom = 0;

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}

	/// <summary>
	/// Add a dark shadow below and to the right of the sprite.
	/// </summary>

	void AddShadow (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width;
			int h1 = se.tex.height;

			int w2 = w1 + 2;
			int h2 = h1 + 2;

			Color32[] c2 = NGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);
			NGUIEditorTools.AddShadow(c2, w2, h2, NGUISettings.backgroundColor);

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			if ((se.borderLeft | se.borderRight | se.borderBottom | se.borderTop) != 0)
			{
				++se.borderLeft;
				++se.borderRight;
				++se.borderTop;
				++se.borderBottom;
			}

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}

	/// <summary>
	/// Add a dark shadowy outline around the sprite, giving it some visual depth.
	/// </summary>

	void AddOutline (UISpriteData sprite)
	{
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(mAtlas, sprites);
		UIAtlasMaker.SpriteEntry se = null;

		for (int i = 0; i < sprites.Count; ++i)
		{
			if (sprites[i].name == sprite.name)
			{
				se = sprites[i];
				break;
			}
		}

		if (se != null)
		{
			int w1 = se.tex.width;
			int h1 = se.tex.height;

			int w2 = w1 + 2;
			int h2 = h1 + 2;

			Color32[] c2 = NGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);
			NGUIEditorTools.AddDepth(c2, w2, h2, NGUISettings.backgroundColor);

			if (se.temporaryTexture) DestroyImmediate(se.tex);

			if ((se.borderLeft | se.borderRight | se.borderBottom | se.borderTop) != 0)
			{
				++se.borderLeft;
				++se.borderRight;
				++se.borderTop;
				++se.borderBottom;
			}

			se.tex = new Texture2D(w2, h2);
			se.tex.name = sprite.name;
			se.tex.SetPixels32(c2);
			se.tex.Apply();
			se.temporaryTexture = true;

			UIAtlasMaker.UpdateAtlas(mAtlas, sprites);

			DestroyImmediate(se.tex);
			se.tex = null;
		}
	}
}