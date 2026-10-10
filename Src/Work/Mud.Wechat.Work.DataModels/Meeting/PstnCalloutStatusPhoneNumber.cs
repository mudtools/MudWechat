// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 外呼状态号码对象（获取会议的外呼状态响应 <c>phone_numbers</c> 元素；
/// 无论从客户端、API 还是 Web 端发起的外呼均可获取，不针对呼叫人做数据隔离）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class PstnCalloutStatusPhoneNumber
{
    /// <summary>获取或设置国家/地区代码（例如：中国是 86）。</summary>
    [JsonPropertyName("area")]
    public int? Area { get; set; }

    /// <summary>获取或设置电话号码或固定电话总机号。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>获取或设置固定电话分机号。</summary>
    [JsonPropertyName("extension_number")]
    public string? ExtensionNumber { get; set; }

    /// <summary>获取或设置外呼状态。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置当场会议的成员临时 ID（可用于会控操作，适用于所有成员）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }
}
