using Mysqlx.Prepare;
using SasaLib;
using SasaLib.PrintConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace CommonCommitLogic
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public class CreateImage
    {
        private Delegate_ExecuteTiffExport_Method _Delegate_ExecuteTiffExport_Method;

        private Action<string> _WrteLine;

        public CreateImage(Delegate_ExecuteTiffExport_Method Delegate_ExecuteTiffExport_Method, Action<string> WriteLine)
        {
            _Delegate_ExecuteTiffExport_Method = Delegate_ExecuteTiffExport_Method;
            _WrteLine = WriteLine;
        }

        public bool Execute(string tiffFullPahtWithExtension, string ImagePosXMLfileFullPath)
        {
            // TIFFイメージ書き出し------------------------------------------------------------------------------------------------------------------------------------------------------------
            string tiffImageFullPathWithOutExt = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(tiffFullPahtWithExtension) ?? "", System.IO.Path.GetFileNameWithoutExtension(tiffFullPahtWithExtension));

            CommonPaperSize cuurenetPaperSize;
            bool ans = _Delegate_ExecuteTiffExport_Method(tiffImageFullPathWithOutExt, ImagePosXMLfileFullPath, out cuurenetPaperSize, _WrteLine,  true);

            _WrteLine($"");
            if (ans == false)
            {
                _WrteLine($"");
                return false;
            }
            else
                _WrteLine($"");
            try
            {
                var tiffFullPath = System.IO.Path.ChangeExtension(tiffImageFullPathWithOutExt, "TIF");
                System.Drawing.Image img = System.Drawing.Image.FromFile(tiffFullPath);
                bool result = ConvertImageToPDF(img, System.IO.Path.ChangeExtension(tiffImageFullPathWithOutExt, "PDF"), "Microsoft Print to PDF");
            }
            catch { }

            return true;
        }

        public bool ConvertImageToPDF(System.Drawing.Image outPutimage, string PrintOutputFullFileName, string PrinterDriverName)
        {
            List<PaperSizeAndSource> PaperSizeAndSources;

            PaperSizeAndSources = new List<PaperSizeAndSource>()
            {
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A0横向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A0L,PaperName="A0",Xoffset=0,Yoffset=0,LandScape=true,SourceName="自動"    ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A0縦向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A0P,PaperName="A0",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動"  ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A1横向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A1L,PaperName="A1",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動"   ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A1縦向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A1P,PaperName="A1",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動"  ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A2横向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A2L,PaperName="A2",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動"   ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A2縦向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A2P,PaperName="A2",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動"  ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A3横向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A3L,PaperName="A3",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動"   ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A3縦向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A3P,PaperName="A3",Xoffset=0,Yoffset=0,LandScape=false  ,SourceName="自動" ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A4横向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A4L,PaperName="A4",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動"   ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
                new PaperSizeAndSource{Comment="Microsoft Print to PDF A4縦向き",CommonPaperSizeEnum=SasaLib.PrintConfig.CommonPaperSize.A4P,PaperName="A4",Xoffset=0,Yoffset=0,LandScape=false  ,SourceName="自動" ,BeforeExtractType = PrinterSimple.BeforeExtractType.ページサイズ範囲},
            };

            PrinterSimple printService = new PrinterSimple(outPutimage, PrinterDriverName, PaperSizeAndSources);

            string DocumentName = System.IO.Path.GetFileNameWithoutExtension(PrintOutputFullFileName);

            bool result = printService.PrintExecute(DocumentName, PrintOutputFullFileName);

            return result;
        }

    }
}
