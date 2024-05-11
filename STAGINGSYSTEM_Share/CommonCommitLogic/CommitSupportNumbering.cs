using CommonCommitLogic;
using SasaLib.ArcSuitePreview;
using SasaLib.NumberingSupport;
using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace CommonCommitLogic
{
    [SupportedOSPlatform("windows")]
    public class CommitSupportNumbering
    {

        /// <summary>
        /// 
        /// </summary>
        CommitParam commitParam;

        /// <summary>
        /// 
        /// </summary>
        SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ReserveNumber> ReserveNumberChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<TypeNumber> TypeNumberChanged;


        /// <summary>
        /// 
        /// </summary>
        private ReserveNumber _reserveNumber;


        /// <summary>
        /// 
        /// </summary>
        public ReserveNumber ReserveNumber
        {
            get { return _reserveNumber; }
            set
            {
                if (_reserveNumber != value)
                {
                    _reserveNumber = value;
                    OnReserveNumberChanged(ReserveNumber);
                }

            }
        }

        /// <summary>
        ///
        /// </summary>
        private TypeNumber _typeNumber;

        /// <summary>
        /// 
        /// </summary>
        public TypeNumber TypeNumber
        {
            get { return _typeNumber; }
            set
            {
                if (_typeNumber != value)
                {
                    _typeNumber = value;
                    OnTypeNumberChanged(TypeNumber);
                }

            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="commitParam"></param>
        /// <param name="WriteLine"></param>
        public CommitSupportNumbering(CommitParam commitParam, SasaLibDelegateWriteLine WriteLine)
        {
            this.commitParam = commitParam;
            this.WriteLine = WriteLine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnReserveNumberChanged(ReserveNumber e)
        {
            ReserveNumberChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnTypeNumberChanged(TypeNumber e)
        {
            TypeNumberChanged?.Invoke(this, e);
        }



        /// <summary>
        /// 採番システムへ採番状況を問い合わせる
        /// </summary>
        /// <param name="_partnumber"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        internal async void CheckReserveNumber(string _partnumber, IWin32Window owner)
        {

            bool CheckAcquiredNumberedNormal;
            bool NumberingRecordAvailable;
            string CheckNumberdHistoryAnser;

            /// 採番システムが正常動作ならtrue 
            CheckAcquiredNumberedNormal = false;
            /// 採番実績有無
            NumberingRecordAvailable = false;
            /// 採番実績有無検索結果の文字列
            CheckNumberdHistoryAnser = null;

            //採番システムへ問い合わせ
            WriteLine($"■CommitSupportNumbering.CheckReserveNumber(..) 採番システム【{commitParam.NumberingServerName}】へ【{_partnumber}】の採番実績を問い合わせています");

            Encryption sasaLibEncryptionNumbering = new Encryption("SasaAuth3.1");
            string NumberngConnectionUserPlanePass = sasaLibEncryptionNumbering.Decoding(commitParam.NumberingServerDB_Crypt31Password); //復号化

            //NumberingSupport numberingSupport = new NumberingSupport(
            //    commitParam.NumberingServerName, commitParam.NumberingServerDbPort,  // 採番サーバーホスト名、採番サービスポート番号
            //    commitParam.NumberingServerDB_Username, NumberngConnectionUserPlanePass); //採番サーバー接続ﾕｰｻﾞｰ名, 採番サーバ接続ﾕｰｻﾞｰパスワード

            await Task.Run(() =>
            {
                try
                {
                    //CheckAcquiredNumberedNormal = numberingSupport.CheckAcquiredNumbered(_partnumber, out NumberingRecordAvailable, out CheckNumberdHistoryAnser, WriteLine);
                    CheckAcquiredNumberedNormal = CheckAcquiredNumbered(commitParam.NumberingServerName, commitParam.NumberingServerDbPort, commitParam.NumberingServerDB_Username, NumberngConnectionUserPlanePass, _partnumber, out NumberingRecordAvailable, out CheckNumberdHistoryAnser, WriteLine);

                    WriteLine($"■CommitSupportNumbering.CheckReserveNumber(..) 採番システムへの問い合わせ結果→{NumberingRecordAvailable}, {CheckNumberdHistoryAnser}");
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("CommonCommitLogic", EventLogEntryType.Error, 0, $"※CheckArcSuiteRegistedで例外{ex.Message}");
                }
                ReserveNumber = new ReserveNumber() { commitTarget_partnumber = _partnumber, CheckAcquiredNumberedNormal = CheckAcquiredNumberedNormal, NumberingRecordAvailable = NumberingRecordAvailable, CheckNumberdHistoryAnser = CheckNumberdHistoryAnser };
            });

        }

        /// <summary>
        /// 図番から図面種類を調査
        /// </summary>
        /// <param name="_partnumber"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        internal bool CheckNumberType(string _partnumber, IWin32Window owner)
        {
            bool result = true;

            bool isVariant = false;
            string VariantSuffixMIN;
            string VariantSuffixMAX;
            string TypeName;

            // ステージサーバーへ図面番号が妥当かを確認する
            NumberTypeConfig.DrawingType drawingType;
            VariantSuffixMIN = null; ;
            VariantSuffixMAX = null; ;
            if (ToyoDrawingTypeClassify.CheckNumber(_partnumber, out drawingType, ref isVariant, ref VariantSuffixMIN, ref VariantSuffixMAX, out TypeName, WriteLine,verbose:false) == false)
            {
                MessageBox.Show(owner, 
                    $"図面番号 \"{_partnumber}\" を図面種類 \"{TypeName}\" として認識しました。現在対応していない図番形式です。\n" +
                    $"もしくは全角文字が混ざっていたり末尾に余分なスペースの存在が考えられます。\n" +
                    $"図番を修正するか、今までにない番号形式の場合はＣＡＤシステム管理チームにご連絡お願いします", "※コミットシステム 警告・対処が必要", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                WriteLine($"※CommitSupportNumbering.CheckNumberType(..) コミットシステム警告 図面番号 \"{_partnumber}\" は現在対応していない図番形式です図番を修正するか、今までにない番号形式の場合はＣＡＤシステム管理チームにご連絡お願いします");
                result = false;
            }
            else
            {
                WriteLine($"■CommitSupportNumbering.CheckNumberType(..) 図面番号 \"{_partnumber}\" , コミットサーバー {commitParam.StageServerHost} / PIPE名 {commitParam.PipeNameDC} 認識した図面種類は『{TypeName}』です。");
            }

            if (string.IsNullOrWhiteSpace(TypeName))
            {
                MessageBox.Show(owner, $"図面種類の識別に失敗","■東陽ｱﾄﾞｲﾝ・エラー",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                WriteLine($"※CommitSupportNumbering.CheckNumberType(..) 図面種類の識別に失敗 PARTNUMBER = \"{_partnumber}\",  情報 StageServerHost={commitParam.StageServerHost} PIPEnameDC={commitParam.PipeNameDC} DomainName={commitParam.ClientDomainName} UserName={commitParam.ClientUserName}, ClsLogon={commitParam.ClsLogon}");
                result = false;
            }

            TypeNumber = new TypeNumber() { commitTarget_partnumber = _partnumber, drawingType = drawingType, TypeName = TypeName, isVariant = isVariant, VariantSuffixMIN = VariantSuffixMIN, VariantSuffixMAX = VariantSuffixMAX };

            return result;
        }

        /// <summary>
        /// 採番システムに採番実績を問い合わせる
        /// </summary>
        /// <param name="PARTNUMBER">問い合わせる</param>
        /// <param name="findMessage">結果が文字列として返る</param>
        /// <returns>正常動作ならtrue</returns>
        public bool CheckAcquiredNumbered(string NumberingServerName, int NumberingServeMySqlPortNumber,
            string NumberingServerConnectUser, string NumberngConnectionUserPlanePass,
            string PARTNUMBER, out bool NumberingRecordAvaliableFinulAnser, out string findMessage, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            try
            {
                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

                /// データベース接続可否（正常接続か？）
                bool dbConnectNormal = false;

                bool _partNumberingRecordAvaliable = false;
                bool _partWithoutRLNumberingRecordAvaliable = false;
                string partAnserMessage = null;
                string partWitoutRLAnserMessage = null;

                bool _kumizuNumberingRecordAvaliable = false;
                string kumizuAnzser = null;

                bool _layoutNumberingRecordAvaliable = false;
                string layoutAnser = null;

                string PREF_NAME;
                string PREF_RL;

                NumberingRecordAvaliableFinulAnser = false;
                findMessage = null;

                if (SasaLib.Net.CheckPing(NumberingServerName) == false)
                {
                    delegateWriteLine($"※CheckAcquiredNumbered(..) 採番ｻｰﾊﾞｰ{NumberingServerName} はPINGに応答しませんでした。falseで抜けます");
                    return false;
                }

                SplitPARTNUMBERtoRL(PARTNUMBER, out PREF_NAME, out PREF_RL);

                SasaLibMySqlConnect MysqlDB_buhinzu_Name = new SasaLibMySqlConnect(NumberingServerName, NumberingServeMySqlPortNumber, "db_buhinzu", "utf8", NumberingServerConnectUser, NumberingServerConnectUser);
                SasaLibMySqlConnect MysqlDB_kumizu_Name = new SasaLibMySqlConnect(NumberingServerName, NumberingServeMySqlPortNumber, "db_kumizu", "utf8", NumberingServerConnectUser, NumberingServerConnectUser);
                SasaLibMySqlConnect MysqlDB_layout_Name = new SasaLibMySqlConnect(NumberingServerName, NumberingServeMySqlPortNumber, "db_layout", "utf8", NumberingServerConnectUser, NumberingServerConnectUser);


                dbConnectNormal = MysqlDB_buhinzu_Name.DrawingPartNumberFindFirst("t01prefecture", PREF_NAME, PREF_RL, out _partNumberingRecordAvaliable, out partAnserMessage);

                if (_partNumberingRecordAvaliable) { partAnserMessage = "部品図(図番完全一致)として" + partAnserMessage; }

                if (dbConnectNormal)
                    delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン① 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続成功 PREF_NAME(ゼロ補完なし)(ラストのRLを除去した文字列)=【{PREF_NAME}】,PREF_RL=【{PREF_RL}】検索結果【{partAnserMessage}】");
                else
                    delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン① 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続失敗 PREF_NAME(ゼロ補完なし)(ラストのRLを除去した文字列)=【{PREF_NAME}】,PREF_RL=【{PREF_RL}】");

                if (_partNumberingRecordAvaliable == false)
                {
                    string ZeroPaddingdPREF_NAME = SanitaizingSimplificationString(PREF_NAME);

                    dbConnectNormal = MysqlDB_buhinzu_Name.DrawingPartNumberFindFirst("t01prefecture", ZeroPaddingdPREF_NAME, PREF_RL, out _partNumberingRecordAvaliable, out partAnserMessage);

                    if (_partNumberingRecordAvaliable) { partAnserMessage = "部品図(枝番ｾﾞﾛ補完後一致)として" + partAnserMessage; }
                    if (dbConnectNormal)
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン② 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続成功 ZeroPaddingdPREF_NAME(枝番3桁ゼロ補完)(ラストのRLを除去した文字列)=【{ZeroPaddingdPREF_NAME}】PREF_RL=【{PREF_RL}】検索結果【{partAnserMessage}】");
                    else
                        delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン② 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続失敗 ZeroPaddingdPREF_NAME(枝番3桁ゼロ補完)(ラストのRLを除去した文字列)=【{ZeroPaddingdPREF_NAME}】PREF_RL=【{PREF_RL}】");

                } // 検索パターン①でヒットしなかった場合は検索パターン②にて調査

                if (_partNumberingRecordAvaliable == false)
                {
                    dbConnectNormal = MysqlDB_buhinzu_Name.DrawingPartNumberFindFirst("t01prefecture", PARTNUMBER, "", out _partNumberingRecordAvaliable, out partWitoutRLAnserMessage);

                    if (_partNumberingRecordAvaliable) { partAnserMessage = "部品図(枝番ｾﾞﾛ補完後一致/RL無し)として" + partAnserMessage; }
                    if (dbConnectNormal)
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン③ 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続成功 PARTNUMBER(入力値のまま)=【{PARTNUMBER}】,検索結果【{partAnserMessage}】");
                    else
                        delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン③ 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続失敗 PARTNUMBER(入力値のまま)=【{PARTNUMBER}】");
                } // 検索パターン②でヒットしなかった場合は検索パターン③にて調査

                if (_partNumberingRecordAvaliable == false)
                {
                    dbConnectNormal = MysqlDB_buhinzu_Name.DrawingPartNumberFindFirst("t01prefecture", PREF_NAME, "", out _partNumberingRecordAvaliable, out partAnserMessage);

                    if (_partNumberingRecordAvaliable) { partAnserMessage = "部品図(RL無し)として" + partAnserMessage; }
                    if (dbConnectNormal)
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン④ 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続成功 PREF_NAME(ゼロ補完なし)(ラストのRLを除去した文字列)【{PREF_NAME}】,PREF_RL=【】検索結果【{partAnserMessage}】");
                    else
                        delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン④ 部品図・ﾃﾞｰﾀﾍﾞｰｽ sampledb050 に対する接続失敗 PREF_NAME(ゼロ補完なし)(ラストのRLを除去した文字列)【{PREF_NAME}】");
                } // 検索パターン③でヒットしなかった場合は検索パターン④にて調査

                if (_partNumberingRecordAvaliable == false)
                {

                    dbConnectNormal = MysqlDB_kumizu_Name.DrawingAssyNumberFindFirst("t01prefecture", PARTNUMBER, out _kumizuNumberingRecordAvaliable, out kumizuAnzser);

                    if (_partNumberingRecordAvaliable) { partAnserMessage = "組立図(図番完全一致)として" + partAnserMessage; }
                    if (dbConnectNormal)
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑤ 組立図・ﾃﾞｰﾀﾍﾞｰｽ dbkumizu に対する接続成功 PARTNUMBER=【{PARTNUMBER}】 検索結果【{kumizuAnzser}】");
                    else
                        delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑤ 組立図・ﾃﾞｰﾀﾍﾞｰｽ dbkumizu に対する接続失敗 PARTNUMBER=【{PARTNUMBER}】");

                    if (_kumizuNumberingRecordAvaliable == false)
                    {
                        string ZeroPaddingdPARTNUMBER = SanitaizingSimplificationString(PARTNUMBER);

                        dbConnectNormal = MysqlDB_kumizu_Name.DrawingAssyNumberFindFirst("t01prefecture", ZeroPaddingdPARTNUMBER, out _kumizuNumberingRecordAvaliable, out kumizuAnzser);

                        if (_partNumberingRecordAvaliable) { partAnserMessage = "組立図(枝番ｾﾞﾛ保管後一致)として" + partAnserMessage; }
                        if (dbConnectNormal)
                            delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑥ 組立図・ﾃﾞｰﾀﾍﾞｰｽ dbkumizu に対する接続成功 ZeroPaddingdPARTNUMBER=【{ZeroPaddingdPARTNUMBER}】検索結果【{kumizuAnzser}】");
                        else
                            delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑥ 組立図・ﾃﾞｰﾀﾍﾞｰｽ dbkumizu に対する接続失敗 ZeroPaddingdPARTNUMBER=【{ZeroPaddingdPARTNUMBER}】");
                    }
                } // 部品図として見つからなかった場合組立図として検索する

                if (_partNumberingRecordAvaliable == false && _kumizuNumberingRecordAvaliable == false)
                {

                    dbConnectNormal = MysqlDB_layout_Name.DrawingLayoutNumberFindFirst("t01prefecture", PARTNUMBER, out _layoutNumberingRecordAvaliable, out layoutAnser);

                    if (_partNumberingRecordAvaliable) { partAnserMessage = "ﾚｲｱｳﾄ図(図番完全一致)として" + partAnserMessage; }
                    if (dbConnectNormal)
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑦ ﾚｲｱｳﾄ・ﾃﾞｰﾀﾍﾞｰｽ dblayout に対する接続成功 【{dbConnectNormal}】【{PARTNUMBER}】検索結果 【{layoutAnser}】");
                    else
                        delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() 採番システム：検索パターン⑦ ﾚｲｱｳﾄ・ﾃﾞｰﾀﾍﾞｰｽ dblayout に対する接続失敗 【{dbConnectNormal}】【{PARTNUMBER}】");
                } //部品図としても組立図としても見つからなかった場合は技術図書図として検索する

                if (dbConnectNormal)
                {
                    if (_partNumberingRecordAvaliable == true)
                    {
                        NumberingRecordAvaliableFinulAnser = true;
                        findMessage = partAnserMessage;
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：部品図です： {findMessage}");
                        return true;
                    }
                    else if (_partWithoutRLNumberingRecordAvaliable == true)
                    {
                        NumberingRecordAvaliableFinulAnser = true;
                        findMessage = partWitoutRLAnserMessage;
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：部品図です(RL指定をPREF__NAME側に含めることでヒット)： {findMessage}");
                        return true;

                    }
                    else if (_kumizuNumberingRecordAvaliable == true)
                    {
                        NumberingRecordAvaliableFinulAnser = true;
                        findMessage = kumizuAnzser;
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：組立図です： {findMessage}");
                        return true;
                    }
                    else if (_layoutNumberingRecordAvaliable == true)
                    {
                        NumberingRecordAvaliableFinulAnser = true;
                        findMessage = layoutAnser;
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：技術図書図です： {findMessage}");
                        return true;
                    }
                    else
                    {
                        NumberingRecordAvaliableFinulAnser = false;
                        findMessage = $"{PARTNUMBER}の採番実績がありませんでした";
                        delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：{findMessage}");
                        return true;
                    }
                }
                else
                {
                    NumberingRecordAvaliableFinulAnser = true;
                    findMessage = "注意：採番システムに接続できませんでした";
                    delegateWriteLine($"■NumberingSupport.CheckAcquiredNumbered() 採番システム上での図面種類判定結果：{findMessage}");
                    return false;
                }

            }
            catch (Exception ex)
            {
                delegateWriteLine($"※NumberingSupport.CheckAcquiredNumbered() にて例外検知{ex.Message} {ex.InnerException}");
                NumberingRecordAvaliableFinulAnser = false;
                findMessage = null;
                return false;
            }
        }

        /// <summary>
        /// 部品番号からRL,R,L を分離し、PREF_NAME　PREF_RLに戻す。
        /// </summary>
        /// <param name="PARTNUMBER"></param>
        /// <param name="PREF_NAME"></param>
        /// <param name="PREF_RL"></param>
        private void SplitPARTNUMBERtoRL(string PARTNUMBER, out string PREF_NAME, out string PREF_RL)
        {
            string withoutRL = Regex.Replace(PARTNUMBER, $"(RL)$", "", RegexOptions.IgnoreCase);
            string withooutR = Regex.Replace(PARTNUMBER, $"(R)$", "", RegexOptions.IgnoreCase);
            string withouutL = Regex.Replace(PARTNUMBER, $"(L)$", "", RegexOptions.IgnoreCase);

            if (PARTNUMBER != withoutRL || PARTNUMBER != withooutR || PARTNUMBER != withouutL)
            {
                if (withoutRL != null)
                {
                    PREF_NAME = withoutRL;
                    PREF_RL = "RL";
                }
                else if (withooutR != null)
                {
                    PREF_NAME = withooutR;
                    PREF_RL = "R";
                }
                else if (withouutL != null)
                {
                    PREF_NAME = withouutL;
                    PREF_RL = "L";
                }
                else
                {
                    PREF_NAME = PARTNUMBER;
                    PREF_RL = null;
                }
            }
            else
            {
                PREF_NAME = PARTNUMBER;
                PREF_RL = null;
            }
        }

        /// <summary>
        /// サニタイズ関数（空白を削除したのち、全角を半角にする。（半角ｶﾅは使用しない）最後に、ハイフンで区切られた3項目目をゼロ保管処理）
        /// </summary>
        /// <param name="orgValue"></param>
        /// <returns></returns>
        private string SanitaizingSimplificationString(string orgValue)
        {
            string sanitaizedValue;
            bool result = ArcSuiteSupport.ZeroPaddingPartNumber(orgValue, out sanitaizedValue);
            return sanitaizedValue;
        }

    }
}
