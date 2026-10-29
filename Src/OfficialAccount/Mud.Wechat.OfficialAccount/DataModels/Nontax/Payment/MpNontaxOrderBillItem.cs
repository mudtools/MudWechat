// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 非税支付订单明细项（官方 <c>items</c> 项；统一下单 / 微信支付各请求共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxOrderBillItem
{
    /// <summary>获取或设置明细序号（官方 <c>no</c>）。</summary>
    [JsonPropertyName("no")]
    public int No { get; set; }

    /// <summary>获取或设置收费项目 ID（官方 <c>item_id</c>）。</summary>
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;

    /// <summary>获取或设置收费项目名称（官方 <c>item_name</c>）。</summary>
    [JsonPropertyName("item_name")]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>获取或设置是否逾期（官方 <c>overdue</c>；0/1，可选）。</summary>
    [JsonPropertyName("overdue")]
    public int? Overdue { get; set; }

    /// <summary>获取或设置滞纳金（官方 <c>penalty</c>，单位分，可选）。</summary>
    [JsonPropertyName("penalty")]
    public int? Penalty { get; set; }

    /// <summary>获取或设置收费金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
}
