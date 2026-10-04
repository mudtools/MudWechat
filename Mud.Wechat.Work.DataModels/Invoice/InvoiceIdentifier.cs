// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 电子发票标识参数（发票卡券的 <c>card_id</c> + <c>encrypt_code</c>，二者共同构成唯一标识）。
/// <para>批量查询电子发票（<c>getinvoiceinfobatch</c> 的 <c>item_list</c> 项）与
/// 批量更新发票状态（<c>updatestatusbatch</c> 的 <c>invoice_list</c> 项）共用本结构。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class InvoiceIdentifier
{
    /// <summary>
    /// 获取或设置发票 id（官方必填）。
    /// </summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>
    /// 获取或设置加密 code（官方必填，与 card_id 共同构成发票唯一标识）。
    /// </summary>
    [JsonPropertyName("encrypt_code")]
    public string? EncryptCode { get; set; }
}
