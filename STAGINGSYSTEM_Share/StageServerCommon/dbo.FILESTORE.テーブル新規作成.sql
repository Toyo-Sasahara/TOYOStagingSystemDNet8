USE [DATABASE1]
GO

/****** Object:  Table [dbo].[FILESTORE]    Script Date: 2023/02/03 9:18:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FILESTORE](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DEAD_FLUG] [bit] NULL,
	[GUID_RAW] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GUIDBASE64] [nvarchar](50) NOT NULL,
	[TICKETCODE] [nvarchar](255) NULL,
	[TIMESTAMP] [datetime2](7) NULL,
	[COMMITHOST] [nvarchar](50) NULL,
	[COMMITUSER] [nvarchar](50) NULL,
	[REQUESTPRINTER] [nvarchar](50) NULL,
	[PRINTINGTIME] [datetime2](7) NULL,
	[CREATESOFTWARE] [nvarchar](50) NULL,
	[DOCUMENTNAME] [nvarchar](max) NULL,
	[PLOTPAPERSIZE] [nvarchar](10) NULL,
	[PAPERSIZE] [nvarchar](10) NULL,
	[PARTNUMBER] [nvarchar](125) NULL,
	[SANITIZEDPARTNUMBER] [nvarchar](125) NULL,
	[REV] [nvarchar](50) NULL,
	[DRAWINGTYPE] [nvarchar](50) NULL,
	[TITLE] [nvarchar](125) NULL,
	[PARTSNAME] [nvarchar](125) NULL,
	[DESCRIPTION] [nvarchar](max) NULL,
	[MATERIAL] [nvarchar](50) NULL,
	[MATERIALCODE] [nvarchar](50) NULL,
	[MACHINETYPE] [nvarchar](50) NULL,
	[AUTHOR] [nvarchar](50) NULL,
	[AUTHORDATE] [nvarchar](50) NULL,
	[AUTHORPCUSER] [nvarchar](20) NULL,
	[AUTHOR_STAMP_POS_X] [nvarchar](10) NULL,
	[AUTHOR_STAMP_POS_Y] [nvarchar](10) NULL,
	[AUTHOR_STAMP_SCALE] [nvarchar](10) NULL,
	[DESIGNER] [nvarchar](50) NULL,
	[CHECKDATE] [nvarchar](50) NULL,
	[CHECKDPCUSER] [nvarchar](20) NULL,
	[CHECKED_STAMP_POS_X] [nvarchar](10) NULL,
	[CHECKED_STAMP_POS_Y] [nvarchar](10) NULL,
	[CHECKED_STAMP_SCALE] [nvarchar](10) NULL,
	[APPROVEDUSER] [nvarchar](50) NULL,
	[APPROVEDDATE] [nvarchar](50) NULL,
	[APPROVEDHOST] [nvarchar](50) NULL,
	[APPROVEDPCUSER] [nvarchar](20) NULL,
	[APPROVED_STAMP_POS_X] [nvarchar](10) NULL,
	[APPROVED_STAMP_POS_Y] [nvarchar](10) NULL,
	[APPROVED_STAMP_SCALE] [nvarchar](10) NULL,
	[CHECKREQUEST] [bit] NULL,
	[APPROVALREQUEST] [bit] NULL,
	[REGISTWAITINGFLAG] [bit] NULL,
	[PRIORITYREGISTFLAG] [bit] NULL,
	[REGISTWAITINGFLAGGEDTIME] [datetime2](7) NULL,
	[REGISTEDTIME] [nvarchar](50) NULL,
	[REGISTEDUSER] [nvarchar](50) NULL,
	[REGISTEDUSERID] [nvarchar](10) NULL,
	[ARCSUITEID] [nvarchar](50) NULL,
	[CUSTOMER] [nvarchar](62) NULL,
	[FIRSTCUSTOMER] [nvarchar](62) NULL,
	[ORDERNUMBER] [nvarchar](50) NULL,
	[OLD_DWG_NO] [nvarchar](25) NULL,
	[SUPPLIER] [nvarchar](50) NULL,
	[SURFACETREATMENT] [nvarchar](50) NULL,
	[VARIANT] [bit] NULL,
 CONSTRAINT [PK_FILESTORE_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FILESTORE] ADD  CONSTRAINT [DF_FILESTORE_GUID_RAW]  DEFAULT (newid()) FOR [GUID_RAW]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'インデックス' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GUIDコード' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'GUID_RAW'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GUIDをBASE64でエンコードしたもの' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'GUIDBASE64'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'チケットコード(図面に印字される)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'TICKETCODE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミット日時' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'TIMESTAMP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミットしたPC名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'COMMITHOST'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミットしたPCのログインユーザー名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'COMMITUSER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミット時に要求されたプリンタ（プリンタ設定.xmlへのショートカットファイル）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'REQUESTPRINTER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'予約プロパティ「印刷を開始する時間」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'PRINTINGTIME'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面を作成したソフトウェア' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'CREATESOFTWARE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミット元の図面ファイル名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'DOCUMENTNAME'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'コミット元のアプリケーションでのページ設定（用紙サイズ・方向）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'PLOTPAPERSIZE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生成されたTIFFファイルのページ設定（用紙サイズ・方向）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'PAPERSIZE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「図面番号」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'PARTNUMBER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'アークスイートに登録するためにゼロ補完等をした登録用図番号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'SANITIZEDPARTNUMBER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「リビジョン番号」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'REV'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面種類名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'DRAWINGTYPE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'TITLE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部品図面表題欄の部品名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'PARTSNAME'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'組立図表題欄の説明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'DESCRIPTION'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部品図面表題欄の「材質名」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'MATERIAL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部品図面表題欄の「材質コード」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'MATERIALCODE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部品図面表題欄の「機種名」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'MACHINETYPE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「製図者氏名」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'AUTHOR'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「製図日」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'AUTHORDATE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'未使用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'AUTHORPCUSER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「設計者名」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'DESIGNER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'図面表題欄の「設計日」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'CHECKDATE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'未使用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'CHECKDPCUSER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'標準プロパティ「」' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE', @level2type=N'COLUMN',@level2name=N'CUSTOMER'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'不用意にデータ型を変更しないこと！！' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FILESTORE'
GO

