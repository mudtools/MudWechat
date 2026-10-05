// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡记录（获取打卡记录数据响应 <c>checkindata</c> 元素，自建应用与服务商代开发文档口径）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>checkin_type</c>/<c>exception_type</c> 为<b>中文字符串</b>（如「上班打卡」「时间异常」），不是枚举整数，
/// 多个异常以分号间隔；<c>lat</c>/<c>lng</c> 为实际经纬度的 1000000 倍（GCJ-02 坐标系），仅位置打卡记录返回；
/// <c>deviceid</c> 仅外出/设备打卡场景返回，非固定返回字段。
/// 第三方应用文档页记录为另一套旧字段结构（<see cref="ThirdPartyCheckinRecord"/>），两形态不可混用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinRecord
{
    /// <summary>获取或设置用户 id。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置打卡规则名称。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置打卡类型（字符串，目前有：上班打卡，下班打卡，外出打卡，仅记录打卡时间和位置）。</summary>
    [JsonPropertyName("checkin_type")]
    public string? CheckinType { get; set; }

    /// <summary>获取或设置异常类型（字符串，包括：时间异常，地点异常，未打卡，wifi 异常，非常用设备；如果有多个异常，以分号间隔）。</summary>
    [JsonPropertyName("exception_type")]
    public string? ExceptionType { get; set; }

    /// <summary>获取或设置打卡时间（Unix 时间戳）。</summary>
    [JsonPropertyName("checkin_time")]
    public long? CheckinTime { get; set; }

    /// <summary>获取或设置打卡地点 title。</summary>
    [JsonPropertyName("location_title")]
    public string? LocationTitle { get; set; }

    /// <summary>获取或设置打卡地点详情。</summary>
    [JsonPropertyName("location_detail")]
    public string? LocationDetail { get; set; }

    /// <summary>获取或设置打卡 wifi 名称。</summary>
    [JsonPropertyName("wifiname")]
    public string? Wifiname { get; set; }

    /// <summary>获取或设置打卡备注。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置打卡的 MAC 地址/bssid。</summary>
    [JsonPropertyName("wifimac")]
    public string? Wifimac { get; set; }

    /// <summary>获取或设置打卡的附件 media_id 列表（可使用 media/get 获取附件）。</summary>
    [JsonPropertyName("mediaids")]
    public List<string>? Mediaids { get; set; }

    /// <summary>获取或设置位置打卡地点纬度（实际纬度的 1000000 倍，与腾讯地图一致采用 GCJ-02 坐标系统标准；仅位置打卡记录返回）。</summary>
    [JsonPropertyName("lat")]
    public long? Lat { get; set; }

    /// <summary>获取或设置位置打卡地点经度（实际经度的 1000000 倍，与腾讯地图一致采用 GCJ-02 坐标系统标准；仅位置打卡记录返回）。</summary>
    [JsonPropertyName("lng")]
    public long? Lng { get; set; }

    /// <summary>获取或设置打卡设备 id（仅外出/设备打卡场景返回）。</summary>
    [JsonPropertyName("deviceid")]
    public string? Deviceid { get; set; }

    /// <summary>获取或设置标准打卡时间（指此次打卡时间对应的标准上班时间或标准下班时间，Unix 时间戳；标准打卡时间只对于固定排班和自定义排班两种类型有效）。</summary>
    [JsonPropertyName("sch_checkin_time")]
    public long? SchCheckinTime { get; set; }

    /// <summary>获取或设置规则 id（表示打卡记录所属规则的 id）。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置班次 id（表示打卡记录所属规则中，所属班次的 id）。</summary>
    [JsonPropertyName("schedule_id")]
    public long? ScheduleId { get; set; }

    /// <summary>获取或设置时段 id（表示打卡记录所属规则中，某一班次中的某一时段的 id，如上下班时间为 9:00-12:00、13:00-18:00 的班次中，9:00-12:00 为其中一组时段）。</summary>
    [JsonPropertyName("timeline_id")]
    public long? TimelineId { get; set; }
}
