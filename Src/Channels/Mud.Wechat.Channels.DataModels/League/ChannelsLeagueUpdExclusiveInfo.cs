// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>特殊推广信息·更新形态（官方 <c>exclusive_info</c> 对象，<c>item/upd</c>）。</summary>
/// <remarks>
/// 与 <see cref="ChannelsLeagueExclusiveInfo"/>（响应形态）分建：更新形态承载
/// <c>add_talent_appids</c>/<c>del_talent_appids</c>（各不超过 20 个），响应形态为
/// <c>talent_num</c>（推广带货者数量）——字段集不同、语义可独立演化。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueUpdExclusiveInfo
{
    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置用于新增推广达人列表（官方 <c>add_talent_appids</c>，选填，不超过 20 个）。</summary>
    [JsonPropertyName("add_talent_appids")]
    public List<string>? AddTalentAppIds { get; set; }

    /// <summary>获取或设置删除推广达人列表（官方 <c>del_talent_appids</c>，选填，不超过 20 个）。</summary>
    [JsonPropertyName("del_talent_appids")]
    public List<string>? DelTalentAppIds { get; set; }
}
