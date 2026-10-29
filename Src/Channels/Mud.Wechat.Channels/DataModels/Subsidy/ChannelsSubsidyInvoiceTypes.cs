// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Subsidy;

/// <summary>国补发票类型常量（官方 <c>invoice_type</c> 枚举值，upload_invoice_info）。</summary>
public static class ChannelsSubsidyInvoiceTypes
{
    /// <summary>增值税专用发票。</summary>
    public const string VatSpecial = "01";

    /// <summary>纸质普通发票。</summary>
    public const string PaperGeneral = "04";

    /// <summary>数电专票。</summary>
    public const string DigitalSpecial = "09";

    /// <summary>电子普通发票。</summary>
    public const string ElectronicGeneral = "10";

    /// <summary>数电普票。</summary>
    public const string DigitalGeneral = "90";
}
