// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟推广计划扩展信息（官方 <c>ext_info</c> 对象，<c>item/get</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：参数表字段为 <c>is_banned</c>，代码示例为 <c>is_sale_forbidden</c>——按参数表建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueItemExtInfo
{
    /// <summary>获取或设置是否被官方封禁（官方 <c>is_banned</c>）。</summary>
    [JsonPropertyName("is_banned")]
    public bool? IsBanned { get; set; }
}
