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

namespace CommonCommitLogic
{
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

            NumberingSupport numberingSupport = new NumberingSupport(
                commitParam.NumberingServerName, commitParam.NumberingServerDbPort,  // 採番サーバーホスト名、採番サービスポート番号
                commitParam.NumberingServerDB_Username, NumberngConnectionUserPlanePass); //採番サーバー接続ﾕｰｻﾞｰ名, 採番サーバ接続ﾕｰｻﾞｰパスワード

            await Task.Run(() =>
            {
                try
                {
                    CheckAcquiredNumberedNormal = numberingSupport.CheckAcquiredNumbered(_partnumber, out NumberingRecordAvailable, out CheckNumberdHistoryAnser, WriteLine);

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

    }
}
