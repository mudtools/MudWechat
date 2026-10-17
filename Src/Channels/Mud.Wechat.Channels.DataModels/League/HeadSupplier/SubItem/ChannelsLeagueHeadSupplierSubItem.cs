// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构配置达人佣金计划条目（官方 <c>list[]</c>，<c>headsupplier/subitem/list/get</c>）。
/// 三个佣金率字段并存：<c>ratio_to_service_rate</c>（相对服务费）、<c>ratio</c>（整数百分比）、<c>ratio_ext</c>（十万分之一）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierSubItem
{
    /// <summary>获取或设置机构配置达人佣金计划 id（官方 <c>sub_item_id</c>）。</summary>
    [JsonPropertyName("sub_item_id")]
    public long? SubItemId { get; set; }

    /// <summary>获取或设置机构配置达人佣金计划链接（官方 <c>head_supplier_item_link</c>）。</summary>
    [JsonPropertyName("head_supplier_item_link")]
    public string? HeadSupplierItemLink { get; set; }

    /// <summary>获取或设置计划状态（官方 <c>status</c>，见 <see cref="ChannelsLeagueHeadSupplierSubItemStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置相对于服务费的达人佣金率（官方 <c>ratio_to_service_rate</c>，区间 <c>[0, 90]</c> 百分比）。</summary>
    [JsonPropertyName("ratio_to_service_rate")]
    public int? RatioToServiceRate { get; set; }

    /// <summary>获取或设置达人佣金率（官方 <c>ratio</c>，区间 <c>[0, 90]</c> 百分比）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }

    /// <summary>获取或设置达人精确佣金率（官方 <c>ratio_ext</c>，区间 <c>[0, 900000]</c>，10000 = 1%，2026-03-23 更新描述）。</summary>
    [JsonPropertyName("ratio_ext")]
    public long? RatioExt { get; set; }

    /// <summary>获取或设置推广二维码对应短链（官方 <c>qr_code_url</c>，2026-03-09 新增；须自行转成二维码后扫码）。</summary>
    [JsonPropertyName("qr_code_url")]
    public string? QrCodeUrl { get; set; }
}
