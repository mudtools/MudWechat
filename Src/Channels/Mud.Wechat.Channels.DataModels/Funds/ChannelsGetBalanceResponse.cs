// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>
/// 获取账户余额（<c>getbalance</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）；
/// 金额字段单位均为<b>分</b>。所属权限集 id：138（第三方平台代小店商家调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBalanceResponse : ChannelsResponse
{
    /// <summary>获取或设置可提现余额（官方 <c>available_amount</c>，单位：分）。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>获取或设置待结算余额（官方 <c>pending_amount</c>，单位：分）。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }

    /// <summary>获取或设置二级商户号（官方 <c>sub_mchid</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}
