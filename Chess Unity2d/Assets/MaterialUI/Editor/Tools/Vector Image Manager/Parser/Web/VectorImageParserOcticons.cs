//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
    public class VectorImageParserOcticons : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/primer/octicons/raw/master/build/font/octicons.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://github.com/primer/octicons/raw/master/LICENSE?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://raw.githubusercontent.com/primer/octicons/master/build/font/octicons.css";
        }

        public override string GetWebsite()
        {
            return "https://octicons.github.com/";
        }

        public override string GetFontName()
        {
            return "Octicons";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            var vectorImageSet = new VectorImageSet();

            foreach (var line in fontDataContent.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries))
                if (line.StartsWith(".octicon-") && line.Contains("content:"))
                {
                    var name = line.Replace(".octicon-", string.Empty).Trim();
                    name = name.Split(new[] {":"}, StringSplitOptions.None)[0];

                    var unicode = line.Split(new[] {"\""}, StringSplitOptions.None)[1].Replace("\\", "");

                    var glyph = new Glyph(name, unicode, false);
                    vectorImageSet.iconGlyphList.Add(glyph);
                }

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            return fontDataLicenseContent;
        }
    }
}