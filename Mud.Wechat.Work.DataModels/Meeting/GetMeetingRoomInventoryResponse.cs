// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取 Rooms 会议室资源响应体（<c>/cgi-bin/meeting/rooms/get_inventory</c>；获取企业购买的 Rooms 会议室资源；
/// 官方契约为<b>无请求体的 POST</b>，仅以 Query 注入的 access_token 鉴权）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingRoomInventoryResponse : WechatWorkResponse
{
    /// <summary>获取或设置普通设备数。</summary>
    [JsonPropertyName("normal_count")]
    public int? NormalCount { get; set; }

    /// <summary>获取或设置专款设备数。</summary>
    [JsonPropertyName("special_count")]
    public int? SpecialCount { get; set; }

    /// <summary>获取或设置普通设备使用数。</summary>
    [JsonPropertyName("normal_used_count")]
    public int? NormalUsedCount { get; set; }

    /// <summary>获取或设置专款设备使用数。</summary>
    [JsonPropertyName("special_used_count")]
    public int? SpecialUsedCount { get; set; }

    /// <summary>获取或设置普通设备过期数。</summary>
    [JsonPropertyName("normal_expired_count")]
    public int? NormalExpiredCount { get; set; }

    /// <summary>获取或设置专款设备过期数。</summary>
    [JsonPropertyName("special_expired_count")]
    public int? SpecialExpiredCount { get; set; }
}
