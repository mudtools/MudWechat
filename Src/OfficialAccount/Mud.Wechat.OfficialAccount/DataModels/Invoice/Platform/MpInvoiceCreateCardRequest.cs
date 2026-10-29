// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 创建发票卡券模板（POST /card/invoice/platform/createcard）

/// <summary>
/// 创建发票卡券模板（<c>POST /card/invoice/platform/createcard</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>invoice_info</c>（发票模板对象）必填。
/// <b>适用范围为「公众号 / 服务号 —— 需申请」</b>（须提交场景申请并审核通过后方可调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCreateCardRequest
{
    /// <summary>获取或设置发票模板对象（官方 <c>invoice_info</c>，必填）。</summary>
    [JsonPropertyName("invoice_info")]
    public MpInvoiceTemplateInfo InvoiceInfo { get; set; } = new MpInvoiceTemplateInfo();
}
