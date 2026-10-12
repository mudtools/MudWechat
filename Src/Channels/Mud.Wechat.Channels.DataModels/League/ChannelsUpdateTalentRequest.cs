// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>编辑带货者（<c>talent/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsUpdateTalentRequest
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string TalentAppId { get; set; } = string.Empty;

    /// <summary>获取或设置编辑操作类型（官方 <c>upd_talent_type</c>，必填，见 <see cref="ChannelsLeagueUpdTalentTypes"/>）。</summary>
    [JsonPropertyName("upd_talent_type")]
    public int UpdTalentType { get; set; }
}
