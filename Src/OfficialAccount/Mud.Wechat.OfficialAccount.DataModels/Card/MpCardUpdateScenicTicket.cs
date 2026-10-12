// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>景区门票（修改方向，官方 <c>scenic_ticket</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateScenicTicket
{
    /// <summary>获取或设置基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardUpdateBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置票种（官方 <c>ticket_class</c>，选填）。</summary>
    [JsonPropertyName("ticket_class")]
    public string? TicketClass { get; set; }

    /// <summary>获取或设置景区导览图 URL（官方 <c>guide_url</c>，选填）。</summary>
    [JsonPropertyName("guide_url")]
    public string? GuideUrl { get; set; }
}
