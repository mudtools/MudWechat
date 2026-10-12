// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商画像数据（官方 <c>live_ec_profile</c> 对象）。</summary>
/// <remarks>
/// 官方参数表对 <c>profiles</c>/<c>totals</c> 仅给实体名（LiveEcProfile_DataNodeThirdList / DataNodeList）未给字段表，
/// SDK 以 <see cref="System.Text.Json.JsonElement"/> 保真透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcProfile
{
    /// <summary>获取或设置不同用户群体的统计数据（官方 <c>profiles</c>，实体未展开）。</summary>
    [JsonPropertyName("profiles")]
    public List<System.Text.Json.JsonElement>? Profiles { get; set; }

    /// <summary>获取或设置总体数据统计（官方 <c>totals</c>，实体未展开）。</summary>
    [JsonPropertyName("totals")]
    public List<System.Text.Json.JsonElement>? Totals { get; set; }
}
