// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 管理端操作日志明细（<c>/cgi-bin/security/admin_oper_log/list</c> 响应 record_list 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class AdminOperLogItem
{
    /// <summary>
    /// 获取或设置操作时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// 获取或设置操作者 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置操作类型：2-权限管理变更 3-成员与部门变更 7-其它 8-应用变更
    /// 11-通讯录与聊天管理 12-企业信息管理 13-外部联系人管理。
    /// </summary>
    [JsonPropertyName("oper_type")]
    public int? OperType { get; set; }

    /// <summary>
    /// 获取或设置操作行为（详见官方行为类型表，如 1-绑定手机 14-新增成员 23-登录后台 161-重置Secret 等）。
    /// </summary>
    [JsonPropertyName("detail_type")]
    public int? DetailType { get; set; }

    /// <summary>
    /// 获取或设置相关数据。
    /// </summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>
    /// 获取或设置操作者 IP。
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }
}
