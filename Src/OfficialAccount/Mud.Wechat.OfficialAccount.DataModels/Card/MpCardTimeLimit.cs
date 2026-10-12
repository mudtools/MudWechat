// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>使用时段限制（官方 <c>advanced_info.time_limit[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardTimeLimit
{
    /// <summary>获取或设置限制类型（官方 <c>type</c>，星期/每天/节假日枚举<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置起始小时（官方 <c>begin_hour</c>，格式 H，0~23）。</summary>
    [JsonPropertyName("begin_hour")]
    public int? BeginHour { get; set; }

    /// <summary>获取或设置起始分钟（官方 <c>begin_minute</c>，格式 m，0~59）。</summary>
    [JsonPropertyName("begin_minute")]
    public int? BeginMinute { get; set; }

    /// <summary>获取或设置结束小时（官方 <c>end_hour</c>）。</summary>
    [JsonPropertyName("end_hour")]
    public int? EndHour { get; set; }

    /// <summary>获取或设置结束分钟（官方 <c>end_minute</c>）。</summary>
    [JsonPropertyName("end_minute")]
    public int? EndMinute { get; set; }
}
