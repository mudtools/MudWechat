// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 发票定位（官方 <c>card_id</c> + <c>encrypt_code</c> 组合；批量更新与批量查询共用）。
/// </summary>
/// <remarks>官方原文：<c>card_id</c> 与 <c>encrypt_code</c> <b>共同构成一张发票卡券的唯一标识</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceRef
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方 <c>encrypt_code</c>，必填）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}
