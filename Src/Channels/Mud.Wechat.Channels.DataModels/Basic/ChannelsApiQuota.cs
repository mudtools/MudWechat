// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>当日调用额度详情（官方 <c>quota</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsApiQuota
{
    /// <summary>获取或设置当天可调用次数（官方 <c>daily_limit</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public int DailyLimit { get; set; }

    /// <summary>获取或设置当天已调用次数（官方 <c>used</c>）。</summary>
    [JsonPropertyName("used")]
    public int Used { get; set; }

    /// <summary>获取或设置当天剩余调用次数（官方 <c>remain</c>）。</summary>
    [JsonPropertyName("remain")]
    public int Remain { get; set; }
}
