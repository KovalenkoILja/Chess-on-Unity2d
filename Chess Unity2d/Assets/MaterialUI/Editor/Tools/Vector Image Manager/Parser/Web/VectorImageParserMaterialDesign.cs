//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MaterialUI
{
    public class VectorImageParserMaterialDesign : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
            return
                "https://github.com/google/material-design-icons/blob/master/iconfont/MaterialIcons-Regular.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://github.com/google/material-design-icons/blob/master/LICENSE?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://github.com/google/material-design-icons/raw/master/iconfont/codepoints?raw=true";
        }

        public override string GetWebsite()
        {
            return "https://www.google.com/design/icons/";
        }

        public override string GetFontName()
        {
            return "Material Design Icons";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            var vectorImageSet = new VectorImageSet();

            foreach (var line in fontDataContent.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries))
            {
                var lineData = line.Split(' ');
                var iconname = lineData[0];
                var unicode = lineData[1];

                vectorImageSet.iconGlyphList.Add(new Glyph(iconname, unicode, false));
            }

            GenerateIconEnum(vectorImageSet.iconGlyphList);

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            return fontDataLicenseContent;
        }

        private void GenerateIconEnum(List<Glyph> glyphList)
        {
#if UNITY_EDITOR
            var content = "//  Copyright 2016 MaterialUI for Unity http://materialunity.com";
            content += "\n//  Please see license file for terms and conditions of use, and more information.";
            content += "\n";
            content += "\nnamespace MaterialUI";
            content += "\n{";
            content += "\n\tpublic enum MaterialIconEnum";
            content += "\n\t{";

            for (var i = 0; i < glyphList.Count; i++)
            {
                var name = glyphList[i].name.ToUpper().Replace(" ", "_");
                if (char.IsDigit(name[0])) name = "_" + name;

                content += "\n\t\t" + name + ",";
            }

            content += "\n\t}";
            content += "\n}";

            File.WriteAllText(Application.dataPath + "/MaterialUI/Scripts/Components/VectorImages/MaterialIconEnum.cs",
                content);
#endif
        }
    }
}