// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 客户联系规则组权限配置（<c>privilege</c>；创建 / 编辑规则组时未传 = 不设置，官方对多数字段默认 true）。
/// </summary>
/// <remarks>
/// 基础权限（<see cref="ViewCustomerList"/> / <see cref="ViewCustomerData"/> / <see cref="ViewRoomList"/> /
/// <see cref="ContactMe"/> / <see cref="JoinRoom"/>）不可取消。
/// </remarks>
public class CustomerStrategyPrivilege
{
    /// <summary>
    /// 获取或设置查看客户列表（基础权限，不可取消）。
    /// </summary>
    [JsonPropertyName("view_customer_list")]
    public bool? ViewCustomerList { get; set; }

    /// <summary>
    /// 获取或设置查看客户统计数据（基础权限，不可取消）。
    /// </summary>
    [JsonPropertyName("view_customer_data")]
    public bool? ViewCustomerData { get; set; }

    /// <summary>
    /// 获取或设置查看群聊列表（基础权限，不可取消）。
    /// </summary>
    [JsonPropertyName("view_room_list")]
    public bool? ViewRoomList { get; set; }

    /// <summary>
    /// 获取或设置可使用联系我（基础权限，不可取消）。
    /// </summary>
    [JsonPropertyName("contact_me")]
    public bool? ContactMe { get; set; }

    /// <summary>
    /// 获取或设置可加入群聊（基础权限，不可取消）。
    /// </summary>
    [JsonPropertyName("join_room")]
    public bool? JoinRoom { get; set; }

    /// <summary>
    /// 获取或设置允许分享客户给其他成员（默认 true）。
    /// </summary>
    [JsonPropertyName("share_customer")]
    public bool? ShareCustomer { get; set; }

    /// <summary>
    /// 获取或设置允许分配离职成员客户（默认 true）。
    /// </summary>
    [JsonPropertyName("oper_resign_customer")]
    public bool? OperResignCustomer { get; set; }

    /// <summary>
    /// 获取或设置允许分配离职成员客户群（默认 true）。
    /// </summary>
    [JsonPropertyName("oper_resign_group")]
    public bool? OperResignGroup { get; set; }

    /// <summary>
    /// 获取或设置允许给企业客户发送消息（默认 true）。
    /// </summary>
    [JsonPropertyName("send_customer_msg")]
    public bool? SendCustomerMsg { get; set; }

    /// <summary>
    /// 获取或设置允许配置欢迎语（默认 true）。
    /// </summary>
    [JsonPropertyName("edit_welcome_msg")]
    public bool? EditWelcomeMsg { get; set; }

    /// <summary>
    /// 获取或设置允许查看成员联系客户统计。
    /// </summary>
    [JsonPropertyName("view_behavior_data")]
    public bool? ViewBehaviorData { get; set; }

    /// <summary>
    /// 获取或设置允许查看群聊数据统计（默认 true）。
    /// </summary>
    [JsonPropertyName("view_room_data")]
    public bool? ViewRoomData { get; set; }

    /// <summary>
    /// 获取或设置允许发送消息到企业的客户群（默认 true）。
    /// </summary>
    [JsonPropertyName("send_group_msg")]
    public bool? SendGroupMsg { get; set; }

    /// <summary>
    /// 获取或设置允许对企业客户群进行去重（默认 true）。
    /// </summary>
    [JsonPropertyName("room_deduplication")]
    public bool? RoomDeduplication { get; set; }

    /// <summary>
    /// 获取或设置配置快捷回复（默认 true）。
    /// </summary>
    [JsonPropertyName("rapid_reply")]
    public bool? RapidReply { get; set; }

    /// <summary>
    /// 获取或设置转接在职成员的客户（默认 true）。
    /// </summary>
    [JsonPropertyName("onjob_customer_transfer")]
    public bool? OnjobCustomerTransfer { get; set; }

    /// <summary>
    /// 获取或设置编辑企业成员防骚扰规则（默认 true）。
    /// </summary>
    [JsonPropertyName("edit_anti_spam_rule")]
    public bool? EditAntiSpamRule { get; set; }

    /// <summary>
    /// 获取或设置导出客户列表（默认 true）。
    /// </summary>
    [JsonPropertyName("export_customer_list")]
    public bool? ExportCustomerList { get; set; }

    /// <summary>
    /// 获取或设置导出成员客户统计（默认 true）。
    /// </summary>
    [JsonPropertyName("export_customer_data")]
    public bool? ExportCustomerData { get; set; }

    /// <summary>
    /// 获取或设置导出客户群列表（默认 true）。
    /// </summary>
    [JsonPropertyName("export_customer_group_list")]
    public bool? ExportCustomerGroupList { get; set; }

    /// <summary>
    /// 获取或设置配置企业客户标签（默认 true）。
    /// </summary>
    [JsonPropertyName("manage_customer_tag")]
    public bool? ManageCustomerTag { get; set; }
}
