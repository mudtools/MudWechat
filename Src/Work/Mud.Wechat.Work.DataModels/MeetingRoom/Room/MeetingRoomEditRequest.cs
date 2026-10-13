// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 编辑会议室请求体（<c>/cgi-bin/oa/meetingroom/edit</c>）。
/// <para>
/// 官方字段面是「meetingroom_id 定位 + 其余字段按需覆盖」的形态：与
/// <see cref="MeetingRoomAddRequest"/> 不同，本类型的 name / capacity 亦可缺省，
/// 故除定位键外全部字段做成可空超集，不拆「全量编辑 / 局部编辑」两套 DTO。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomEditRequest
{
    /// <summary>获取或设置会议室 ID（官方 meetingroom_id，必填）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int MeetingRoomId { get; set; }

    /// <summary>获取或设置会议室名称（官方 name）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置会议室容纳人数（官方 capacity）。</summary>
    [JsonPropertyName("capacity")]
    public int? Capacity { get; set; }

    /// <summary>获取或设置会议室所在城市（官方 city）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置会议室所在楼宇（官方 building）。</summary>
    [JsonPropertyName("building")]
    public string? Building { get; set; }

    /// <summary>获取或设置会议室所在楼层（官方 floor）。</summary>
    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    /// <summary>获取或设置会议室所在建筑经纬度（官方 coordinate）。</summary>
    [JsonPropertyName("coordinate")]
    public MeetingRoomCoordinate? Coordinate { get; set; }

    /// <summary>获取或设置使用范围信息（官方 range）。</summary>
    [JsonPropertyName("range")]
    public MeetingRoomRange? Range { get; set; }

    /// <summary>获取或设置会议室支持的设备 ID 列表（官方 equipment）。</summary>
    [JsonPropertyName("equipment")]
    public List<int>? EquipmentIdList { get; set; }
}
