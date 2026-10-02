// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 查询截屏/录屏操作记录请求体（<c>/cgi-bin/security/get_screen_oper_record</c>）。
/// </summary>
/// <remarks>
/// 官方限制：时间跨度不超过 14 天；userid_list / department_id_list 单次各最多 100 个（须在应用可见范围内）；
/// limit 最多 1000。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class GetScreenOperRecordRequest
{
    /// <summary>
    /// 获取或设置开始时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置结束时间（Unix 秒；与开始时间跨度不超过 14 天）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置截屏操作者的 userid 列表（单次最多 100 个，须在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }

    /// <summary>
    /// 获取或设置操作者部门 id 列表（单次最多 100 个，须在应用可见范围内）。
    /// </summary>
    [JsonPropertyName("department_id_list")]
    public List<int>? DepartmentIdList { get; set; }

    /// <summary>
    /// 获取或设置内容类型过滤：1-聊天 2-通讯录 3-邮件 4-文件 5-日程 6-其他（不设置默认全部）。
    /// </summary>
    [JsonPropertyName("screen_shot_type")]
    public int? ScreenShotType { get; set; }

    /// <summary>
    /// 获取或设置分页游标（首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回条数（最多 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
