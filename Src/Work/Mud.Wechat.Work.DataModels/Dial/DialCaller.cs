// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Dial;

/// <summary>
/// 公费电话主叫用户信息（<c>/cgi-bin/dial/get_dial_record</c> 响应内嵌结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Dial")]
public class DialCaller
{
    /// <summary>获取或设置主叫用户的 userid（官方 userid）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置主叫用户的通话时长（官方 duration）。</summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
}
