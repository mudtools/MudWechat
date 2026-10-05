// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡记录（获取打卡记录数据响应 <c>checkindata</c> 元素，第三方应用文档口径的旧字段结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：第三方文档页（94205）与自建/代开发文档页（90262/96497）同路由不同构 ——
/// 本旧结构无 <c>lat</c>/<c>lng</c>/<c>deviceid</c>/<c>sch_checkin_time</c>，代之以 <c>agency_name</c>、
/// <c>location_title_lat</c>/<c>location_title_lng</c> 与 <c>schedule_checkin_time</c>；
/// <c>checkin_type</c> 参数表标注为字符串但返回示例为整数（1/2），以示例的整数形态承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinRecord
{
    /// <summary>获取或设置用户 id。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置打卡规则名称。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置打卡类型（参数表标注字符串「1.上班打卡；2.下班打卡；3.外出打卡」，返回示例为整数 1/2，以示例的整数形态承载）。</summary>
    [JsonPropertyName("checkin_type")]
    public int? CheckinType { get; set; }

    /// <summary>获取或设置异常类型（字符串，包括：时间异常，地点异常，wifi 异常，非常用设备异常）。</summary>
    [JsonPropertyName("exception_type")]
    public string? ExceptionType { get; set; }

    /// <summary>获取或设置打卡时间（Unix 时间戳）。</summary>
    [JsonPropertyName("checkin_time")]
    public long? CheckinTime { get; set; }

    /// <summary>获取或设置打卡地点名称。</summary>
    [JsonPropertyName("location_title")]
    public string? LocationTitle { get; set; }

    /// <summary>获取或设置打卡地点详情。</summary>
    [JsonPropertyName("location_detail")]
    public string? LocationDetail { get; set; }

    /// <summary>获取或设置打卡地点 wifi 名称。</summary>
    [JsonPropertyName("wifiname")]
    public string? Wifiname { get; set; }

    /// <summary>获取或设置打卡备注。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置打卡地点 wifi 的 MAC 地址/bssid。</summary>
    [JsonPropertyName("wifimac")]
    public string? Wifimac { get; set; }

    /// <summary>获取或设置打卡的附件 media_id 列表（可使用获取临时素材接口下载）。</summary>
    [JsonPropertyName("mediaids")]
    public List<string>? Mediaids { get; set; }

    /// <summary>获取或设置代打卡人的名字（仅代打卡时有）。</summary>
    [JsonPropertyName("agency_name")]
    public string? AgencyName { get; set; }

    /// <summary>获取或设置打卡地点纬度（实际纬度的 1000000 倍，GCJ-02 坐标系统标准）。</summary>
    [JsonPropertyName("location_title_lat")]
    public long? LocationTitleLat { get; set; }

    /// <summary>获取或设置打卡地点经度（实际经度的 1000000 倍，GCJ-02 坐标系统标准）。</summary>
    [JsonPropertyName("location_title_lng")]
    public long? LocationTitleLng { get; set; }

    /// <summary>获取或设置打卡规则 id（示例中为 0）。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置排班打卡时间（Unix 时间戳，仅排班打卡有效；可能为 0，如范围外打卡）。</summary>
    [JsonPropertyName("schedule_checkin_time")]
    public long? ScheduleCheckinTime { get; set; }
}
