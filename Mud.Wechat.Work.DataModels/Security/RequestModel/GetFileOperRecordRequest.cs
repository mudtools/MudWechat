// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 查询文件操作记录请求体（<c>/cgi-bin/security/get_file_oper_record</c>）。
/// </summary>
/// <remarks>官方限制：时间跨度不超过 14 天；userid_list 单次最多 100 个；limit 最多 1000。</remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class GetFileOperRecordRequest
{
    /// <summary>
    /// 获取或设置开始时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置结束时间（Unix 秒；与开始时间间隔不超过 14 天）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置文件操作者 userid 列表（单次最多 100 个）。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }

    /// <summary>
    /// 获取或设置操作类型过滤条件。
    /// </summary>
    [JsonPropertyName("operation")]
    public FileOperRecordOperation? Operation { get; set; }

    /// <summary>
    /// 获取或设置分页游标（首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回条数限制（最多 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
