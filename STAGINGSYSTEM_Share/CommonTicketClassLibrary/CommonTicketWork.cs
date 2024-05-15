using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Versioning;
[SupportedOSPlatform("windows")]
public class CommonTicketWork
{
    static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

    public static string Attr { get; private set; }
    public static string BlockName { get; private set; }

    /// <summary>
    ///  MakeTicketTemplateが定義するバージョン番号（MakeTicketTemplateを書きかけるたびに上げる）
    /// </summary>
    public static double CurrentRemakeVersion = 1.6d;

    /// <summary>
    /// チケットファイルのテンプレートを強制作成
    /// </summary>
    /// <param name="confFIle"></param>
    public static void MakeTicketTemplate(string confFIle)
    {
        // パラメータを構成する.（初期値が無いものは""とすること）
        CommonTicket obj = new CommonTicket
        {
            VERSION = CurrentRemakeVersion,
            /// "^\[ \t]*$" 空行一致
            Params = new List<CommonTicket.Param>() {
                //
                new CommonTicket.Param{
                    Key = "PARTNUMBER",
                    Acad_attr = @"^GEN-TITLE-NR",
                    SW_porp = @"Number",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "PRP_DWGNO" },

                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "PRP DWGNO" },

                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP DWGNO" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Part Number",
                },
                //
                new CommonTicket.Param{
                    Key = "SANITIZEDPARTNUMBER",
                    Acad_attr = @"^\[ \t]*$",
                    SW_porp = @"",
                    Inventor_ipropSet = @"",
                    Inventor_iprop = @"",
                },
                //
                new CommonTicket.Param{
                    Key = "DRAWINGTYPE",
                    Acad_attr = @"^\[ \t]*$",
                    SW_porp = @"",
                    Inventor_ipropSet = @"",
                    Inventor_iprop = @"",
                },
                //
                new CommonTicket.Param{
                    Key = "REV",
                    Acad_attr = @"^GEN-TITLE-REV",
                    SW_porp = @"ﾘﾋﾞｼﾞｮﾝ",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "AutoAttr0" },
                    },
                    Inventor_ipropSet = @"Summary Information",
                    Inventor_iprop = @"Revision Number",
                },
                //
                new CommonTicket.Param{
                    Key = "TITLE",
                    Acad_attr = @"^GEN-TITLE-DES1",
                    SW_porp = @"Description",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "PRPSHEET_TITLE" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr2" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP_TITLE" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Description",
                },
                //
                new CommonTicket.Param{
                    Key = "MATERIAL",
                    Acad_attr = @"^GEN-TITLE-MAT1",
                    SW_porp = @"材質",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr8" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr8" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr8" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr8" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "PRPSHEET_MATERIAL" },
                    },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"材料",
                },
                //
                new CommonTicket.Param{
                    Key = "MATERIALCODE",
                    Acad_attr = @"^GEN-TITLE-MAT2",
                    SW_porp = @"材質コード",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr1" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr1" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr1" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr1" },
                    },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"材料コード",
                },
                //
                new CommonTicket.Param{
                    Key = "MACHINETYPE",
                    Acad_attr = @"^STR-TYPE",
                    SW_porp = @"機種名",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "顧客・機種名", Attr= "AutoAttr0" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr10" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr10" },
                    },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"機種",
                },
                //
                new CommonTicket.Param{
                    Key = "CUSTOMER",
                    Acad_attr = @"^USER",
                    SW_porp = @"顧客",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "東陽購買使用欄", Attr= "AutoAttr1" },
                        new CommonTicket.BlockAttr{BlockName= "顧客・機種名", Attr= "AutoAttr2" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr9" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP_USER" },
                   },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"顧客",
                },
                //
                new CommonTicket.Param{
                    Key = "FIRSTCUSTOMER",
                    Acad_attr = @"^STR-INIUSER",
                    SW_porp = @"初期顧客",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "顧客・機種名", Attr= "AutoAttr2" }
                    },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"顧客",
                },
                //
                new CommonTicket.Param{
                    Key = "DESCRIPTION",
                    Acad_attr = @"^DESCRIPTION",
                    SW_porp = @"Description",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr3" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "PRPSHEET_TITLE" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr2" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP_TITLE" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Description"
                },
                //
                new CommonTicket.Param{
                    Key = "SURFACETREATMENT",
                    Acad_attr = @"^SURFACETREATMENT",
                    SW_porp = @"",
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"表面処理"
                },
                //
                new CommonTicket.Param{
                    Key = "AUTHOR",
                    Acad_attr = @"^GEN-TITLE-NAME",
                    SW_porp = @"作図者",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr6" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr6" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr7" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr7" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "PRPSHEET_DRAWN" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP_DRAWN" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Checked By",
                },
                //
                new CommonTicket.Param{
                    Key = "AUTHORDATE",
                    Acad_attr = @"^GEN-TITLE-DAT",
                    SW_porp = @"作図日",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr4" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRP_DRAWN_DATE" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Date Checked",
                },
                //
                new CommonTicket.Param{
                    Key = "DESIGNER",
                    Acad_attr = @"^GEN-TITLE-CHKM",
                    SW_porp = @"設計者",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr7" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr6" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr6" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr5" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRPSHEET_DESIGNED" },
                    },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Engr Approved By",
                },
                //
                new CommonTicket.Param{
                    Key = "CHECKDATE",
                    Acad_attr = @"^GEN-TITLE-CHKD",
                    SW_porp = @"設計日",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr11" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr11" },
                         new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr11" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr11" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図標題欄", Attr= "AutoAttr8" },
                        new CommonTicket.BlockAttr{BlockName= "東陽組立図表題欄", Attr= "PRPSHEET_DESIGNED_DATE" },
                   },
                    Inventor_ipropSet = @"Design Tracking Properties",
                    Inventor_iprop = @"Engr Date Approved",
                },
                //
                new CommonTicket.Param{
                    Key = "OLD_DWG_NO",
                    Acad_attr = @"^OLD_DWG_NO",
                    SW_porp = @"",
                    SW_BlockAttr = new List<CommonTicket.BlockAttr>{
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ12", Attr= "AutoAttr10" },
                        new CommonTicket.BlockAttr{BlockName= "ﾌﾞﾛｯｸ13", Attr= "AutoAttr10" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄", Attr= "AutoAttr10" },
                        new CommonTicket.BlockAttr{BlockName= "東陽部品図表題欄(設計課)", Attr= "AutoAttr10" },
                   },
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"OLD DWG NO",
                },
                //
                new CommonTicket.Param{
                    Key = "ORDERNUMBER",
                    Acad_attr = @"^ORDER-NUMBER",
                    SW_porp = @"指令番号",
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"指令番号",
                },
                //
                new CommonTicket.Param{
                    Key = "SUPPLIER",
                    Acad_attr = @"^SUPPLIER",
                    SW_porp = @"",
                    Inventor_ipropSet = @"User Defined Properties",
                    Inventor_iprop = @"サプライヤ",
                },
                //new CommonTicket.Param{
                //    Key = "AUTHOR_STAMP_POS_X",
                //},
                //new CommonTicket.Param{
                //    Key = "AUTHOR_STAMP_POS_Y",
                //},
                //new CommonTicket.Param{
                //    Key = "AUTHOR_STAMP_SCALE",
                //},
                //new CommonTicket.Param{
                //    Key = "CHECKED_STAMP_POS_X",
                //},
                //new CommonTicket.Param{
                //    Key = "CHECKED_STAMP_POS_Y",
                //},
                //new CommonTicket.Param{
                //    Key = "CHECKED_STAMP_SCALE",
                //},
                //new CommonTicket.Param{
                //    Key = "APPROVED_STAMP_POS_X",
                //},
                //new CommonTicket.Param{
                //    Key = "APPROVED_STAMP_POS_Y",
                //},
                //new CommonTicket.Param{
                //    Key = "APPROVED_STAMP_SCALE",
                //},
                //new CommonTicket.Param{
                //    Key = "TECHS_HINBAN",
                //    Acad_attr = @"^TECHS_HINBAN",
                //    SW_porp = @"^TECHS_HINBAN",
                //    Inventor_ipropSet = @"User Defined Properties",
                //    Inventor_iprop = @"TECHS品番",
                //},
            },
            // 
            Variant = new List<CommonTicket.Param>() { }

            ///// 表図面用キー追加
            //Variant = new List<CommonTicket.Param>(){
            //    new CommonTicket.Param {
            //        Key = "PARTNUMBER",
            //        Value = "XX-12345-001",

            //    },
            //    new CommonTicket.Param {
            //         Key = "PARTNUMBER",
            //        Value = "XX-12341-100",
            //    },
            //}
            /////
        };

        SaveTicketTemplate(confFIle, obj);
    }

    public static void SaveTicketTemplate(string confFIle, CommonTicket obj)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));

        using (StreamWriter sw = new StreamWriter(confFIle, false, Encoding.UTF8))
        {
            serializer.Serialize(sw, obj);
        }
    }

    /// <summary>
    /// チケットファイルのテンプレートを強制作成
    /// </summary>
    /// <param name="confFIle"></param>
    /// <param name="Params"></param>
    public static void MakeTicketTemplate(string confFIle, List<CommonTicket.Param> Params, List<CommonTicket.Param> Variant)
    {
        CommonTicket obj = new CommonTicket
        {
            VERSION = CurrentRemakeVersion,
            Params = Params,
            Variant = Variant
        };

        XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));

        using (StreamWriter sw = new StreamWriter(confFIle, false, Encoding.UTF8))
        {
            serializer.Serialize(sw, obj);
        }
    }

    /// <summary>
    /// CommonTicket内のParamのキーを検索して struct Param　ごと入れ替えるテスト
    /// </summary>
    /// <param name="ticketObj"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool UpdateComonTikectParam(ref CommonTicket ticketObj, string key, string value)
    {
        for (int count = 0; count < ticketObj.Params.Count; count++)
        {
            if (ticketObj.Params[count].Key == key)
            {
                ticketObj.Params[count] = new CommonTicket.Param { Key = key, Value = value };
                return true;
            }
        }
        return false;
    }
}