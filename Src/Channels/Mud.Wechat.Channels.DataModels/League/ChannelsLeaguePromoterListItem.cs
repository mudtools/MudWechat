// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>带货者列表条目（官方 <c>list</c> 数组元素，<c>promoter/list/get</c>）。</summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentBaseInfo"/> 字段集不同
/// （仅 <c>talent_appid</c>/<c>nickname</c>，无 <c>head_img</c>），按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromoterListItem
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置带货者昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
