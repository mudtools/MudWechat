// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 添加打卡记录单项（添加打卡记录请求 <c>records</c> 元素，一批最多 200 个）。
/// </summary>
/// <remarks>
/// <para>官方参数表将顶层参数（records）与 records 元素字段混在一张表里且未标注嵌套关系，嵌套结构以官方请求示例为准。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinAddRecordItem
{
    /// <summary>获取或设置用户 id（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置打卡时间（Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("checkin_time")]
    public long? CheckinTime { get; set; }

    /// <summary>获取或设置打卡地点 title（官方必填；限制 1024 字符）。</summary>
    [JsonPropertyName("location_title")]
    public string? LocationTitle { get; set; }

    /// <summary>获取或设置打卡地点详情（官方必填；限制 1024 字符）。</summary>
    [JsonPropertyName("location_detail")]
    public string? LocationDetail { get; set; }

    /// <summary>获取或设置打卡备注（限制 1024 字符）。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置打卡 wifi 名称（限制 1024 字符；传入 wifiname 时 wifimac 必填）。</summary>
    [JsonPropertyName("wifiname")]
    public string? Wifiname { get; set; }

    /// <summary>获取或设置打卡的 MAC 地址/bssid（须满足六段冒号分隔正则表达式；传入 wifiname 时必填）。</summary>
    [JsonPropertyName("wifimac")]
    public string? Wifimac { get; set; }

    /// <summary>获取或设置打卡的附件 media_id 列表（可使用 media/upload 上传附件；当前最多只允许传 1 个）。</summary>
    [JsonPropertyName("mediaids")]
    public List<string>? Mediaids { get; set; }

    /// <summary>获取或设置位置打卡地点纬度（实际纬度的 1000000 倍，GCJ-02 坐标系统标准；范围 -90000000 ~ 90000000）。</summary>
    [JsonPropertyName("lat")]
    public long? Lat { get; set; }

    /// <summary>获取或设置位置打卡地点经度（实际经度的 1000000 倍，GCJ-02 坐标系统标准；范围 -180000000 ~ 180000000）。</summary>
    [JsonPropertyName("lng")]
    public long? Lng { get; set; }

    /// <summary>获取或设置打卡设备类型（官方必填）：1 - 门禁；2 - 考勤机（人脸识别、指纹识别）；3 - 其他。</summary>
    [JsonPropertyName("device_type")]
    public int? DeviceType { get; set; }

    /// <summary>获取或设置打卡设备品牌（官方必填；字符串写入，限制 40 个字符内）。</summary>
    [JsonPropertyName("device_detail")]
    public string? DeviceDetail { get; set; }
}
