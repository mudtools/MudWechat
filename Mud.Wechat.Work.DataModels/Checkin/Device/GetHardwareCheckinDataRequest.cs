// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 获取设备打卡数据请求体（<c>/cgi-bin/hardware/get_hardware_checkin_data</c>；获取考勤设备上产生的原始打卡记录）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：获取记录时间跨度不超过一个月；用户列表不超过 100 个，若超过 100 个请分批获取；
/// 获取的是通过考勤设备打卡的原始记录，不包含企业微信 app 手机打卡的记录；userid 无效时忽略该参数，不报错。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class GetHardwareCheckinDataRequest
{
    /// <summary>获取或设置过滤类型：1 - 按打卡时间过滤；2 - 按设备上传打卡记录的时间过滤（默认为 1；本字段决定 starttime/endtime 的语义）。</summary>
    [JsonPropertyName("filter_type")]
    public int? FilterType { get; set; }

    /// <summary>获取或设置开始时间（Unix 时间戳，官方必填；filter_type 为 1 时表示打卡的开始时间，为 2 时表示设备上传记录的开始时间）。</summary>
    [JsonPropertyName("starttime")]
    public long? Starttime { get; set; }

    /// <summary>获取或设置结束时间（Unix 时间戳，官方必填；filter_type 为 1 时表示打卡的结束时间，为 2 时表示设备上传记录的结束时间）。</summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }

    /// <summary>获取或设置需要获取打卡记录的用户列表（官方必填；不超过 100 个，若超过 100 个请分批获取；userid 无效时忽略该参数不报错）。</summary>
    [JsonPropertyName("useridlist")]
    public List<string>? UserIdList { get; set; }
}
