//using CommonCommitLogic;
//using SasaLib.ArcSuitePreview;
//using SasaLib.NumberingSupport;
//using SasaLib;
//using StageServerRemote;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Diagnostics.Eventing.Reader;
//using static CommonCommitLogic.ArcSuiteSearchResult;
//using System.Runtime.Versioning;

//namespace CommonCommitLogic
//{
//    [SupportedOSPlatform("windows")]
//    public class CommitSupportArcSuite
//    {

//        /// <summary>
//        /// 
//        /// </summary>
//        CommitParam commitParam;

//        /// <summary>
//        /// 
//        /// </summary>
//        SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;



//        public event EventHandler<ArcSuiteSearchResult> ArcSuiteSearchResultChanged;

//        private ArcSuiteSearchResult _ArcSuiteSearchResult;

//        public ArcSuiteSearchResult ArcSuiteSearchResult
//        {
//            get { return _ArcSuiteSearchResult; }
//            set
//            {
//                if (_ArcSuiteSearchResult != value)
//                {
//                    _ArcSuiteSearchResult = value;
//                    OnArcSuiteSearchResult(ArcSuiteSearchResult);
//                }
//            }

//        }

//        /// <summary>
//        /// コンストラクタ
//        /// </summary>
//        /// <param name="commitParam"></param>
//        /// <param name="WriteLine"></param>
//        internal CommitSupportArcSuite(CommitParam commitParam, SasaLibDelegateWriteLine WriteLine)
//        {
//            this.commitParam = commitParam;
//            this.WriteLine = WriteLine;
//        }

//        private void OnArcSuiteSearchResult(ArcSuiteSearchResult e)
//        {
//            ArcSuiteSearchResultChanged?.Invoke(this, e);
//        }

//        /// <summary>
//        /// ｱｰｸｽｲｰﾄに図面があるか確認する。データﾀﾞｳﾝﾛｰﾄﾞはしません
//        /// </summary>
//        internal async void CheckArcSuiteRegisted(string target_partNumber, string target_description, List<string> suffixs, CancellationToken ct)
//        {
//            // ゼロ補完を先に実行
//            string SanitaizingSimplificationString = ArcSuiteSupport.SanitaizingSimplificationStringForToyoDRAWINGNumber(target_partNumber,WriteLine);

//            var neareyPartNumberSarchKey = Create_TOYODRAWINGNUMBERFORMAT_SearchString(SanitaizingSimplificationString, suffixs); // 枝番末尾の指定文字列を削除後、ワイルドカード付与
//            var neareyPartNumberSarchKey_string = string.Join(", ", neareyPartNumberSarchKey.Select(item => $"\"{item}\""));

//            WriteLine($"■CommitSupportArcSuite.CheckArcSuiteRegisted(..) ArcSuite図面の検索：ｽﾃｰｼﾞｻｰﾊﾞｰ {commitParam.StageServerHost} 経由でアークスイートへ図面 {SanitaizingSimplificationString} を含む近似番号として 【{neareyPartNumberSarchKey_string}】 を検索を確認します。");


//            Encryption sasaLibencryptionPipeConnection = new Encryption("SasaAuth3.1");
//            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commitParam.PipeConnection31Password); //復号化

//            List<ArcsuitePreview> arcSuitePreviews = null;

//            ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.Null;

//            await Task.Run(() =>
//            {
//                try
//                {

//                    /// リモート操作をするオブジェクトを生成
//                    RemoteClientDRAWREGIST remoteClientDR = new RemoteClientDRAWREGIST(
//                        commitParam.ClientDomainName,
//                        commitParam.ClientUserName,
//                        PipeClientPlanePass,
//                        commitParam.ClsLogon,
//                        commitParam.StageServerHost,
//                        commitParam.PipeNameDR
//                        );

//                    ///ｱｰｸｽｲｰﾄへ問合せを実行する。時間コスト高い処理
//                    CheckArcSuiteData arcSuiteDuplicateConfirm = new CheckArcSuiteData(remoteClientDR);




//                    // ArcSuite検索 時間のかかる処理
//                    arcSuitePreviews = arcSuiteDuplicateConfirm.QueryStart(neareyPartNumberSarchKey, ct, WriteLine);


//                }
//                catch (Exception ex)
//                {
//                    SasaLib.Eventlog.Log.WriteEntry("CommonCommitLogic", EventLogEntryType.Error, 0, $"※CheckArcSuiteRegistedで例外{ex.Message}");
//                }
//            });


//            if (arcSuitePreviews != null)
//            {
//                if (arcSuitePreviews.Count == 0)
//                {
//                    hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.ArcSuiteに同番図面なし類番図面もなし;

//                } // 検索結果がゼロ件の場合
//                else
//                {
//                    if (arcSuitePreviews[0].Found == true)
//                    {
//                        ///検索しようとした図番
//                        bool containSearchPartNumber = arcSuitePreviews.Any(x => x.user_zuban.ToUpper() == SanitaizingSimplificationString.ToUpper());

//                        if ((arcSuitePreviews.Count == 1) && (containSearchPartNumber == true))
//                        {
//                            hanteikekka = Check_OneResultSameSearchTarget(target_description, arcSuitePreviews[0].user_description);

//                        } // 検索結果が１件のみ存在する場合 コミット対象が含まれる場合
//                        else if ((arcSuitePreviews.Count == 1) && (containSearchPartNumber == false))
//                        {
//                            hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面なし類番図面あり;

//                        } // 検索結果が１件のみ存在する場合 コミット対象が含まれない場合
//                        else if ((arcSuitePreviews.Count > 1) && (containSearchPartNumber == true))
//                        {
//                            hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果複数ArcSuiteに同番図面あり類番図面あり;

//                        } // 検索結果が複数存在する場合 コミット対象が含まれない場合
//                        else if ((arcSuitePreviews.Count > 1) && (containSearchPartNumber == false))
//                        {
//                            hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果複数ArcSuiteに同番図面なし類番図面があり;
//                        } // 検索結果が複数存在する場合 コミット対象が含まれない場合
//                        else
//                        {
//                            hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.エラー;
//                            throw new Exception($"システム想定外  arcSuitePreviews.Count={arcSuitePreviews.Count} と containSearchPartNumber = {containSearchPartNumber}");
//                        } // システム想定外
//                    }
//                    else
//                    {
//                        hanteikekka = DESCRIPTION_ComparResult_MessagetypeEnum.エラー;
//                        throw new Exception($"システム想定外  検索結果１件なのに、arcSuitePreviews[0].Found が false");
//                    } // システム想定外
//                }

//                WriteLine($"■CommitSupportArcSuite.CheckArcSuiteRegisted(..) ArcSuite図面の検索：成功・結果： \"{hanteikekka}\"");

//                ArcSuiteSearchResult = new ArcSuiteSearchResult()
//                {
//                    commitTarget_partnumber = SanitaizingSimplificationString,
//                    commitTarget_Description = target_description,
//                    searchTargets = neareyPartNumberSarchKey,
//                    arcSuitePreviews = arcSuitePreviews,
//                    dESCRIPTION_ComparResult_MessagetypeEnum = hanteikekka
//                };

//            } // ArcSuiteの接続に成功している場合 
//            else
//            {
//                WriteLine($"※CommitSupportArcSuite.CheckArcSuiteRegisted(..) ArcSuite図面の検索：失敗・結果： \"{hanteikekka}\"");

//                ArcSuiteSearchResult = null;
//            }// ArcSuiteの接続に失敗している場合
//        }

//        ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum Check_OneResultSameSearchTarget(string commitTarget_Descripiton, string arcSuite_Description)
//        {
//            WriteLine($"一致・非一致確認前の強制変更 DatabaseInsert.");

//            // 
//            var orginal_commitTarget_Descripiton = commitTarget_Descripiton;
//            commitTarget_Descripiton = commitTarget_Descripiton.Replace(" ", "").Replace("　", "").Zen2HanANK();
//            WriteLine($"一致・非一致確認前の強制変更(空白を削除したのち、全角を半角にする。半角ｶﾅは使用しない：commitTarget_Descripiton 変更前： \"{orginal_commitTarget_Descripiton}\" 変更後： \"{commitTarget_Descripiton}\"");
//            // 
//            var oriinal_arcSuite_Description = arcSuite_Description;
//            arcSuite_Description = arcSuite_Description.Replace(" ", "").Replace("　", "").Zen2HanANK();
//            WriteLine($"一致・非一致確認前の強制変更(空白を削除したのち、全角を半角にする。半角ｶﾅは使用しない：arcSuite_Description 変更前： \"{oriinal_arcSuite_Description}\" 変更後： \"{arcSuite_Description}\"");



//            //非常手段。全角長音記号はすべて半角のハイフンに置換 , 全角'・'は半角'-'へ置換 してから比較
//            var orginal1_commitTarget_Descripiton = commitTarget_Descripiton;
//            commitTarget_Descripiton = commitTarget_Descripiton.Replace("ー", "-").Replace("・", "-");
//            WriteLine($"一致・非一致確認前の強制変更(全角長音記号はすべて半角のハイフンに置換 , 全角'・'は半角'-'へ置換)：commitTarget_Descripiton 変更前： \"{orginal1_commitTarget_Descripiton}\" 変更後： \"{commitTarget_Descripiton}\"");

//            var oriinal2_arcSuite_Description = arcSuite_Description;
//            arcSuite_Description = arcSuite_Description.Replace("ー", "-").Replace("・", "-");
//            WriteLine($"一致・非一致確認前の強制変更(全角長音記号はすべて半角のハイフンに置換 , 全角'・'は半角'-'へ置換)：arcSuite_Description 変更前： \"{oriinal2_arcSuite_Description}\" 変更後： \"{arcSuite_Description}\"");



//            ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum result = DESCRIPTION_ComparResult_MessagetypeEnum.Null;


//            if ((string.IsNullOrWhiteSpace(commitTarget_Descripiton) == false) && (string.IsNullOrWhiteSpace(arcSuite_Description) == false))
//            {
//                if (commitTarget_Descripiton != arcSuite_Description)
//                {
//                    result = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題は相違;
//                    WriteLine($"■CommitSupportArcSuite.Check_OneResultSameSearchTarget(..) ｱｰｸｽｲｰﾄに同番の図面が１件のみありました。しかし名称が違います ｺﾐｯﾄする側：\"{commitTarget_Descripiton}\" ArcSuite側：\"{arcSuite_Description}\"");
//                }
//                else
//                {
//                    result = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題は合致;
//                    WriteLine($"■CommitSupportArcSuite.Check_OneResultSameSearchTarget(..) ｱｰｸｽｲｰﾄに同番の図面が１件のみありました。名称も一致しました");
//                }

//            }
//            else if ((string.IsNullOrWhiteSpace(commitTarget_Descripiton) == false) && (string.IsNullOrWhiteSpace(arcSuite_Description) == true))
//            {
//                result = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がArcSuite側に無し;
//            }
//            else if ((string.IsNullOrWhiteSpace(commitTarget_Descripiton) == true) && (string.IsNullOrWhiteSpace(arcSuite_Description) == false))
//            {
//                result = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がCAD側に無し;
//            }
//            else if ((string.IsNullOrWhiteSpace(commitTarget_Descripiton) == true) && (string.IsNullOrWhiteSpace(arcSuite_Description) == true))
//            {
//                result = DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がはどちらも無し;
//            }
//            else
//            {
//                result = DESCRIPTION_ComparResult_MessagetypeEnum.エラー;

//                WriteLine($"※CommitSupportArcSuite.Check_OneResultSameSearchTarget(..) ｱｰｸｽｲｰﾄに同番の図面が１件のみありました。 ArcSuite側から表題は読み取れません");
//            }

//            return result;
//        }


//        /// <summary>
//        /// ■指定された図面番号から考えられるサフィックスを追加した検索用図面番号のコレクションを組立・返します。。
//        /// </summary>
//        /// <param name="input"></param>
//        /// <param name="suffixList"></param>
//        /// <returns></returns>
//        private List<string> Create_TOYODRAWINGNUMBERFORMAT_SearchString(string input, List<string> suffixList)
//        {
//            // 末尾記号のリストが図面番号の末尾に含まれるか確認
//            bool containsSuffix = suffixList.Exists(suffix => input.EndsWith(suffix));


//            if (containsSuffix)
//            {
//                // 末尾に含まれる場合はそれを削除
//                foreach (string suffix in suffixList)
//                {
//                    if (input.EndsWith(suffix))
//                    {
//                        input = input.Remove(input.LastIndexOf(suffix));
//                        break; // 最初に見つかったものだけを削除する
//                    }
//                }
//            }

//            // 新しいコレクションを作成して追加
//            List<string> newList = new List<string> { input };
//            List<string> addList = suffixList.SelectMany(item => new List<string> { input + item }).ToList();
//            newList.AddRange(addList);

//            return newList;
//        }

//    }
//}
