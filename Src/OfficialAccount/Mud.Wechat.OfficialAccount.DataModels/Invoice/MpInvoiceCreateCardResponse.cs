// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 创建发票卡券模板（<c>POST /card/invoice/platform/createcard</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<c>card_id</c>（错误码为 0 时返回；后续调用插卡接口时必填）。
/// 官方文档缺陷（照录）：本页错误码表与「创建卡券模板」场景不匹配（大量为插卡 / 票据领取场景码
/// —— 族级表整段复用）；且把 <c>errcode</c> 标为 <c>string</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCreateCardResponse : MpResponse
{
    /// <summary>获取或设置发票卡券模板编号（官方 <c>card_id</c>；后续插卡接口必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}
