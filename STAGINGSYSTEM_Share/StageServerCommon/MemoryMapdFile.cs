using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SasaLib;

namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// メモリマップドファイル直接制御
    /// </summary>
    public class MemoryMapdFile
    {
        /// <summary>
        /// メモリマップドファイル
        /// </summary>
        private MemoryMappedFile mmf;

        // TODO:MemoryMappedFileSecurit は .NETcoreに存在しない
        //MemoryMappedFileSecurity accessControl;

        /* ------ */

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Label"> "TOYOSTAGE_" + Label; を メモリマップドファイルのラベルとします</param>
        public MemoryMapdFile(string Label)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;

            try
            {
                mmf = MemoryMappedFile.OpenExisting(mmfLabel);
            }
            catch (Exception ex)
            {
                mmf = null;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外検知  MemoryMapdFile.cs　MemoryMappedFile.OpenExisting({mmfLabel})　{ex.Message} {ex.InnerException}");
            }
        }

        /// <summary>
        /// コンストラクタ（string）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="value">null, "", ホワイトスペースの時 Writeしない</param>
        public MemoryMapdFile(string Label, string value)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;
            try
            {
                mmf = MemoryMappedFile.CreateOrOpen(mmfLabel, 2048);

                // TODO:MemoryMappedFileSecurit は .NETcoreに存在しない
                //accessControl = mmf.GetAccessControl();

                using (MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor())
                {
                    byte[] textBytes = Encoding.UTF8.GetBytes(value);
                    viewAccessor.Write(0, textBytes.Length); // メモリの先頭にbyte配列の大きさを書き込む
                    viewAccessor.WriteArray(sizeof(byte), textBytes, 0, textBytes.Length); // 本文を書き込む
                }
            }
            catch (Exception ex)
            {
                mmf = null;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外検知  ToyoSTAGINGSYSTEMwatich MemoryMapdFile.cs　MemoryMapdFile.MemoryMapdFile(..)　{ex.Message} {ex.InnerException}");
            }
        }

        /// <summary>
        /// コンストラクタ（int）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="value"></param>
        public MemoryMapdFile(string Label, int value)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;

            try
            {
                mmf = MemoryMappedFile.CreateOrOpen(mmfLabel, 4);
                Write(value);
            }
            catch (Exception ex)
            {
                mmf = null;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外検知  ToyoSTAGINGSYSTEMwatich MemoryMapdFile.cs　MemoryMapdFile.MemoryMapdFile(..)　{ex.Message} {ex.InnerException}");
            }
        }

        /// <summary>
        /// コンストラクタ（bool）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="flag"></param>
        public MemoryMapdFile(string Label, bool flag)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;

            try
            {
                mmf = MemoryMappedFile.CreateOrOpen(mmfLabel, 1);


                if (flag)
                    Write(1);
                else
                    Write(0);
            }
            catch (Exception ex)
            {
                mmf = null;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外検知  ToyoSTAGINGSYSTEMwatich MemoryMapdFile.cs　MemoryMapdFile.MemoryMapdFile(..)　{ex.Message} {ex.InnerException}");
            }

        }

        /* ------ */

        /// <summary>
        /// 読み出し（string）
        /// </summary>
        /// <returns></returns>
        public string ReadStr()
        {
            using (MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor())
            {
                int size = viewAccessor.ReadByte(0); // 文字列のサイズを最初に読み込み

                byte[] bytes = new byte[size];

                int res = viewAccessor.ReadArray(sizeof(byte), bytes, 0, bytes.Length);
                string text = Encoding.UTF8.GetString(bytes);

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, $"ToyoStageService.MemoryMapdFile.ReadStr() を実行　 ReadStr() = \"{text}\"");

                return text;
            }
        }

        /// <summary>
        /// 読み出し（int）
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            int ans;
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                ans = reader.ReadInt32();
            }
            return ans;
        }

        /// <summary>
        /// 読み出し（bool）
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            bool ans;
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                ans = reader.ReadBoolean();
            }
            return ans;
        }

        /* ------ */

        /// <summary>
        /// 書込み（string）
        /// </summary>
        /// <param name="Value"></param>
        public void Write(string Value)
        {
            using (MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor())
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(Value);
                viewAccessor.Write(0, textBytes.Length); // メモリの先頭にbyte配列の大きさを書き込む
                viewAccessor.WriteArray(sizeof(byte), textBytes, 0, textBytes.Length); // 本文を書き込む
            }
        }

        /// <summary>
        /// 書込み（int）
        /// </summary>
        /// <param name="Value"></param>
        public void Write(int Value)
        {
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(Value);
            }
        }

        /// <summary>
        /// 書込み（boolean）
        /// </summary>
        /// <param name="Value"></param>
        public void Write(bool Value)
        {
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(Value);
            }
        }
    }

}
