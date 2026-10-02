// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 成员操作记录明细（<c>/cgi-bin/security/member_oper_log/list</c> 响应 record_list 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class MemberOperLogItem
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
    /// 获取或设置操作类型：1-添加外部联系人 2-删除外部联系人 3-标记企业客户 4-新设备登录
    /// 5-更换手机号 6-绑定微信号 7-换绑微信号 8-邀请成员 9-封禁登录 11-修改昵称 12-修改姓名
    /// 13-副设备登录 15-确认高级功能订单 16-应用变更 17-确认会话内容存档订单 20-封禁互通 21-锁定设备。
    /// </summary>
    [JsonPropertyName("oper_type")]
    public int? OperType { get; set; }

    /// <summary>
    /// 获取或设置相关数据（如姓名、手机号等）。
    /// </summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>
    /// 获取或设置操作者 IP。
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }
}
