// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 成员变更事件载荷（<b>结构族</b>：覆盖 <c>create_user</c> / <c>update_user</c> / <c>delete_user</c>；官方 90970）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：三个事件的官方报文<b>结构同一</b>（<c>delete_user</c> 仅
/// <c>UserID</c> 命中，其余字段为 <c>null</c>），具体变更类别由信封 <c>ChangeType</c> 判别
/// ⇒ 3 个旧 DTO 收敛为 1 个载荷。
/// </para>
/// <para>
/// <b>权限分层（处理器不得假设必有值）</b>：2022-08-15 后通讯录助手新配置 URL 仅回调
/// <c>UserID</c>/<c>Department</c> 子集；<c>Name</c>/<c>Mobile</c>/<c>Gender</c>/<c>Email</c>/
/// <c>BizMail</c>/<c>Avatar</c>/<c>Alias</c>/<c>Telephone</c>/<c>Address</c>/<c>Position</c>
/// 需「管理员授权 / 成员 oauth2 授权 / 第三方通讯录应用」才返回；<c>BizMail</c>/<c>Avatar</c>
/// 代开发应用不可获取。
/// </para>
/// <para>
/// <b>三模式共用一份可空超集（ADR-14）</b>：三种应用模式的差异是「值是否出现」而非「报文结构不同」，
/// 故本载荷同时服务企业自建 / 第三方 / 服务商代开发，<b>不得</b>按应用模式分叉。
/// </para>
/// <para>
/// 字段映射由上游 <c>PayloadFieldMapGenerator</c> 依 <c>[PayloadContract]</c>/<c>[PayloadField]</c> 生成；
/// 元素名与属性名的配对受<b>编译期校验</b>（改名/漏项即 <c>PAYLOAD004/006/007</c>）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class ContactUserChangedPayload : WechatCallbackPayload
{
    /// <summary>成员 UserId（官方 <c>UserID</c>；成员账号唯一标识）。</summary>
    [PayloadField("UserID")]
    public string? UserId { get; set; }

    /// <summary>新成员 UserId（官方 <c>NewUserID</c>；仅 <c>update_user</c> 且 UserId 变更时命中）。</summary>
    [PayloadField("NewUserID")]
    public string? NewUserId { get; set; }

    /// <summary>成员名称（官方 <c>Name</c>；需授权）。</summary>
    [PayloadField("Name")]
    public string? Name { get; set; }

    /// <summary>所属部门 id 列表（官方 <c>Department</c> 为逗号分隔串 <c>"1,2,3"</c>）。</summary>
    [PayloadField("Department", Format = PayloadFieldFormat.Delimited, Separator = ',')]
    public List<long> DepartmentIds { get; set; } = new List<long>();

    /// <summary>主部门（官方 <c>MainDepartment</c>）。</summary>
    [PayloadField("MainDepartment")]
    public string? MainDepartmentId { get; set; }

    /// <summary>在部门内的任职领导（官方 <c>IsLeaderInDept</c>，与 <see cref="DepartmentIds"/> 同序对应）。</summary>
    [PayloadField("IsLeaderInDept", Format = PayloadFieldFormat.Delimited)]
    public List<int> LeaderInDeptFlags { get; set; } = new List<int>();

    /// <summary>直属上级 UserId 列表（官方 <c>DirectLeader</c> 为竖线分隔串；仅第三方通讯录应用可获取）。</summary>
    [PayloadField("DirectLeader", Format = PayloadFieldFormat.Delimited, Separator = '|')]
    public List<string> DirectLeaderIds { get; set; } = new List<string>();

    /// <summary>职务信息（官方 <c>Position</c>；需对应部门/成员权限）。</summary>
    [PayloadField("Position")]
    public string? Position { get; set; }

    /// <summary>手机号码（官方 <c>Mobile</c>；需管理员授权且成员在应用可见范围内）。</summary>
    [PayloadField("Mobile")]
    public string? Mobile { get; set; }

    /// <summary>性别（官方 <c>Gender</c>：1 = 男性，2 = 女性）。</summary>
    [PayloadField("Gender", Method = nameof(WechatPayloadConverter.ParseGender))]
    public WechatUserGender? Gender { get; set; }

    /// <summary>邮箱（官方 <c>Email</c>；需管理员授权）。</summary>
    [PayloadField("Email")]
    public string? Email { get; set; }

    /// <summary>企业邮箱（官方 <c>BizMail</c>；需管理员授权，代开发应用不可获取）。</summary>
    [PayloadField("BizMail")]
    public string? BizMail { get; set; }

    /// <summary>激活状态（官方 <c>Status</c>：1 已激活 / 2 已禁用 / 4 未激活 / 5 退出企业）。</summary>
    [PayloadField("Status", Method = nameof(WechatPayloadConverter.ParseStatus))]
    public WechatUserStatus? Status { get; set; }

    /// <summary>头像 URL（官方 <c>Avatar</c>；需管理员授权，代开发应用不可获取）。</summary>
    [PayloadField("Avatar")]
    public string? Avatar { get; set; }

    /// <summary>别名（官方 <c>Alias</c>；第三方通讯录应用或管理员授权才返回）。</summary>
    [PayloadField("Alias")]
    public string? Alias { get; set; }

    /// <summary>座机号（官方 <c>Telephone</c>；需管理员授权且成员在应用可见范围内）。</summary>
    [PayloadField("Telephone")]
    public string? Telephone { get; set; }

    /// <summary>地址（官方 <c>Address</c>；需管理员授权）。</summary>
    [PayloadField("Address")]
    public string? Address { get; set; }

    /// <summary>
    /// 扩展属性列表（官方 <c>ExtAttr</c>：<c>&lt;Item Name Type&gt;&lt;Text/&gt;&lt;/Item&gt;</c> 形态；
    /// 需管理员授权，报文无该节点时为空列表）。
    /// </summary>
    [PayloadField("ExtAttr", Format = PayloadFieldFormat.ItemsWithAttributes,
                  ItemName = "Item", NameAttribute = "Name", ValueElement = "Text")]
    public List<WechatCallbackExtAttrItem> ExtAttr { get; set; } = new List<WechatCallbackExtAttrItem>();
}
