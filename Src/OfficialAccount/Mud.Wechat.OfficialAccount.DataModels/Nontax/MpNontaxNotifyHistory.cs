// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询订单」响应的通知尝试记录（官方 <c>notify_history</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxNotifyHistory
{
    /// <summary>获取或设置通知的 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置通知名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置通知尝试明细（官方 <c>notify_detail</c>）。</summary>
    [JsonPropertyName("notify_detail")]
    public MpNontaxNotifyAttempt[]? NotifyDetail { get; set; }

    /// <summary>获取或设置通知次数（官方 <c>notify_cnt</c>）。</summary>
    [JsonPropertyName("notify_cnt")]
    public int NotifyCount { get; set; }
}
