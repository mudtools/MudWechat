// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 维护状态控制信息（官方 <c>control_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftMaintainControl
{
    /// <summary>获取或设置商户侧维护类型（官方 <c>biz_control_type</c>）。</summary>
    [JsonPropertyName("biz_control_type")]
    public string? BizControlType { get; set; }

    /// <summary>获取或设置系统侧维护类型（官方 <c>system_biz_control_type</c>）。</summary>
    [JsonPropertyName("system_biz_control_type")]
    public string? SystemBizControlType { get; set; }

    /// <summary>获取或设置维护中的货架列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public MpCardGiftMaintainPage[]? PageList { get; set; }
}
