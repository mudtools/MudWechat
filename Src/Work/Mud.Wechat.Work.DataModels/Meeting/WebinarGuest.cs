// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 网络研讨会嘉宾对象（获取/更新网络研讨会嘉宾列表的 <c>guests</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class WebinarGuest
{
    /// <summary>
    /// 获取或设置嘉宾类型（官方必填）：1 - 内部嘉宾；2 - 外部嘉宾；3 - 链接嘉宾
    /// （更新嘉宾列表文档页仅列 1/2 两种取值）。
    /// </summary>
    [JsonPropertyName("guest_type")]
    public int? GuestType { get; set; }

    /// <summary>获取或设置嘉宾的 userid（内部嘉宾必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置国家/地区代码（外部嘉宾必填；例如：中国传 86，不是 +86）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>获取或设置手机号（外部嘉宾必填；通过嘉宾链接接入会的嘉宾无此字段）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>获取或设置嘉宾名称（外部嘉宾必填，1~16 位字符长度）。</summary>
    [JsonPropertyName("guest_name")]
    public string? GuestName { get; set; }

    /// <summary>获取或设置邮箱（外部嘉宾选填；通过嘉宾链接接入会的嘉宾无此字段）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
