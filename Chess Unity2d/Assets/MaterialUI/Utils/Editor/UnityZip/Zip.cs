//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using System.IO;
using Ionic.Zip;

namespace MaterialUI
{
    public class ZipUtil
    {
        public static void Unzip(string zipFilePath, string location)
        {
            Directory.CreateDirectory(location);

            using (var zip = ZipFile.Read(zipFilePath))
            {
                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
#endif