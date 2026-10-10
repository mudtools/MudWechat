// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取配置的专员与客户群响应体（<c>/cgi-bin/kf/customer/get_upgrade_service_config</c>）。
/// <para>
/// 返回企业在「微信客服」-「升级服务」中配置的专员与客户群范围，
/// API 升级接口（<c>upgrade_service</c>）仅可从该已配置范围中选取（否则官方返回 95021 错误码）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfUpgradeServiceConfigResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置专员服务配置范围。
    /// </summary>
    [JsonPropertyName("member_range")]
    public KfUpgradeMemberRange? MemberRange { get; set; }

    /// <summary>
    /// 获取或设置客户群配置范围。
    /// </summary>
    [JsonPropertyName("groupchat_range")]
    public KfUpgradeGroupchatRange? GroupchatRange { get; set; }
}

/// <summary>
/// 升级服务的专员配置范围（<see cref="GetKfUpgradeServiceConfigResponse.MemberRange"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfUpgradeMemberRange
{
    /// <summary>
    /// 获取或设置专员 userid 列表。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UserIdList { get; set; }

    /// <summary>
    /// 获取或设置专员部门列表。
    /// <para>
    /// 官方参数表写作 <c>department_list</c>、官方 JSON 示例与自建应用文档均为 <c>department_id_list</c>，
    /// 此处按官方 JSON 示例为准（对齐 admin_oper_log 游标 cusor/cursor 的处置先例）。
    /// </para>
    /// </summary>
    [JsonPropertyName("department_id_list")]
    public List<int>? DepartmentIdList { get; set; }
}

/// <summary>
/// 升级服务的客户群配置范围（<see cref="GetKfUpgradeServiceConfigResponse.GroupchatRange"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfUpgradeGroupchatRange
{
    /// <summary>
    /// 获取或设置客户群 id 列表。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }
}
