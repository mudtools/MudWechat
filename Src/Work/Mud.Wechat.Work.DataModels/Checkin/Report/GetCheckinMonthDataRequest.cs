// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 获取打卡月报数据请求体（<c>/cgi-bin/checkin/getcheckin_monthdata</c>；三种应用类型请求形态一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class GetCheckinMonthDataRequest
{
    /// <summary>获取或设置获取月报的开始时间（0 点 Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("starttime")]
    public long? Starttime { get; set; }

    /// <summary>获取或设置获取月报的结束时间（0 点 Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }

    /// <summary>获取或设置获取月报的 userid 列表（官方必填；单个 userid 不少于 1 字节、不多于 64 字节，可填充个数 1~100）。</summary>
    [JsonPropertyName("useridlist")]
    public List<string>? UserIdList { get; set; }
}
