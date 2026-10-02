// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 截屏/录屏操作记录明细（<c>/cgi-bin/security/get_screen_oper_record</c> 响应 record_list 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ScreenOperRecordItem
{
    /// <summary>
    /// 获取或设置操作时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// 获取或设置企业用户账号 id。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置部门 id（仅当在应用可见范围内才返回）。
    /// </summary>
    [JsonPropertyName("department_id")]
    public int? DepartmentId { get; set; }

    /// <summary>
    /// 获取或设置截屏内容类型：1-聊天 2-通讯录 3-邮件 4-文件 5-日程 6-其他。
    /// </summary>
    [JsonPropertyName("screen_shot_type")]
    public int? ScreenShotType { get; set; }

    /// <summary>
    /// 获取或设置截屏内容。
    /// </summary>
    [JsonPropertyName("screen_shot_content")]
    public string? ScreenShotContent { get; set; }

    /// <summary>
    /// 获取或设置企业用户的操作系统。
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; set; }

    /// <summary>
    /// 获取或设置操作类型：0-截屏 1-录屏。
    /// </summary>
    [JsonPropertyName("operation_type")]
    public int? OperationType { get; set; }

    /// <summary>
    /// 获取或设置设备信息。
    /// </summary>
    [JsonPropertyName("device_info")]
    public ScreenRecordDeviceInfo? DeviceInfo { get; set; }

    /// <summary>
    /// 获取或设置设备 IP 和地址。
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }
}
