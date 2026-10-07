// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Dial;

/// <summary>
/// 获取公费电话拨打记录请求体（<c>/cgi-bin/dial/get_dial_record</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Dial")]
public class GetDialRecordRequest
{
    /// <summary>获取或设置查询的起始时间戳（官方 start_time，秒级；查询范围为 [start_time, end_time] 双闭区间）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置查询的结束时间戳（官方 end_time，秒级；与 start_time 均指定时结束时间不得小于开始时间，否则返回 600018 无效的起止时间）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置分页查询的偏移量（官方 offset）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>获取或设置分页查询的每页大小（官方 limit，默认 100 条，大于 100 按 100 处理）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
