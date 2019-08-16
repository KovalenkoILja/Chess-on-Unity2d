//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

#if UNITY_5_2
using LitJson;
#endif

namespace MaterialUI
{
    public class VectorImageParserIonicons : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/driftyco/ionicons/blob/master/fonts/ionicons.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://github.com/driftyco/ionicons/blob/master/LICENSE?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://raw.githubusercontent.com/driftyco/ionicons/master/builder/manifest.json?raw=true";
        }

        public override string GetWebsite()
        {
            return "http://ionicons.com/";
        }

        public override string GetFontName()
        {
            return "Ionicons";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            var vectorImageSet = new VectorImageSet();

#if UNITY_5_2
			JsonData jsonData = JsonMapper.ToObject(fontDataContent);
			JsonData iconArray = jsonData["icons"];

			for (int i = 0; i < iconArray.Count; i++)
			{
				JsonData iconData = iconArray[i];
				string name = iconData["name"].ToString();
				string unicode = iconData["code"].ToString();
				unicode = unicode.Replace("0x", string.Empty);

				vectorImageSet.iconGlyphList.Add(new Glyph(name, unicode, false));
			}
#else
            fontDataContent = fontDataContent.Replace("name", "m_Name").Replace("code", "m_Unicode")
                .Replace("icons", "m_IconGlyphList");

            var ioniconsInfo = JsonUtility.FromJson<VectorImageSet>(fontDataContent);

            for (var i = 0; i < ioniconsInfo.iconGlyphList.Count; i++)
            {
                var name = ioniconsInfo.iconGlyphList[i].name;
                var unicode = ioniconsInfo.iconGlyphList[i].unicode;
                unicode = unicode.Replace("0x", string.Empty);

                vectorImageSet.iconGlyphList.Add(new Glyph(name, unicode, false));
            }
#endif

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            return fontDataLicenseContent;
        }
    }
}