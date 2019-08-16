//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_5_2
using LitJson;
#endif

namespace MaterialUI
{
    public abstract class VectorImageFontParser
    {
        private VectorImageSet m_CachedVectorImageSet;
        private Action m_OnDoneDownloading;
        protected abstract string GetIconFontUrl();
        protected abstract string GetIconFontLicenseUrl();
        protected abstract string GetIconFontDataUrl();
        public abstract string GetFontName();
        public abstract string GetWebsite();
        protected abstract VectorImageSet GenerateIconSet(string fontDataContent);
        protected abstract string ExtractLicense(string fontLicenseDataContent);

        protected virtual void CleanUp()
        {
        }

        public void DownloadIcons(Action onDoneDownloading = null)
        {
            m_OnDoneDownloading = onDoneDownloading;
            EditorCoroutine.Start(DownloadIconFontCoroutine());
        }

        private IEnumerator DownloadIconFontCoroutine()
        {
            var iconFontURL = GetIconFontUrl();
            if (string.IsNullOrEmpty(iconFontURL)) yield break;

            var www = new WWW(iconFontURL);
            while (!www.isDone) yield return null;

            if (!string.IsNullOrEmpty(www.error))
            {
                ClearProgressBar();
                throw new Exception("Error downloading icon font (" + GetFontName() + ") at path: " + GetIconFontUrl() +
                                    " - " + www.error);
            }

            CreateFolderPath();

            File.WriteAllBytes(GetIconFontPath(), www.bytes);
            EditorCoroutine.Start(DownloadFontLicenseCoroutine());
        }

        private IEnumerator DownloadFontLicenseCoroutine()
        {
            if (!string.IsNullOrEmpty(GetIconFontLicenseUrl()))
            {
                var www = new WWW(GetIconFontLicenseUrl());
                while (!www.isDone) yield return null;

                if (!string.IsNullOrEmpty(www.error))
                {
                    ClearProgressBar();
                    throw new Exception("Error downloading icon font license (" + GetFontName() + ") at path: " +
                                        GetIconFontLicenseUrl());
                }

                CreateFolderPath();

                var licenseData = ExtractLicense(www.text);

                File.WriteAllText(GetIconFontLicensePath(), licenseData);
            }

            EditorCoroutine.Start(DownloadIconFontData());
        }

        private IEnumerator DownloadIconFontData()
        {
            var www = new WWW(GetIconFontDataUrl());
            while (!www.isDone) yield return null;

            if (!string.IsNullOrEmpty(www.error))
            {
                ClearProgressBar();
                throw new Exception("Error downloading icon font data (" + GetFontName() + ") at path: " +
                                    GetIconFontDataUrl() + "\n" + www.error);
            }

            CreateFolderPath();

            var vectorImageSet = GenerateIconSet(www.text);
            FormatNames(vectorImageSet);

#if UNITY_5_2
			string codePointJson = JsonMapper.ToJson(vectorImageSet);
			codePointJson =
 codePointJson.Replace("name", "m_Name").Replace("unicode", "m_Unicode").Replace("iconGlyphList", "m_IconGlyphList");
#else
            var codePointJson = JsonUtility.ToJson(vectorImageSet);
#endif

            File.WriteAllText(GetIconFontDataPath(), codePointJson);

            if (m_OnDoneDownloading != null) m_OnDoneDownloading();

            CleanUp();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        private void CreateFolderPath()
        {
            if (!Directory.Exists(GetFolderPath())) Directory.CreateDirectory(GetFolderPath());
        }

        public string GetFolderPath()
        {
            var path = Application.dataPath + "/" + VectorImageManager.fontDestinationFolder + "/" + GetFontName() +
                       "/";

            if (VectorImageManager.IsMaterialDesignIconFont(GetFontName()))
                path = VectorImageManager.materialDesignIconsFolderPath + "/";

            path = path.Replace("//", "/");
            return path;
        }

        private string GetIconFontPath()
        {
            return GetFolderPath() + GetFontName() + ".ttf";
        }

        public string GetIconFontLicensePath()
        {
            return GetFolderPath() + "LICENSE.txt";
        }

        private string GetIconFontDataPath()
        {
            return GetFolderPath() + GetFontName() + ".json";
        }

        public bool IsFontAvailable()
        {
            var isFontAvailable = File.Exists(GetIconFontPath());
            var isFontDataAvailable = File.Exists(GetIconFontDataPath());

            return isFontAvailable && isFontDataAvailable;
        }

        private void FormatNames(VectorImageSet set)
        {
            for (var i = 0; i < set.iconGlyphList.Count; i++)
            {
                var name = set.iconGlyphList[i].name;
                name = name.Replace("-", "_");
                name = name.Replace(" ", "_");
                name = name.ToLower();
                set.iconGlyphList[i].name = name;

                var unicode = set.iconGlyphList[i].unicode;
                unicode = unicode.Replace("\\u", "");
                unicode = unicode.Replace("\\\\u", "");
                set.iconGlyphList[i].unicode = unicode;
            }
        }

        public VectorImageSet GetIconSet()
        {
            if (!IsFontAvailable())
                throw new Exception("Can't get the icon set because the font has not been downloaded");

#if UNITY_5_2
			string iconFontData = File.ReadAllText(GetIconFontDataPath());
			iconFontData =
 iconFontData.Replace("m_Name", "name").Replace("m_Unicode", "unicode").Replace("m_IconGlyphList", "iconGlyphList");
			VectorImageSet vectorImageSet = JsonMapper.ToObject<VectorImageSet>(iconFontData);
#else
            var vectorImageSet = JsonUtility.FromJson<VectorImageSet>(File.ReadAllText(GetIconFontDataPath()));
#endif

            return vectorImageSet;
        }

        public VectorImageSet GetCachedIconSet()
        {
            if (m_CachedVectorImageSet == null) m_CachedVectorImageSet = GetIconSet();

            return m_CachedVectorImageSet;
        }

        public void Delete()
        {
            var path = GetFolderPath();

            // Delete folder
            Directory.Delete(path, true);

            // Sync AssetDatabase with the delete operation.
            var metaPath = path.Substring(Application.dataPath.IndexOf("/Assets") + 1);
            if (metaPath.EndsWith("/")) metaPath = metaPath.Substring(0, metaPath.Length - 1);

            AssetDatabase.DeleteAsset(metaPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}