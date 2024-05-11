//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace AdditinalConfig
//{
//    static class Utility_FileFolder
//    {

//        /// <summary>
//        /// フルパスファイル名の拡張子を除いたファイル名の部分を別の文字列に置き換える
//        /// C:\ABC\DEF.TXT  -> C:\ABC\XXXX.TXT
//        /// </summary>
//        /// <param name="sourceFullpath"></param>
//        /// <param name="filenameawithoutExt"></param>
//        /// <returns></returns>
//        public static string RenameFileOtherThanTheFileFullpathNameExtension(string sourceFullpath, string filenameawithoutExt)
//        {
//            string dotExtention = System.IO.Path.GetExtension(sourceFullpath);

//            string pathWithoutFilename = System.IO.Path.GetDirectoryName(sourceFullpath);
//            string anser = System.IO.Path.Combine(pathWithoutFilename, filenameawithoutExt + dotExtention);
//            return anser;
//        }


//    }
//}
