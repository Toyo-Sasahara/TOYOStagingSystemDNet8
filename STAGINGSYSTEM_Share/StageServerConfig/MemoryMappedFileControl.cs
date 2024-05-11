using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// サービス間ステータス共有クラス
    /// </summary>
    public class MemoryMappedFileControl
    {

        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// メモリマップドファイル
        /// </summary>
        private MemoryMappedFile mmf;

        // TODO:MemoryMappedFileSecurity が .NETにはない
        //MemoryMappedFileSecurity accessControl;


        /* ------ */

        public MemoryMappedFileControl(string Label)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;

            try
            {
                mmf = MemoryMappedFile.OpenExisting(mmfLabel);
            }
            catch (Exception ex)
            {
                mmf = null;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外　{ex.Message}");
            }
        }

        /// <summary>
        /// コンストラクタ（string）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="value">null, "", ホワイトスペースの時 Writeしない</param>
        public MemoryMappedFileControl(string Label, string value = null)
        {
            string mmfLabel = "TOYOSTAGE_" + Label;
            try
            {
                mmf = MemoryMappedFile.CreateOrOpen(mmfLabel, 2048);

                // TODO:MemoryMappedFileSecurity が .NETにはない
                ///accessControl = mmf.GetAccessControl();

                if (value != null)
                {
                    using (MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor())
                    {
                        byte[] textBytes = Encoding.UTF8.GetBytes(value);
                        viewAccessor.Write(0, textBytes.Length); // メモリの先頭にbyte配列の大きさを書き込む
                        viewAccessor.WriteArray(sizeof(byte), textBytes, 0, textBytes.Length); // 本文を書き込む
                    }
                }
            }
            catch (Exception ex)
            {
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外　{ex.Message}");
            }
        }

        /// <summary>
        /// コンストラクタ（int）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="value"></param>
        public MemoryMappedFileControl(string Label, int value)
        {
            try
            {
                mmf = MemoryMappedFile.CreateOrOpen("TOYOSTAGE_" + Label, 4);


                Write(value);
            }
            catch (Exception ex)
            {
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外　{ex.Message}");
            }
        }

        /// <summary>
        /// コンストラクタ（bool）
        /// </summary>
        /// <param name=""TOYOSTAGE_"+Label"></param>
        /// <param name="flag"></param>
        public MemoryMappedFileControl(string Label, bool flag)
        {
            try
            {
                mmf = MemoryMappedFile.CreateOrOpen("TOYOSTAGE_" + Label, 1);


                if (flag)
                    Write(1);
                else
                    Write(0);
            }
            catch (Exception ex)
            {
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"例外　{ex.Message}");
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
