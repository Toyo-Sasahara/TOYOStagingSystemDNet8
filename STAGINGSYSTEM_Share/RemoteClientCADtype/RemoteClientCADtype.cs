
using SasaLib;
using SasaLib.PIPE;
using SasaLibDummy;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace StageServerRemote
{
    /// <summary>
    /// CadType エミュレータに関するクラス
    /// </summary>
    public class RemoteClientCADtype
    {
        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public static int ClientTimeOut { get; set; } = 90000;

        public int ReadStreamStringTimeOut { get; set; } = 20000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        /// <summary>
        /// ■CadType
        /// </summary>
        public enum CadType
        {
            NotSet = 0,             //0000_0000
            AutoCAD2D = 1,          //0000_0001
            SolidWorksModel = 2,    //0000_0010
            SolidWorksDraw = 4,     //0000_0100
            InventorModel = 8,      //0000_1000
            InventorDraw = 16,      //0001_0000
            undefined_1 = 32,       //0010_0000
            undefined_2 = 64,       //0100_0000
            undefined_3 = 128       //1000_0000
        }

        /// <summary>
        /// ■アークスイートのCADType属性値の構造体
        /// </summary>
        public struct ArcSuiteAttr
        {
            public string ZUBAN;
            public string TOROKUBI;
            public CadType CadType;
        }

        string PipeServerName;
        string PipeNameDR;
        string ClientDomainName;
        string ClientUserName;
        string ClientUserPassword;
        bool ClsLogon;

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="ClientDomainName"></param>
        /// <param name="ClientUserName"></param>
        /// <param name="ClientUserPassword"></param>
        /// <param name="ClsLogon"></param>
        /// <param name="PipeServerName"></param>
        /// <param name="PipeName"></param>
        public RemoteClientCADtype(
            string ClientDomainName,
            string ClientUserName, 
            string ClientUserPassword,
            bool ClsLogon, 
            string PipeServerName, 
            string PipeName
            )
        {
            this.PipeServerName = PipeServerName;
            this.PipeNameDR = PipeName;
            this.ClientDomainName = ClientDomainName;
            this.ClientUserName = ClientUserName;
            this.ClientUserPassword = ClientUserPassword;
            this.ClsLogon = ClsLogon;
        }

        /// <summary>
        /// 未定義
        /// </summary>
        /// <param name="arcSuiteZuban"></param>
        /// <param name="arcsuiteAttr"></param>
        /// <param name="writeLine"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool GetArcSuiteAttrCADType(string arcSuiteZuban, ref object arcsuiteAttr, object writeLine)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// ■アークスイートの属性現在のcadtypeに対して enum CadTypeを足したり引いたりする
        /// </summary>
        /// <param name="PipeServerName">PIPEサーバー名</param>
        /// <param name="drawregist_pipename">接続PIPE名</param>
        /// <param name="arcSuiteZuban">図面番号</param>
        /// <param name="newCadType">追加するCadTypeコード</param>
        /// <param name="SetMode"></param>
        /// <param name="ArcSuiteUser"></param>
        /// <param name="ArcSuiteUserPass"></param>
        /// <param name="currentCadTypeEnmu"></param>
        /// <returns></returns>
        public bool SetUnSetArcSuiteAttrCADType(string arcSuiteZuban, CadType newCadType, bool SetMode, ref CadType currentCadTypeEnmu, string ArcSuiteUser, string ArcSuiteUserPass, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;


            ArrayList ans;

            ans = GetArcSuiteAttribute(arcSuiteZuban, "user:cadtype");

            if (ans == null)
                return false;

            if (ans.Count != 2)
                return false;

            string user_strCadtype = SasaLib.Csv.GetFromCsvArrayList(ans, "user:cadtype", 1);
            int previous_cadtype;

            if (String.IsNullOrEmpty(user_strCadtype))
                previous_cadtype = 0;
            else
            {
                if (int.TryParse(user_strCadtype, out previous_cadtype) == false)
                    return false;
            }

            // intをCadType
            currentCadTypeEnmu = (CadType)previous_cadtype;

            WriteLine("→変更前のCadType値を二進数表示 " + Convert.ToString((int)currentCadTypeEnmu, 2).PadLeft(8, '0'));

            if (SetMode)
            {
                WriteLine("→セットするCadType値を二進数表示 " + Convert.ToString((int)newCadType, 2).PadLeft(8, '0'));

                if (currentCadTypeEnmu.HasFlag(newCadType))
                {
                    WriteLine($"→すでに{newCadType}はセット済みでした");
                    return true;
                }
                // 再設定する値を計算
                currentCadTypeEnmu = currentCadTypeEnmu | newCadType;

            }
            else
            {
                WriteLine("→アンセットするCadType値を二進数表示 " + Convert.ToString((int)newCadType, 2).PadLeft(8, '0'));
                if (!currentCadTypeEnmu.HasFlag(newCadType))
                {
                    WriteLine($"→すでに{newCadType}はアンセット済みでした");
                    return true;
                }
                // 再設定する値を計算
                currentCadTypeEnmu = currentCadTypeEnmu & ~newCadType;
            }



            //二進数を表示
            WriteLine("→変更後のCadType値を二進数表示 " + Convert.ToString((int)currentCadTypeEnmu, 2).PadLeft(8, '0'));

            string currentCadTypeStr = ((int)currentCadTypeEnmu).ToString();

            bool ans2 = MergeArcSuiteAttribute1(arcSuiteZuban, "user:cadtype", currentCadTypeStr, ArcSuiteUser, ArcSuiteUserPass, WriteLine);

            if (ans2 == false)
            {
                WriteLine("→失敗");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 指定したCadTypeが含まれるならtrueを返す
        /// </summary>
        /// <param name="arcSuiteZuban"></param>
        /// <param name="newCadType"></param>
        /// <param name="currentCadTypeEnmu"></param>
        /// <param name="ArcSuiteUser"></param>
        /// <param name="ArcSuiteUserPass"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public bool ContainArcSuiteAttrCADType(string arcSuiteZuban, CadType newCadType, ref CadType currentCadTypeEnmu, string ArcSuiteUser, string ArcSuiteUserPass, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            ArrayList ans;

            ans = GetArcSuiteAttribute(arcSuiteZuban, "user:cadtype");

            if (ans == null)
                return false;

            if (ans.Count != 2)
                return false;

            string user_strCadtype = SasaLib.Csv.GetFromCsvArrayList(ans, "user:cadtype", 1);
            int previous_cadtype;

            if (String.IsNullOrEmpty(user_strCadtype))
                previous_cadtype = 0;
            else
            {
                if (int.TryParse(user_strCadtype, out previous_cadtype) == false)
                    return false;
            }

            // intをCadType
            currentCadTypeEnmu = (CadType)previous_cadtype;

            WriteLine("ContainArcSuiteAttrCADType(..) →変更前のCadType値を二進数表示 " + Convert.ToString((int)currentCadTypeEnmu, 2).PadLeft(8, '0'));

            WriteLine("ContainArcSuiteAttrCADType(..) →比較するCadType値を二進数表示 " + Convert.ToString((int)newCadType, 2).PadLeft(8, '0'));

            if (currentCadTypeEnmu.HasFlag(newCadType))
            {
                WriteLine($"ContainArcSuiteAttrCADType(..) →指定した{newCadType}は含まれます");
                return true;
            }
            else
            {
                WriteLine($"ContainArcSuiteAttrCADType(..) →指定した{newCadType}は含まれません");
                return false;
            }
        }

        /// <summary>
        /// アークスイートの属性値を取得（最新版フラグがTRUEのみ）
        /// </summary>
        /// <param name="PipeServerName"></param>
        /// <param name="drawregist_pipename"></param>
        /// <param name="ZUBAN"></param>
        /// <param name="Attrstr">検索結果表示属性設定CSV</param>
        /// <returns></returns>
        public ArrayList GetArcSuiteAttribute(string ZUBAN, string Attrstr, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            ArrayList result = null;

            using (new ClsLogonDummy(ClientDomainName, ClientUserName, ClientUserPassword, ClsLogon))
            {

                NamedPipeClientStream pipeClientst = new NamedPipeClientStream(this.PipeServerName, this.PipeNameDR);

                // 待機中のサーバーへ接続
                try
                {
                    pipeClientst.Connect(ClientTimeOut);

                    ArrayList AnsArrayList = new ArrayList();

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeClientst);

                    // ①接続文字の受信とチェック
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"RemoteClientCADtype.GetArcSuiteAttribute(..) PIPE接続失敗。ハンドシェイクでタイムアウト");
                        return null;
                    }
                    if (CheckFirstMessage(input0))
                    {
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtribute1);

                        // ③検索図面番号送信
                        stst.WriteString(ZUBAN);

                        // ④検索結果表示属性設定CSVの記述を送信
                        stst.WriteString(Attrstr);

                        // ⑤サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeClientst, Encoding.UTF8, true))
                        {
                            var anser = reader.ReadObject<ArrayList>();
                            AnsArrayList = anser;
                        }
                    }
                    else
                    {
                        WriteLine($"※エラー RemoteClientCADtype.GetArcSuiteAttribute(...)：サーバーからの接続回答が期待したものと違います {input0}");
                        pipeClientst.Close();
                        return null;
                    }

                    pipeClientst.Close();

                    result = AnsArrayList;
                }
                catch (Exception ex)
                {
                    WriteLine($"※エラー RemoteClientCADtype.GetArcSuiteAttribute(...)：PIPEサーバー接続エラー {ex.Message}");
                    try
                    {
                        pipeClientst.Close();
                        result = null;
                    }
                    catch
                    {
                        WriteLine($"※エラー RemoteClientCADtype.GetArcSuiteAttribute(...)：PIPEサーバークローズに失敗しています　 return nullを実行");
                        result = null;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ■アークスイートのCADType属性値を検索します。構造体の参照渡しで結果を返します。
        /// </summary>
        /// <param name="PipeServerName"></param>
        /// <param name="drawregist_pipename"></param>
        /// <param name="arcSuiteZuban"></param>
        /// <param name="ArcSuiteAttr"></param>
        /// <returns></returns>
        public bool GetArcSuiteAttrCADType(string arcSuiteZuban, ref ArcSuiteAttr ArcSuiteAttr, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            WriteLine($"GetArcSuiteAttrCADType(..) , PipeServerName:{PipeServerName}, PIPENAME:{PipeNameDR}, 検索図面番号:{arcSuiteZuban}");

            ArrayList ans;

            ans = GetArcSuiteAttribute(arcSuiteZuban, "user:zuban|user:torokubi|user:cadtype", WriteLine);

            if (ans == null)
                return false;
            if (ans.Count != 2)
                return false;

            string user_strZuban = SasaLib.Csv.GetFromCsvArrayList(ans, "user:zuban", 1);
            string user_strTorokubi = SasaLib.Csv.GetFromCsvArrayList(ans, "user:torokubi", 1);
            string user_strCadtype = SasaLib.Csv.GetFromCsvArrayList(ans, "user:cadtype", 1);

            int previous_cadtype;

            if (String.IsNullOrEmpty(user_strCadtype))
                previous_cadtype = 0;
            else
            {
                if (int.TryParse(user_strCadtype, out previous_cadtype) == false)
                    return false;
            }


            ArcSuiteAttr.ZUBAN = user_strZuban;
            ArcSuiteAttr.TOROKUBI = user_strTorokubi;
            ArcSuiteAttr.CadType = (CadType)previous_cadtype;

            WriteLine($"GetArcSuiteAttrCADType(..), {ArcSuiteAttr.ZUBAN} →登録日:{ArcSuiteAttr.TOROKUBI} CadType={ArcSuiteAttr.CadType}");
            WriteLine("GetArcSuiteAttrCADType(..), →CadType値を二進数表示 " + Convert.ToString((int)ArcSuiteAttr.CadType, 2).PadLeft(8, '0'));

            return true;
        }

        /// <summary>
        /// ■アークスイートの属性値を変更
        /// </summary>
        /// <param name="PipeServerName">このコマンドを受け付けるサーバー名</param>
        /// <param name="drawregist_pipename">接続パイプ名</param>
        /// <param name="Zuban">アークスイートでの図面番号</param>
        /// <param name="ArcSuiteAttrField">変更対象のユーザー属性 user:type 等</param>
        /// <param name="ArcSuiteAttrValue">属性値</param>
        /// <param name="ArcSuiteUserID">アークスイートユーザID（変更権利のあるユーザであること）</param>
        /// <param name="ArcSuiteUserPassword">アークスイートユーザーパスワード</param>
        /// <returns></returns>
        public bool MergeArcSuiteAttribute1(string Zuban, string ArcSuiteAttrField, string ArcSuiteAttrValue, string ArcSuiteUserID, string ArcSuiteUserPassword, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            NamedPipeClientStream pipeClientst = new NamedPipeClientStream(PipeServerName, PipeNameDR);

            // 待機中のサーバーへ接続
            try
            {
                pipeClientst.Connect(ClientTimeOut);

                bool result = false;

                // サーバーからの書き込みを受け取ります。
                StreamString stst = new StreamString(pipeClientst);

                // ①接続文字の受信とチェック
                bool OperationCanceledException;
                bool AggregateException;

                string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                if (OperationCanceledException || AggregateException)
                {
                    SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"RemoteClientCADtype.MergeArcSuiteAttribute1(..) PIPE接続失敗。ハンドシェイクでタイムアウト");
                    return false;
                }
                if (CheckFirstMessage(input0))
                {
                    // ②コマンド名送信
                    int writeResult = stst.WriteString(CMDS.DR_MergeArcSuiteAtrtribute1);

                    // ③検索図面番号送信
                    stst.WriteString(Zuban);

                    //  ④対象のArcSuiteユーザー属性名を送信
                    stst.WriteString(ArcSuiteAttrField);

                    //  ⑤属性値を送信
                    stst.WriteString(ArcSuiteAttrValue);

                    // ⑥変更を実行するアークスイートユーザーIDを送信
                    stst.WriteString(ArcSuiteUserID);

                    // ⑦パスワードを送信
                    stst.WriteString(ArcSuiteUserPassword);

                    // サーバーから結果を受信
                    using (BinaryReader reader = new BinaryReader(pipeClientst, Encoding.UTF8, true))
                    {
                        result = reader.ReadObject<bool>();
                    }
                }
                else
                {
                    WriteLine($"※エラー：MergeArcSuiteAttribute1() サーバーからの接続回答が期待したものと違います {input0}");
                    pipeClientst.Close();
                    return false;
                }
                pipeClientst.Close();
                //
                return result;
            }
            catch (Exception ex)
            {
                WriteLine($"※エラー：MergeArcSuiteAttribute1() 例外検知 {ex.Message} {ex.StackTrace}");
                pipeClientst.Close();
                return false;
            }
        }

        /// <summary>
        /// ■指定したArcSuite属性 の値をクリアする
        /// </summary>
        /// <param name="PipeServerName"></param>
        /// <param name="drawregist_pipename"></param>
        /// <param name="arcSuiteZuban"></param>
        /// <param name="ArcSuiteAttrField"></param>
        /// <returns></returns>
        public bool ArcSuiteAttributeSetEmpty(string arcSuiteZuban, string ArcSuiteAttrField,
                                                string ArcSuiteUserID, string ArcSuiteUserPassword, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            string arcSuiteAttrField = ArcSuiteAttrField;
            string user_attributeValue = null;


            ArrayList ans2 = GetArcSuiteAttribute(arcSuiteZuban, arcSuiteAttrField);
            if (ans2.Count == 2)
            {
                user_attributeValue = SasaLib.Csv.GetFromCsvArrayList(ans2, arcSuiteAttrField, 1);

                if (string.IsNullOrWhiteSpace(user_attributeValue) == true)
                {
                    WriteLine("ArcSuiteAttributeSetEmpty(..) 属性値は空です");
                    return true;
                }
                else
                {
                    string attributeValue = @"\0";
                    CadType resultarc = CadType.NotSet;

                    bool result = MergeArcSuiteAttribute1(arcSuiteZuban, arcSuiteAttrField, attributeValue, ArcSuiteUserID, ArcSuiteUserPassword);

                    if (result)
                    {
                        bool cmdans = SetUnSetArcSuiteAttrCADType(arcSuiteZuban, CadType.InventorModel, true, ref resultarc, ArcSuiteUserID, ArcSuiteUserPassword);
                        if (cmdans)
                        {

                            if (resultarc != CadType.NotSet)
                            {
                                WriteLine($"ArcSuiteAttributeSetEmpty(..) {arcSuiteZuban} ArcSuite側属性値\"{user_attributeValue}\"削除に成功 CadType={resultarc}");
                                return true;
                            }
                            else
                            {

                                WriteLine($"ArcSuiteAttributeSetEmpty(..) {arcSuiteZuban} ArcSuite側属性値\"{user_attributeValue}\"削除に成功  CadType={resultarc}");
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        WriteLine($"ArcSuiteAttributeSetEmpty(..) {arcSuiteZuban} 更新失敗. ArcSuite側属性値 \"{user_attributeValue}\"");
                        return false;
                    }
                }
            }
            else
                return false;
        }

        /// <summary>
        /// サーバーから返答された識別文字列をチェックする
        /// </summary>
        /// <param name="input0">サーバーからの文字列を指定</param>
        /// <returns></returns>
        bool CheckFirstMessage(string input0, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            if (input0 == CMDS.ConnectKeyword)
                return true;
            else if (input0 == @"BUSY")
            {
                WriteLine("サーバーが混んでいます。しばらくお待ちください");
                return false;
            }
            else
            {
                WriteLine($"このクライアントはサーバーが要求するものとは違います\n{input0}");
                return false;
            }
        }
    }
}

