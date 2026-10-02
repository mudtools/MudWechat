// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 成员扩展属性项（<c>create_user</c>/<c>update_user</c> 报文 <c>ExtAttr/Item</c> 节点）。
/// </summary>
public class WechatCallbackExtAttrItem
{
    /// <summary>扩展属性名称（Item 节点 Name 属性）。</summary>
    public string? Name { get; set; }

    /// <summary>扩展属性类型（Item 节点 Type 属性：0 = 文本，1 = 网页）。</summary>
    public string? Type { get; set; }

    /// <summary>扩展属性值（Text 子节点；网页类型为 url）。</summary>
    public string? Value { get; set; }
}

/// <summary>
/// 新增成员事件（<c>change_contact</c> + <c>create_user</c>；官方 90970）。
/// </summary>
/// <remarks>
/// <para>
/// <b>权限分层（2022-08-15 后新配置 URL）</b>：通讯录助手仅回调 <c>UserID</c>/<c>Department</c> 子集；
/// <c>Name</c>/<c>Mobile</c>/<c>Gender</c>/<c>Email</c>/<c>BizMail</c>/<c>Avatar</c>/<c>Alias</c>/
/// <c>Telephone</c>/<c>Address</c>/<c>Position</c> 等敏感字段需「管理员授权 / 成员 oauth2 授权 /
/// 第三方通讯录应用」才返回——处理器<b>不得假设可空字段必有值</b>。
/// </para>
/// <para>
/// 列表型字段按官方报文<b>字符串承载</b>（v1 方案 §5.5 复用边界②）：<see cref="Department"/> 为逗号分隔
/// 部门 id（如 <c>"1,2,3"</c>），<see cref="IsLeaderInDept"/> 为与 <see cref="Department"/> 顺序对应的
/// 逗号分隔 0/1 串，<see cref="DirectLeader"/> 为竖线分隔上级 UserId；转换助手见
/// <see cref="WechatCallbackEventParser.ParseIdList"/>/<see cref="WechatCallbackEventParser.ParseTextList"/>。
/// </para>
/// </remarks>
public class UserCreatedEvent
{
    /// <summary>新成员 UserId（成员账号唯一标识）。</summary>
    public string? UserID { get; set; }

    /// <summary>成员名称。</summary>
    public string? Name { get; set; }

    /// <summary>成员所属部门列表（逗号分隔部门 id；父部门无需 own_department 权限即可获取）。</summary>
    public string? Department { get; set; }

    /// <summary>主部门（逗号分隔串中的首个部门 id）。</summary>
    public string? MainDepartment { get; set; }

    /// <summary>在部门内的任职领导（与 <see cref="Department"/> 顺序对应的逗号分隔 0/1 串）。</summary>
    public string? IsLeaderInDept { get; set; }

    /// <summary>直属上级（竖线分隔的上级 UserId 列表，仅第三方通讯录应用可获取）。</summary>
    public string? DirectLeader { get; set; }

    /// <summary>职务信息（需对应部门/成员权限）。</summary>
    public string? Position { get; set; }

    /// <summary>手机号码（需管理员授权且成员在应用可见范围内）。</summary>
    public string? Mobile { get; set; }

    /// <summary>性别（1 = 男性，2 = 女性；需管理员授权）。</summary>
    public string? Gender { get; set; }

    /// <summary>邮箱（需管理员授权）。</summary>
    public string? Email { get; set; }

    /// <summary>企业邮箱（需管理员授权；代开发应用不可获取）。</summary>
    public string? BizMail { get; set; }

    /// <summary>激活状态（1=已激活，2=已禁用，4=未激活，5=退出企业；需管理员授权）。</summary>
    public string? Status { get; set; }

    /// <summary>头像 URL（需管理员授权；代开发应用不可获取）。</summary>
    public string? Avatar { get; set; }

    /// <summary>别名（第三方通讯录应用或管理员授权才返回）。</summary>
    public string? Alias { get; set; }

    /// <summary>座机号（需管理员授权且成员在应用可见范围内）。</summary>
    public string? Telephone { get; set; }

    /// <summary>地址（需管理员授权）。</summary>
    public string? Address { get; set; }

    /// <summary>扩展属性列表（需管理员授权；报文无该节点时为空列表）。</summary>
    public List<WechatCallbackExtAttrItem> ExtAttr { get; set; } = new();
}

/// <summary>
/// 更新成员事件（<c>change_contact</c> + <c>update_user</c>；官方 90970）。
/// </summary>
/// <remarks>
/// 2022-08-15 后新配置 URL：仅部门相关变更或 UserId 变更触发；字段可空性与
/// <see cref="UserCreatedEvent"/> 同源（权限分层，不得假设必有值）。
/// </remarks>
public class UserUpdatedEvent
{
    /// <summary>原成员 UserId。</summary>
    public string? UserID { get; set; }

    /// <summary>新成员 UserId（仅 UserId 变更时非空）。</summary>
    public string? NewUserID { get; set; }

    /// <summary>成员名称。</summary>
    public string? Name { get; set; }

    /// <summary>成员所属部门列表（逗号分隔部门 id）。</summary>
    public string? Department { get; set; }

    /// <summary>主部门。</summary>
    public string? MainDepartment { get; set; }

    /// <summary>在部门内的任职领导（与 <see cref="Department"/> 顺序对应的逗号分隔 0/1 串）。</summary>
    public string? IsLeaderInDept { get; set; }

    /// <summary>直属上级（竖线分隔的上级 UserId 列表，仅第三方通讯录应用可获取）。</summary>
    public string? DirectLeader { get; set; }

    /// <summary>职务信息。</summary>
    public string? Position { get; set; }

    /// <summary>手机号码（需管理员授权）。</summary>
    public string? Mobile { get; set; }

    /// <summary>性别（1 = 男性，2 = 女性）。</summary>
    public string? Gender { get; set; }

    /// <summary>邮箱。</summary>
    public string? Email { get; set; }

    /// <summary>企业邮箱（代开发应用不可获取）。</summary>
    public string? BizMail { get; set; }

    /// <summary>激活状态（1=已激活，2=已禁用，4=未激活，5=退出企业）。</summary>
    public string? Status { get; set; }

    /// <summary>头像 URL（代开发应用不可获取）。</summary>
    public string? Avatar { get; set; }

    /// <summary>别名（第三方通讯录应用或管理员授权才返回）。</summary>
    public string? Alias { get; set; }

    /// <summary>座机号。</summary>
    public string? Telephone { get; set; }

    /// <summary>地址。</summary>
    public string? Address { get; set; }

    /// <summary>扩展属性列表（报文无该节点时为空列表）。</summary>
    public List<WechatCallbackExtAttrItem> ExtAttr { get; set; } = new();
}

/// <summary>
/// 删除成员事件（<c>change_contact</c> + <c>delete_user</c>；官方 90970）。
/// </summary>
public class UserDeletedEvent
{
    /// <summary>被删除成员的 UserId。</summary>
    public string? UserID { get; set; }
}
