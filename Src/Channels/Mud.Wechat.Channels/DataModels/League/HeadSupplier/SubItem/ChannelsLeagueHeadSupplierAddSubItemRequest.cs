// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>配置达人佣金率请求（官方 <c>headsupplier/subitem/add</c> 请求体）。
/// <c>ratio_ext</c>（十万分之一，10000 = 1%）与 <c>ratio</c>（整数百分比）二选一。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierAddSubItemRequest
{
    /// <summary>获取或设置机构合作计划 id（官方 <c>cooperative_item_id</c>，必填）。</summary>
    [JsonPropertyName("cooperative_item_id")]
    public long CooperativeItemId { get; set; }

    /// <summary>获取或设置达人精确佣金率（官方 <c>ratio_ext</c>，区间 <c>[0, 900000]</c>，10000 = 1%，与 <c>ratio</c> 二选一）。</summary>
    [JsonPropertyName("ratio_ext")]
    public long? RatioExt { get; set; }

    /// <summary>获取或设置达人佣金率（官方 <c>ratio</c>，区间 <c>[0, 90]</c> 百分比，与 <c>ratio_ext</c> 二选一）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }
}
