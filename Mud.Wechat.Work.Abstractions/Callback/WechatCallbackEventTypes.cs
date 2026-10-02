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
/// 企业微信回调的事件类型分布在三个信封段上（v1 方案 §5.4.2）：
/// 授权族走 <c>InfoType</c>、通讯录变更族走 <c>ChangeType</c>（<c>Event = change_contact</c>）、
/// 异步任务族与上下游任务走 <c>Event</c>、上下游变更族走 <c>ChangeType</c>（<c>Event = change_chain</c>）。
/// 处理器 <see cref="IWechatCallbackEventHandler.SupportedEventType"/> 必须填本类常量之一（或空串 = 兜底）。
/// </para>
/// <para>
/// <b>官方文档索引</b>（便于逐字段核查）：
/// 回调配置 <see href="https://developer.work.weixin.qq.com/document/path/90930">path 90930</see> ·
/// 授权族 <see href="https://developer.work.weixin.qq.com/document/path/90628">path 90628</see>（suite_ticket）/ <see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487</see>（授权通知事件）/ <see href="https://developer.work.weixin.qq.com/document/path/100964">path 100964</see>（修改授权通知）·
/// 通讯录变更族 <see href="https://developer.work.weixin.qq.com/document/path/90967">path 90967</see>（概述）·
/// 上下游变更族 <see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796</see> ·
/// 异步任务族 <see href="https://developer.work.weixin.qq.com/document/path/90973">path 90973</see>（通讯录）/ <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797</see>（上下游）。
/// </para>
/// <para>
/// 契约守卫 CB2（<c>WechatCallbackContractGuards</c>）按本类断言官方事件键全覆盖，新增官方事件键须同批登记。
/// </para>
/// </remarks>
public static class WechatCallbackEventTypes
{
    // ——— 授权族（InfoType） ———

    /// <summary>suite_ticket 推送（每 10 分钟一次，驱动 get_suite_token）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90628">path 90628 推送 suite_ticket</see>。
    /// </remarks>
    public const string SuiteTicket = "suite_ticket";

    /// <summary>授权成功（携带一次性 auth_code）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487 授权通知事件</see>。
    /// </remarks>
    public const string CreateAuth = "create_auth";

    /// <summary>重置永久授权码（代开发 secret 重置，携带 auth_code）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487 授权通知事件</see>。
    /// </remarks>
    public const string ResetPermanentCode = "reset_permanent_code";

    /// <summary>授权变更（应用管理员修改权限等）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100964">path 100964 修改授权通知事件</see>。
    /// </remarks>
    public const string ChangeAuth = "change_auth";

    /// <summary>取消授权。</summary>
    /// <remarks>
    /// 官方文档：服务商应用授权事件回调（授权通知事件
    /// <see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487</see>）。
    /// </remarks>
    public const string CancelAuth = "cancel_auth";

    /// <summary>授权删除（与 cancel_auth 同处置语义）。</summary>
    /// <remarks>官方文档：服务商应用授权事件回调（同 cancel_auth）。</remarks>
    public const string DelAuth = "del_auth";

    // ——— 通讯录变更族（ChangeType；Event = ChangeContact） ———

    /// <summary>新增成员。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90970">path 90970 成员变更通知</see>。
    /// </remarks>
    public const string CreateUser = "create_user";

    /// <summary>更新成员（2022-08-15 后新 URL 仅部门相关/UserId 变更触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90970">path 90970 成员变更通知</see>。
    /// </remarks>
    public const string UpdateUser = "update_user";

    /// <summary>删除成员。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90970">path 90970 成员变更通知</see>。
    /// </remarks>
    public const string DeleteUser = "delete_user";

    /// <summary>新增部门。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
    /// </remarks>
    public const string CreateParty = "create_party";

    /// <summary>更新部门（仅 ParentId 变更触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
    /// </remarks>
    public const string UpdateParty = "update_party";

    /// <summary>删除部门。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
    /// </remarks>
    public const string DeleteParty = "delete_party";

    /// <summary>标签成员变更（与成员事件时序不保证，须拉取接口对齐）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90972">path 90972 标签变更通知</see>。
    /// </remarks>
    public const string UpdateTag = "update_tag";

    /// <summary>通讯录变更事件的 <c>Event</c> 信封值。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90967">path 90967 通讯录回调概述</see>。
    /// </remarks>
    public const string ChangeContact = "change_contact";

    // ——— 异步任务族（Event） ———

    /// <summary>异步任务完成通知（JobType：sync_user / replace_user / invite_user / replace_party /
    /// import_chain_contact——上下游联系人导入，报文字段包在 BatchJob 节点内）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90973">path 90973 异步任务完成通知</see>（通讯录）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797 异步任务完成通知</see>（上下游，BatchJob 包装布局）。
    /// </remarks>
    public const string BatchJobResult = "batch_job_result";

    // ——— 上下游族（官方 95796；Event = ChangeChain，仅自建应用可配置接收） ———

    /// <summary>上下游变更事件的 <c>Event</c> 信封值（具体变更类别看 ChangeType）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string ChangeChain = "change_chain";

    /// <summary>创建上下游空间。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string CreateChain = "create_chain";

    /// <summary>更新上下游空间。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string UpdateChain = "update_chain";

    /// <summary>删除上下游空间。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string DeleteChain = "delete_chain";

    /// <summary>新增上下游分组（携带 GroupIds 分组 id 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string CreateGroup = "create_group";

    /// <summary>更新上下游分组（携带 GroupIds）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string UpdateGroup = "update_group";

    /// <summary>删除上下游分组（携带 GroupIds）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string DeleteGroup = "delete_group";

    /// <summary>企业加入上下游（携带 CorpIds 企业 id 列表；仅已加入上下游的企业会产生对应事件）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string CorpJoin = "corp_join";

    /// <summary>更新企业（变更企业分组时触发；携带 CorpIds）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string UpdateCorp = "update_corp";

    /// <summary>移除企业（携带 CorpIds）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public const string RemoveCorp = "remove_corp";
}
