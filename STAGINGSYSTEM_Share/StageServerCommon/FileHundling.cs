using SasaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SIP = System.IO.Path;
using ToyoMcMfg.Staging.DataBaseConfig;
using System.Runtime.Versioning;


namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    public class FileHundling
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// データベースからの検索結果を元にTIFFイメージを指定フォルダへコピーする
        /// ファイル名はSANITIZEDPARTNUMBERを採用する
        /// </summary>
        /// <param name="SaveFolder"></param>
        /// <returns></returns>
        public static bool CopyTiffFilesWitthSANITIZEDPARTNUMBER(List<FieldValueSet> DBresultList, string SaveFolder)
        {
            bool anser = false;

            foreach (var fieldValueSet in DBresultList)
            {
                if (fieldValueSet.Sucess)
                {
                    string TICKETCODE = fieldValueSet.SearchKey("TICKETCODE");
                    string SANITIZEDPARTNUMBER = fieldValueSet.SearchKey("SANITIZEDPARTNUMBER");

                    string sourceFullPath = StageServerConfig.Config.FileStoreFolder + SIP.DirectorySeparatorChar + TICKETCODE + @".TIF";
                    string distFullPath = SaveFolder.TrimEnd(SIP.DirectorySeparatorChar) + SIP.DirectorySeparatorChar + SANITIZEDPARTNUMBER + @".TIF";

                    try
                    {
                        // 上書きコピー
                        File.Copy(sourceFullPath, distFullPath, true);
                    }
                    catch (IOException iex)
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"{iex.Message}");
                        anser = false;
                    }

                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "-----------------------------------");
                    foreach (var x in fieldValueSet.Params)
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"【{x.Field}】　【{x.Value.ToString()}】　【{x.SqlDBType.ToString()}】");
                    }
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "-----------------------------------");


                    anser = true;
                }
                else
                    anser = false;
            }
            return anser;
        }

        /// <summary>
        /// ストアフォルダから削除
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        public static bool DeleteFile(string GUIDBASE64)
        {
            string TICKETCODE = SasaLib.GUIDExtensions.GetB64FnameStringFromB64String(GUIDBASE64);

            var FileSotreFolder = StageServerConfig.Config.FileStoreFolder;

            var TIF = FileSotreFolder + SIP.DirectorySeparatorChar + TICKETCODE + ".TIF";
            var XML = FileSotreFolder + SIP.DirectorySeparatorChar + TICKETCODE + ".XML";

            try
            {

                bool ansTIF = FileFolder.RemoveFile(TIF);
                bool ansXML = FileFolder.RemoveFile(XML);

                if (ansTIF || ansXML)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Information, 5105, $"{AssemblyInternalName} DataBaseDelet()。削除対象 {TIF}={ansTIF} , {XML}={ansXML}");
                }
                else

                    return true;
            }
            catch (IOException iex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Information, 5105, $"{AssemblyInternalName} DataBaseDelet()。エラー 削除対象 {TIF} , {XML}\nエラー原因：{iex.Message}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// サブホストから実体を削除
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="subhostuncFOLDER"></param>
        /// <returns></returns>
        public static bool DeleteFile(string GUIDBASE64, string subhostuncFOLDER, bool debug = false)
        {
            string TICKETCODE = SasaLib.GUIDExtensions.GetB64FnameStringFromB64String(GUIDBASE64);


            var TifFullFileName = subhostuncFOLDER + SIP.DirectorySeparatorChar + TICKETCODE + ".TIF";
            var XMLFullFileName = subhostuncFOLDER + SIP.DirectorySeparatorChar + TICKETCODE + ".XML";

            try
            {

                bool ansTIF = FileFolder.RemoveFile(TifFullFileName);
                bool ansXML = FileFolder.RemoveFile(XMLFullFileName);

                if (ansTIF || ansXML)
                {
                    if (debug)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Information, 5105, $"{AssemblyInternalName} DataBaseDelet()。削除対象 {TifFullFileName}={ansTIF} , {XMLFullFileName}={ansXML}");
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (IOException iex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Information, 5105, $"{AssemblyInternalName} DataBaseDelet()。エラー 削除対象 {TifFullFileName} , {XMLFullFileName}\nエラー原因：{iex.Message}");
                return false;
            }
            return true;
        }

        private static string GetTikectStoreFileFullPath(string sourceTiketName)
        {
            string sourceTicketFileFullPath = System.IO.Path.Combine(StageServerConfig.Config.FileStoreFolder, System.IO.Path.ChangeExtension(sourceTiketName, "XML"));
            return sourceTicketFileFullPath;
        }

        private static string GetTIFFStoreFileFullPath(string sourceTiketName)
        {
            string sourceTiffFileFullPath = System.IO.Path.Combine(StageServerConfig.Config.FileStoreFolder, System.IO.Path.ChangeExtension(sourceTiketName, "TIF"));
            return sourceTiffFileFullPath;
        }

        /// <summary>
        /// TIFFイメージフルパスを生成（データベースに問合せしない）
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        public static string GetTiffImageFileFullPath(string GUIDBASE64, string extension = @"TIF")
        {
            string TICKETCODE = SasaLib.GUIDExtensions.GetB64FnameStringFromB64String(GUIDBASE64);
            if (TICKETCODE != null)
            {
                string filename = TICKETCODE + @"." + extension;
                string ImageFilePath = StageServerConfig.Config.FileStoreFolder + SIP.DirectorySeparatorChar + filename;
                return ImageFilePath;
            }
            return null;
        }


        /// <summary>
        /// 指定したチケットファイルの実体ファイルを指定したフォルダへ上書きコピー
        /// </summary>
        /// <param name="sourceTiketName"></param>
        /// <param name="distFolderPath"></param>
        /// <returns></returns>
        public static bool FileStoreReplication(string sourceTiketName, string distFolderPath, bool eventOutput = false)
        {
            string sourceTicketFileFullPath = GetTikectStoreFileFullPath(sourceTiketName);
            string sourceTiffFileFullPath = GetTIFFStoreFileFullPath(sourceTiketName);

            string distTicketFileFullPath = System.IO.Path.Combine(distFolderPath, System.IO.Path.GetFileName(sourceTicketFileFullPath));
            string distTiffFileFullPath = System.IO.Path.Combine(distFolderPath, System.IO.Path.GetFileName(sourceTiffFileFullPath));

            try
            {
                // 冗長先サーバーのFILESTOREへチケットファイルの直接コピー実行
                File.Copy(sourceTicketFileFullPath, distTicketFileFullPath, true);

                // 冗長先サーバーのFILESTOREへTIFFファイルの直接コピー実行
                File.Copy(sourceTiffFileFullPath, distTiffFileFullPath, true);

                if (eventOutput)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Information, 5105,
                        $"{AssemblyInternalName} FileStoreReplication(...)サブホストへチケットファイルとTIFFファイルのコピーに成功しました.\n{sourceTiketName} -> {distFolderPath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoFILEhundling", EventLogEntryType.Error, 5105,
                    $"{AssemblyInternalName} FileStoreReplication(...)サブホストへチケットファイルとTIFFファイルのコピー失敗\n{sourceTiketName} -> {distFolderPath}\nex.Message={ex.Message}");
                return false;

            }

        }


    }

}
