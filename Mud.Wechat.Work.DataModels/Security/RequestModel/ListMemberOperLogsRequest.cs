// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 获取成员操作记录请求体（<c>/cgi-bin/security/member_oper_log/list</c>）。
/// </summary>
/// <remarks>
/// 官方限制：时间跨度不超过 7 天且不早于 180 天前；limit 取值 1~400（默认 400）；
/// 频率限制 600 次/分钟；不同过滤条件的游标不能混用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ListMemberOperLogsRequest
{
    /// <summary>
    /// 获取或设置开始时间（Unix 秒；不早于 180 天前）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置结束时间（Unix 秒；需大于开始时间且小于当前时间，跨度不超过 7 天）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置操作类型过滤（不填表示全部）：1-添加外部联系人 2-删除外部联系人 3-标记企业客户
    /// 4-新设备登录 5-更换手机号 6-绑定微信号 7-换绑微信号 8-邀请成员 9-封禁登录 11-修改昵称
    /// 12-修改姓名 13-副设备登录 15-确认高级功能订单 16-应用变更 17-确认会话内容存档订单
    /// 20-封禁互通 21-锁定设备。
    /// </summary>
    [JsonPropertyName("oper_type")]
    public int? OperType { get; set; }

    /// <summary>
    /// 获取或设置操作者 userid 过滤（需在应用可见范围内，可不填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置分页游标（不填表示首页）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置最大记录数（取值 1~400，默认 400）。
    /// <para>官方提示不保证每次返回刚好为指定 limit，必须以返回的 has_more 判断是否继续请求。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
