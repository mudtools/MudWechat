// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 获取温度检测数据请求体（<c>/cgi-bin/devicedata/get_temperature_data</c>）。
/// <para>
/// 与 <see cref="DeviceDataCheckinDataRequest"/>、<see cref="DeviceDataAccessControlDataRequest"/> 字段面同构，
/// 但 data_filter_type 的取值语义各属本端点参数表，故不共用类型。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataTemperatureDataRequest
{
    /// <summary>获取或设置应用 ID（官方 agentid，官方参数表标注为可选）。</summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>获取或设置成员类型（官方 user_type，必填，数字枚举）。</summary>
    [JsonPropertyName("user_type")]
    public int UserType { get; set; }

    /// <summary>获取或设置设备上传记录的开始时间戳（官方 begin_time，必填）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTimestamp { get; set; }

    /// <summary>获取或设置设备上传记录的结束时间戳（官方 end_time，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTimestamp { get; set; }

    /// <summary>获取或设置温度检测数据筛选条件（官方 data_filter_type，必填，数字枚举）。</summary>
    [JsonPropertyName("data_filter_type")]
    public int DataFilterType { get; set; }

    /// <summary>获取或设置设备序列号列表（官方 device_sn_list，与 open_userid_list 二选一定位范围）。</summary>
    [JsonPropertyName("device_sn_list")]
    public List<string>? DeviceSerialNumberList { get; set; }

    /// <summary>获取或设置成员的 OpenUserId 列表（官方 open_userid_list）。</summary>
    [JsonPropertyName("open_userid_list")]
    public List<string>? OpenUserIdList { get; set; }

    /// <summary>获取或设置分页每页数量（官方 limit）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置翻页标记（官方 cursor，取上一次响应的 next_cursor）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
