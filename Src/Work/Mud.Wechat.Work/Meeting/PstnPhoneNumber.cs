// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 电话入会号码对象（批量外呼 / 获取电话入会的成员 ID 请求 <c>phone_numbers</c> 元素，
/// 及批量外呼响应 <c>invalid_phone_numbers</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class PstnPhoneNumber
{
    /// <summary>获取或设置国家/地区代码（官方必填；例如：中国是 86）。</summary>
    [JsonPropertyName("area")]
    public int? Area { get; set; }

    /// <summary>获取或设置电话号码或固定电话总机号（官方必填）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>获取或设置固定电话分机号。</summary>
    [JsonPropertyName("extension_number")]
    public string? ExtensionNumber { get; set; }
}
