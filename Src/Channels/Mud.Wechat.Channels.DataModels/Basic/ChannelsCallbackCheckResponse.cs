// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>网络通信检测（<c>callback/check</c>）响应。</summary>
/// <remarks>
/// <c>package_loss</c> 因仅发送一个 ping 包，取值只有 <c>0%</c> / <c>100%</c> 两种，
/// 不宜作为精细化网络质量指标；官方「注意事项」章节原文为「本接口无特殊注意事项」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsCallbackCheckResponse : ChannelsResponse
{
    /// <summary>获取或设置 DNS 解析结果（官方 <c>dns</c>）。</summary>
    [JsonPropertyName("dns")]
    public ChannelsDnsCheckResult? Dns { get; set; }

    /// <summary>获取或设置 PING 检测结果（官方 <c>ping</c>）。</summary>
    [JsonPropertyName("ping")]
    public ChannelsPingCheckResult? Ping { get; set; }
}
