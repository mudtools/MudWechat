// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>撤销用户已核销团购优惠（<c>withdrawconsume</c>）请求体。</summary>
/// <remarks>
/// 官方契约：待撤回核销的券最多 10 张且需属于同一订单；次卡传多个相同项代表撤销多次，整体同成功或同失败；
/// 撤销核销次卡时必须传 <c>consume_request_no</c>（说明撤销的核销单号）。
/// <b>官方参数表字段名 <c>reovke_vouchers</c> 为官方历史拼写，照抄原文（勿改写为 revoke）。</b>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherRequest
{
    /// <summary>获取或设置撤销操作幂等串（官方 <c>revoke_request_no</c>，必填；商户自定义，请保证在小店下唯一）。</summary>
    [JsonPropertyName("revoke_request_no")]
    public string RevokeRequestNo { get; set; } = string.Empty;

    /// <summary>获取或设置待撤回核销的券列表（官方 <c>reovke_vouchers</c>——官方历史拼写照抄，必填，最多 10 张）。</summary>
    [JsonPropertyName("reovke_vouchers")]
    public List<ChannelsRevokeVoucherItem> ReovkeVouchers { get; set; } = new();

    /// <summary>获取或设置原核销单号（官方 <c>consume_request_no</c>，选填；撤销核销次卡时必传）。</summary>
    [JsonPropertyName("consume_request_no")]
    public string? ConsumeRequestNo { get; set; }
}
