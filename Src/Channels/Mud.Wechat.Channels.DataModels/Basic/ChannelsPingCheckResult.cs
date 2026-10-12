// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>PING 检测结果（官方 <c>ping</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsPingCheckResult
{
    /// <summary>获取或设置 ping 的 IP（官方 <c>ip</c>）。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>获取或设置丢包率（官方 <c>package_loss</c>，取值 <c>0%</c> / <c>100%</c>）。</summary>
    [JsonPropertyName("package_loss")]
    public string? PackageLoss { get; set; }

    /// <summary>获取或设置耗时（官方 <c>time</c>，单位：毫秒）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }
}
