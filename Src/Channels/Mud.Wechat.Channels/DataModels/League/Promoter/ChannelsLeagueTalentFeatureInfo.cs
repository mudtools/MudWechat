// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>带货者特征信息（官方 <c>feature_info</c> 对象，<c>promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentFeatureInfo
{
    /// <summary>获取或设置带货者等级（官方 <c>level</c>，见 <see cref="ChannelsLeagueTalentLevels"/>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置是否被拉黑（官方 <c>is_blacklisted</c>）。</summary>
    [JsonPropertyName("is_blacklisted")]
    public bool? IsBlacklisted { get; set; }
}
