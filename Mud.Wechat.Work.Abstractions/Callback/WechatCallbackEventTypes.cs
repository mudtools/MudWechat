// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback;

/// <summary>
/// 企业微信回调官方事件键常量（<see cref="WechatCallbackEvent.EventTypeKey"/> 的全部合法取值域）。
/// </summary>
/// <remarks>
/// <para>
/// 企业微信回调的事件类型分布在三个信封段上（v1 方案 §5.4.2 / 官方 90930/90970/90971/90972/90973）：
/// 授权族走 <c>InfoType</c>、通讯录变更族走 <c>ChangeType</c>（<c>Event = change_contact</c>）、
/// 异步任务族走 <c>Event</c>（<c>batch_job_result</c>）。处理器
/// <see cref="IWechatCallbackEventHandler.SupportedEventType"/> 必须填本类常量之一（或空串 = 兜底）。
/// </para>
/// <para>
/// 契约守卫 CB2（<c>WechatCallbackContractGuards</c>）按本类断言官方事件键全覆盖，新增官方事件键须同批登记。
/// </para>
/// </remarks>
public static class WechatCallbackEventTypes
{
    // ——— 授权族（InfoType） ———

    /// <summary>suite_ticket 推送（每 10 分钟一次，驱动 get_suite_token）。</summary>
    public const string SuiteTicket = "suite_ticket";

    /// <summary>授权成功（携带一次性 auth_code）。</summary>
    public const string CreateAuth = "create_auth";

    /// <summary>重置永久授权码（代开发 secret 重置，携带 auth_code）。</summary>
    public const string ResetPermanentCode = "reset_permanent_code";

    /// <summary>授权变更（应用管理员修改权限等）。</summary>
    public const string ChangeAuth = "change_auth";

    /// <summary>取消授权。</summary>
    public const string CancelAuth = "cancel_auth";

    /// <summary>授权删除（与 cancel_auth 同处置语义）。</summary>
    public const string DelAuth = "del_auth";

    // ——— 通讯录变更族（ChangeType；Event = ChangeContact） ———

    /// <summary>新增成员。</summary>
    public const string CreateUser = "create_user";

    /// <summary>更新成员（2022-08-15 后新 URL 仅部门相关/UserId 变更触发）。</summary>
    public const string UpdateUser = "update_user";

    /// <summary>删除成员。</summary>
    public const string DeleteUser = "delete_user";

    /// <summary>新增部门。</summary>
    public const string CreateParty = "create_party";

    /// <summary>更新部门（仅 ParentId 变更触发）。</summary>
    public const string UpdateParty = "update_party";

    /// <summary>删除部门。</summary>
    public const string DeleteParty = "delete_party";

    /// <summary>标签成员变更（与成员事件时序不保证，须拉取接口对齐）。</summary>
    public const string UpdateTag = "update_tag";

    /// <summary>通讯录变更事件的 <c>Event</c> 信封值。</summary>
    public const string ChangeContact = "change_contact";

    // ——— 异步任务族（Event） ———

    /// <summary>异步任务完成通知（JobType：sync_user / replace_user / invite_user / replace_party）。</summary>
    public const string BatchJobResult = "batch_job_result";
}
