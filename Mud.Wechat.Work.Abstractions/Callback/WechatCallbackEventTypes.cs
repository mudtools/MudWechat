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
/// 异步任务族与上下游任务走 <c>Event</c>、上下游变更族走 <c>ChangeType</c>（<c>Event = change_chain</c>）、
/// 客户联系/获客助手族与邮箱族以<b>族事件值</b>为事件键（<c>Event</c> 信封取 <c>Event</c> 节点、
/// 第三方套件信封取 <c>InfoType</c> 节点，具体类别由 <c>ChangeType</c> 判别；邮箱族
/// <c>receive_email</c> 在应用邮箱/公共邮箱两族同名，逐 <c>ChangeType</c> 键无法消歧）。
/// 处理器 <see cref="IWechatCallbackEventHandler.SupportedEventType"/> 必须填本类常量之一（或空串 = 兜底）。
/// </para>
/// <para>
/// <b>官方文档索引</b>（便于逐字段核查）：
/// 回调配置 <see href="https://developer.work.weixin.qq.com/document/path/90930">path 90930</see> ·
/// 授权族 <see href="https://developer.work.weixin.qq.com/document/path/90628">path 90628</see>（suite_ticket）/ <see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487</see>（授权通知事件）/ <see href="https://developer.work.weixin.qq.com/document/path/100964">path 100964</see>（修改授权通知）·
/// 通讯录变更族 <see href="https://developer.work.weixin.qq.com/document/path/90967">path 90967</see>（概述）·
/// 客户联系变更族 <see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130</see>（企业自建）/ <see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/96361">path 96361</see>（代开发）·
/// 获客助手族 <see href="https://developer.work.weixin.qq.com/document/path/97299">path 97299</see>（企业自建）/ <see href="https://developer.work.weixin.qq.com/document/path/97402">path 97402</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/99485">path 99485</see>（第三方·组件）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98958">path 98958</see>（代开发）·
/// 上下游变更族 <see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796</see> ·
/// 异步任务族 <see href="https://developer.work.weixin.qq.com/document/path/90973">path 90973</see>（通讯录）/ <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797</see>（上下游）·
/// 安全事件族 <see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080</see>（域名IP变更，仅自建）·
/// 微信客服族 <see href="https://developer.work.weixin.qq.com/document/path/94670">path 94670</see>（接收消息与事件·自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97712">path 97712</see>（回调通知）·
/// 消息与事件（关注/菜单/地理位置/审批/共享/模板卡片/应用状态）<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240</see>（企业内部开发）/
/// <see href="https://developer.work.weixin.qq.com/document/path/90376">path 90376</see>（第三方）/ <see href="https://developer.work.weixin.qq.com/document/path/96468">path 96468</see>（服务商代开发）——
/// 三份文档正文逐字一致，故同一事件键在三种应用模式下共用一个常量。
/// 应用版本付费订单回调族（<see href="https://developer.work.weixin.qq.com/document/path/91929">path 91929</see> 下单成功/
/// <see href="https://developer.work.weixin.qq.com/document/path/91930">path 91930</see> 改单/
/// <see href="https://developer.work.weixin.qq.com/document/path/91931">path 91931</see> 支付成功/
/// <see href="https://developer.work.weixin.qq.com/document/path/91932">path 91932</see> 退款/
/// <see href="https://developer.work.weixin.qq.com/document/path/91933">path 91933</see> 应用版本变更/
/// <see href="https://developer.work.weixin.qq.com/document/path/99353">path 99353</see> 取消订单）·
/// 邮箱族 <see href="https://developer.work.weixin.qq.com/document/path/97495">path 97495</see>（应用邮箱，企业自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97517">path 97517</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97506">path 97506</see>（代开发）/
/// <see href="https://developer.work.weixin.qq.com/document/path/100180">path 100180</see>（公共邮箱，仅自建）——
/// 邮箱族以<b>族事件值</b>为事件键（<c>receive_email</c> 跨族同名）·
/// 文档族 <see href="https://developer.work.weixin.qq.com/document/path/97833">path 97833</see>（企业自建，起）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97839">path 97839</see>（第三方，起）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97836">path 97836</see>（代开发，起）·
/// 智能表格族 <see href="https://developer.work.weixin.qq.com/document/path/100986">path 100986</see>（企业自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018</see>（代开发）·
/// 日程族 <see href="https://developer.work.weixin.qq.com/document/path/97728">path 97728</see>（企业自建，起）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97806">path 97806</see>（第三方，起）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97771">path 97771</see>（代开发，起）·
/// 会议族 <see href="https://developer.work.weixin.qq.com/document/path/99081">path 99081</see>（企业自建，起）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97451">path 97451</see>（第三方，修改/取消合页）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97459">path 97459</see>（代开发，同 97451）。
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

    // ——— 安全事件族（官方 100080；Event = Security，仅自建应用可配置接收） ———

    /// <summary>安全管理事件的 <c>Event</c> 信封值（具体变更类别看 ChangeType）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080 域名IP变更事件</see>。
    /// </remarks>
    public const string Security = "security";

    /// <summary>
    /// 企业微信域名IP变更（官方 <c>ChangeType = change_domain_ip</c>；信封之外无业务字段）。
    /// <para>企业微信的域名或 IP 发生变更时回调；自建应用须配置到「我的企业 - 设置 - 域名IP - 可调用API的应用」，
    /// <b>第三方 / 代开发应用暂不支持</b>（官方权限表明示）。处理器收到后应刷新本地缓存的域名/IP 白名单。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080 域名IP变更事件</see>
    /// （配套查询接口 <see href="https://developer.work.weixin.qq.com/document/path/100079">path 100079 获取企业微信域名IP信息</see>）。
    /// </remarks>
    public const string ChangeDomainIp = "change_domain_ip";

    // ——— 微信客服族（官方 94670/94699/96426 接收消息与事件、97712/97302/97713 回调通知；Event 信封值） ———

    /// <summary>
    /// 微信客服新消息通知（<c>kf_msg_or_event</c>；外层仅 <c>Token</c> + <c>OpenKfId</c>，
    /// 具体消息/事件内容须调 sync_msg 接口拉取，内容保留最近 3 天）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 三种应用模式报文同构（ADR-14）。接收前置：自建应用配置到「微信客服-可调用接口的应用」并授权客服账号；
    /// 第三方/代开发需「微信客服→管理账号、分配会话和收发消息」权限；客服账号须设置为 API 管理、
    /// 接待人员需在应用可见范围内（2023-12-01 起不再支持系统应用 secret 调用）。
    /// </para>
    /// <para>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/94670">path 94670 接收消息和事件（企业自建）</see>/
    /// <see href="https://developer.work.weixin.qq.com/document/path/94699">path 94699（第三方）</see>/
    /// <see href="https://developer.work.weixin.qq.com/document/path/96426">path 96426（服务商代开发）</see>。
    /// </para>
    /// </remarks>
    public const string KfMsgOrEvent = "kf_msg_or_event";

    /// <summary>
    /// 客服账号授权变更（<c>kf_account_auth_change</c>；应用授权的客服账号发生变化时推送）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>本版未登记强类型载荷（ADR-4 降级）</b>：官方 <c>AuthAddOpenKfId</c>/<c>AuthDelOpenKfId</c>
    /// 为<b>同级重名多节点</b>形态（官方参数表「多个节点表示多个新增账号」），现有载荷声明面
    /// （<c>Items</c> 需「容器/子项」两层嵌套，字段解析取首个同名节点）无法无损表达。
    /// 宿主以 <see cref="GenericCallbackPayload"/> 接收（注意 <c>Values</c> 对同名重复子节点取
    /// <b>最后一个</b>）；全量列表须解析 <see cref="WechatCallbackEvent.DecryptedXml"/> 原文获取。
    /// 待上游映射面支持重名兄弟聚合后补登记载荷。
    /// </para>
    /// <para>
    /// 「取消客服账号的授权」不需要微信客服权限（如权限被移除导致的取消授权仍会推送）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97712">path 97712 回调通知（企业自建）</see>/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97302">path 97302（第三方）</see>/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97713">path 97713（服务商代开发）</see>。
    /// </para>
    /// </remarks>
    public const string KfAccountAuthChange = "kf_account_auth_change";

    // ——— 客户联系变更族（官方 92130 自建 / 92277 第三方 / 96361 代开发；族事件值为键） ———

    /// <summary>
    /// 客户联系变更族的<b>族事件值</b>（企业客户变更：添加 / 编辑 / 免验证添加 / 删除 / 删除跟进成员 / 接替失败，
    /// 具体类别看信封 <c>ChangeType</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>族事件值即事件键</b>：官方 <c>change_external_chat</c> / <c>change_external_tag</c> 的
    /// <c>ChangeType</c> 为裸 <c>create</c>/<c>update</c>/<c>delete</c>，且 <c>del_follow_user</c> 与获客助手族同名
    /// —— 逐 <c>ChangeType</c> 键无法消歧，故本族（及获客助手族）以族事件值为事件键，
    /// <c>ChangeType</c> 经信封判别。第三方应用经<b>指令回调 URL</b>（套件信封）接收同类事件，
    /// 外层事件值在 <c>InfoType</c> 节点，与 <c>Event</c> 信封产出同一事件键。
    /// </para>
    /// <para>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/96361">path 96361</see>（服务商代开发）。
    /// </para>
    /// </remarks>
    public const string ChangeExternalContact = "change_external_contact";

    /// <summary>客户群变更族的<b>族事件值</b>（创建 / 变更 / 解散，具体类别看信封 <c>ChangeType</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>
    /// （第三方 <see href="https://developer.work.weixin.qq.com/document/path/92277">92277</see> / 代开发
    /// <see href="https://developer.work.weixin.qq.com/document/path/96361">96361</see> 报文同构）。
    /// </remarks>
    public const string ChangeExternalChat = "change_external_chat";

    /// <summary>企业客户标签变更族的<b>族事件值</b>（创建 / 变更 / 删除 / 重排，具体类别看信封 <c>ChangeType</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>
    /// （第三方 <see href="https://developer.work.weixin.qq.com/document/path/92277">92277</see> / 代开发
    /// <see href="https://developer.work.weixin.qq.com/document/path/96361">96361</see> 报文同构）。
    /// </remarks>
    public const string ChangeExternalTag = "change_external_tag";

    // ——— 获客助手族（官方 97299 自建 / 97402·99485 第三方 / 98958 代开发；族事件值为键） ———

    /// <summary>
    /// 获客助手事件通知的<b>族事件值</b>（额度/链接/好友请求/收消息/删除成员等，具体类别看信封 <c>ChangeType</c>；
    /// 含第三方组件形态 <c>service_balance_low</c> / <c>service_balance_exhausted</c> / <c>service_balance_consumed</c> /
    /// <c>change_price</c>，官方 99485）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 与客户联系变更族同理，本族以族事件值为事件键（<c>del_follow_user</c> 与客户联系变更族同名，
    /// 逐 <c>ChangeType</c> 键无法消歧）；<c>ChangeType</c> 经信封判别。
    /// </para>
    /// <para>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97299">path 97299 获客助手事件通知</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97402">path 97402</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/99485">path 99485</see>（第三方·组件）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98958">path 98958</see>（服务商代开发）。
    /// </para>
    /// </remarks>
    public const string CustomerAcquisition = "customer_acquisition";

    /// <summary>客户可建联成员范围变动事件（无 <c>ChangeType</c> 分组段；官方 92277）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277 事件格式</see>（第三方）。
    /// </remarks>
    public const string CustomerAcquisitionPermitChange = "customer_acquisition_permit_change";

    // ——— 关注 / 进入应用（官方 90240；Event 信封值） ———

    /// <summary>成员关注事件（进入应用可见范围、加入企业、或被禁用后重新激活等时机触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 接收消息事件</see>
    /// （第三方 <see href="https://developer.work.weixin.qq.com/document/path/90376">90376</see> / 代开发
    /// <see href="https://developer.work.weixin.qq.com/document/path/96468">96468</see> 正文一致）。
    /// </remarks>
    public const string Subscribe = "subscribe";

    /// <summary>成员取消关注事件（退出应用可见范围、退出企业或被禁用等时机触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 接收消息事件</see>。
    /// </remarks>
    public const string Unsubscribe = "unsubscribe";

    /// <summary>成员进入应用事件（EventKey 官方标注恒为空）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 进入应用</see>。
    /// </remarks>
    public const string EnterAgent = "enter_agent";

    // ——— 上报地理位置（官方 90240；官方键值为大写 LOCATION） ———

    /// <summary>成员上报地理位置事件（官方键值为大写 <c>LOCATION</c>，进入应用会话时触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 上报地理位置</see>。
    /// </remarks>
    public const string Location = "LOCATION";

    // ——— 菜单事件（官方 90240；Event 信封值 = 自定义菜单 KEY / 行为类型） ———

    /// <summary>点击菜单拉取消息事件（EventKey = 自定义菜单 KEY 值）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string Click = "click";

    /// <summary>点击菜单跳转链接事件（EventKey = 设置的跳转 URL）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string View = "view";

    /// <summary>点击菜单跳转小程序事件（EventKey = 设置的小程序路径）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string ViewMiniProgram = "view_miniprogram";

    /// <summary>扫码推事件（携带 <c>ScanCodeInfo</c> 扫描信息）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string ScanCodePush = "scancode_push";

    /// <summary>扫码推事件且弹出「消息接收中」提示框（与 <c>scancode_push</c> 报文结构同一）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string ScanCodeWaitMsg = "scancode_waitmsg";

    /// <summary>弹出系统拍照发图事件（携带 <c>SendPicsInfo</c> 图片信息）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string PicSysPhoto = "pic_sysphoto";

    /// <summary>弹出拍照或者相册发图事件（与 <c>pic_sysphoto</c> 报文结构同一）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string PicPhotoOrAlbum = "pic_photo_or_album";

    /// <summary>弹出微信相册发图器事件（与 <c>pic_sysphoto</c> 报文结构同一）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string PicWeixin = "pic_weixin";

    /// <summary>弹出地理位置选择器事件（携带 <c>SendLocationInfo</c> 位置信息）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 菜单事件</see>。</remarks>
    public const string LocationSelect = "location_select";

    // ——— 审批状态通知（官方 90240；Event = open_approval_change） ———

    /// <summary>审批状态通知事件（审批状态变化或审批人操作时触发，载荷包在 <c>ApprovalInfo</c> 内）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 审批状态通知事件</see>。
    /// 官方触发时机描述为「自建/第三方应用调用审批流程引擎」⇒ 开放面不含代开发。
    /// </remarks>
    public const string OpenApprovalChange = "open_approval_change";

    // ——— 共享应用（官方 90240；仅自建应用可被共享） ———

    /// <summary>企业互联共享应用事件（上级企业共享/移除自建应用给下级企业）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 企业互联共享应用事件回调</see>。
    /// 官方触发时机为「把自建应用共享给下级企业」⇒ 开放面仅自建。
    /// </remarks>
    public const string ShareAgentChange = "share_agent_change";

    /// <summary>上下游共享应用事件（上游企业共享/移除自建应用给下游企业）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 上下游共享应用事件回调</see>。
    /// 官方触发时机为「把自建应用共享给下游企业」⇒ 开放面仅自建。
    /// </remarks>
    public const string ShareChainChange = "share_chain_change";

    // ——— 模板卡片（官方 90240；Event 信封值） ———

    /// <summary>模板卡片按钮点击事件（携带 <c>SelectedItems</c> 投票/多选结果）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 模板卡片事件推送</see>。</remarks>
    public const string TemplateCardEvent = "template_card_event";

    /// <summary>通用模板卡片右上角菜单事件（与 <c>template_card_event</c> 报文同一结构，仅少 <c>SelectedItems</c> 节点）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 通用模板卡片右上角菜单事件推送</see>。</remarks>
    public const string TemplateCardMenuEvent = "template_card_menu_event";

    // ——— 应用状态与活跃度（官方 90240；Event 信封值） ———

    /// <summary>长期未使用应用停用预警事件（携带 <c>EffectTime</c> 生效时间戳）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 长期未使用应用停用预警事件</see>。</remarks>
    public const string InactiveAlert = "inactive_alert";

    /// <summary>长期未使用应用被系统自动停用事件。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 长期未使用应用临时停用事件</see>。</remarks>
    public const string CloseInactiveAgent = "close_inactive_agent";

    /// <summary>长期未使用应用被管理员重新启用事件。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 长期未使用应用重新启用事件</see>。</remarks>
    public const string ReopenInactiveAgent = "reopen_inactive_agent";

    /// <summary>应用低活跃预警事件（即将限制客户数据访问；携带 <c>EffectTime</c> 生效时间戳）。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 应用低活跃预警事件</see>。</remarks>
    public const string LowActiveAlert = "low_active_alert";

    /// <summary>应用变为低活跃应用事件。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 低活跃应用事件</see>。</remarks>
    public const string LowActive = "low_active";

    /// <summary>低活跃应用重新恢复活跃状态事件。</summary>
    /// <remarks>官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 低活跃应用活跃恢复事件</see>。</remarks>
    public const string ActiveRestored = "active_restored";

    // ——— 应用版本付费订单回调族（官方 91929~91933 / 99353；InfoType 套件信封，指令回调 URL） ———

    /// <summary>
    /// 下单成功通知（官方键值 <c>open_order</c>）。
    /// <para>当企业在应用市场购买付费应用完成下单后，或服务商在管理端为企业代下单后推送；
    /// 携带 <c>OrderId</c>（订单号）与 <c>OperatorId</c>（下单操作者 userid，服务商或代理商代下单时为空）。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91929">path 91929 下单成功通知</see>。
    /// </remarks>
    public const string OpenOrder = "open_order";

    /// <summary>
    /// 改单通知（官方键值 <c>change_order</c>）。
    /// <para>当服务商管理员修改订单价格之后推送；官方明文「修改订单价格后，会产生新的订单号，
    /// 服务商在改单之后要用新的订单号来查询订单详情，以及关联授权应用」——
    /// 故本事件携带 <c>OldOrderId</c> 与 <c>NewOrderId</c>，<b>无 <c>OrderId</c> 节点</b>。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91930">path 91930 改单通知</see>。
    /// </remarks>
    public const string ChangeOrder = "change_order";

    /// <summary>
    /// 应用版本付费「支付成功通知」（官方键值 <c>pay_for_app_success</c>）。
    /// <para>官方文档键值带 <c>for_app</c> 前缀，勿与「接口调用许可」订单族的支付成功通知混淆。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91931">path 91931 支付成功通知</see>。
    /// </remarks>
    public const string PayForAppSuccess = "pay_for_app_success";

    /// <summary>
    /// 退款通知（官方键值 <c>refund</c>）。
    /// <para>官方裸值 <c>refund</c> 跨族同名（「接口调用许可」订单族的退款结果通知复用同键），
    /// 本族与该族同属套件信封 + InfoType 键域，处理器按 <c>evt.AppType</c> 与套件通道消歧。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91932">path 91932 退款通知</see>。
    /// </remarks>
    public const string Refund = "refund";

    /// <summary>
    /// 应用版本变更通知（官方键值 <c>change_editon</c>）。
    /// <para><b>官方拼写陷阱</b>：官方为 <c>change_editon</c>（少一个字母 i，非 <c>change_edition</c>），本 SDK 照抄原文。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91933">path 91933 应用版本变更通知</see>。
    /// </remarks>
    public const string ChangeEditon = "change_editon";

    /// <summary>
    /// 取消订单通知（官方键值 <c>cancel_order</c>）。
    /// <para>服务商或客户企业取消订单时触发；官方参数表未标注「固定为」，仅列出该取值。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99353">path 99353 取消订单通知</see>。
    /// </remarks>
    public const string CancelOrder = "cancel_order";

    // ——— 邮箱族（官方 97495 自建 / 97517 第三方 / 97506 代开发 + 公共邮箱 100180；族事件值为键） ———

    /// <summary>
    /// 应用邮箱接收邮件事件的<b>族事件值</b>（应用邮箱收到邮件后触发；<c>ChangeType</c> 为 <c>receive_email</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>族事件值即事件键</b>：官方 <c>receive_email</c> 与公共邮箱族（<see cref="PublicEmailChange"/>）同名，
    /// 逐 <c>ChangeType</c> 键无法消歧，故本族以族事件值为事件键、<c>ChangeType</c> 经信封判别
    /// （与客户联系/获客族同理）。<c>Amount</c> 表示应用邮箱当前的新邮件数。
    /// </para>
    /// <para>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97495">path 97495 邮件 回调通知</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97517">path 97517</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97506">path 97506</see>（服务商代开发）
    /// —— 三份正文逐字一致（ADR-14）。
    /// </para>
    /// </remarks>
    public const string AppEmailChange = "app_email_change";

    /// <summary>
    /// 公共邮箱接收邮件事件的<b>族事件值</b>（公共邮箱收到邮件后触发；<c>ChangeType</c> 为 <c>receive_email</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 与 <see cref="AppEmailChange"/> 同理以族事件值为事件键（<c>receive_email</c> 跨族同名）；
    /// 携带 <c>Id</c>（公共邮箱 id）与 <c>Amount</c>（新邮件数）。
    /// <b>仅企业自建应用</b>可接收（官方第三方/代开发无对应回调事件）。
    /// </para>
    /// <para>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100180">path 100180 管理公共邮箱 回调通知</see>（企业自建）。
    /// </para>
    /// </remarks>
    public const string PublicEmailChange = "public_email_change";

    // ——— 文档族（官方 97833/97834/97835/98095/98096 自建 · 97839~97841/98055/98056 第三方 · 97836~97838/98097/98098 代开发） ———

    /// <summary>文档变更事件的 <c>Event</c> 信封值（具体变更类别看 <c>ChangeType</c>；仅 API 创建的文档/表格/智能表格触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97833">path 97833 修改文档成员事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97839">path 97839</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97836">path 97836</see>（服务商代开发）。
    /// </remarks>
    public const string DocChange = "doc_change";

    /// <summary>修改文档成员（API 创建的文档、表格、智能表格有成员添加了其他成员；携带 <c>DocId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97833">path 97833 修改文档成员事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97839">path 97839</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97836">path 97836</see>（服务商代开发）。
    /// </remarks>
    public const string DocMemberChange = "doc_member_change";

    /// <summary>删除文档（文档管理员删除 API 创建的文档、表格；携带 <c>DocId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97834">path 97834 删除文档事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97840">path 97840</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97837">path 97837</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteDoc = "delete_doc";

    /// <summary>收集表完成（成员完成 API 创建的收集表；携带 <c>FormId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97835">path 97835 收集表完成事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97841">path 97841</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97838">path 97838</see>（服务商代开发）。
    /// </remarks>
    public const string FormComplete = "form_complete";

    /// <summary>删除收集表（文档管理员删除 API 创建的收集表；携带 <c>FormId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98095">path 98095 删除收集表事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98055">path 98055</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98097">path 98097</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteForm = "delete_form";

    /// <summary>修改收集表设置（管理员权限/收集范围等收集表设置变更；携带 <c>FormId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98096">path 98096 修改收集表设置事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98056">path 98056</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98098">path 98098</see>（服务商代开发）。
    /// </remarks>
    public const string FormSettingsChange = "form_settings_change";

    // ——— 智能表格族（官方 100986/100987 自建 · 101016/101017 第三方 · 101018/101019 代开发） ———

    /// <summary>智能表格变更事件的 <c>Event</c> 信封值（具体变更类别看 <c>ChangeType</c>；仅 API 创建的智能表格触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100987">path 100987 字段变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018</see>（服务商代开发）。
    /// </remarks>
    public const string SmartSheetChange = "smart_sheet_change";

    /// <summary>新增智能表格字段（携带 <c>FieldId</c> 列表）。
    /// <para><b>官方拼写陷阱</b>：官方键值为 <c>add_filed</c>（filed，非 field），本 SDK 照抄原文。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100987">path 100987 字段变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018</see>（服务商代开发）。
    /// </remarks>
    public const string AddFiled = "add_filed";

    /// <summary>更新智能表格字段（携带 <c>FieldId</c> 列表；官方键值 <c>update_filed</c>，拼写陷阱同 <see cref="AddFiled"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100987">path 100987 字段变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018</see>（服务商代开发）。
    /// </remarks>
    public const string UpdateFiled = "update_filed";

    /// <summary>删除智能表格字段（携带 <c>FieldId</c> 列表；官方键值 <c>delete_filed</c>，拼写陷阱同 <see cref="AddFiled"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100987">path 100987 字段变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101016">path 101016</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101018">path 101018</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteFiled = "delete_filed";

    /// <summary>新增智能表格记录（携带 <c>RecordId</c> 列表，一次最多回调 1000 个、超过分批回调）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100986">path 100986 记录变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101017">path 101017</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101019">path 101019</see>（服务商代开发）。
    /// </remarks>
    public const string AddRecord = "add_record";

    /// <summary>更新智能表格记录（携带 <c>RecordId</c> 列表，一次最多回调 1000 个、超过分批回调）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100986">path 100986 记录变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101017">path 101017</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101019">path 101019</see>（服务商代开发）。
    /// </remarks>
    public const string UpdateRecord = "update_record";

    /// <summary>删除智能表格记录（携带 <c>RecordId</c> 列表，一次最多回调 1000 个、超过分批回调）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100986">path 100986 记录变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101017">path 101017</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/101019">path 101019</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteRecord = "delete_record";

    // ——— 日程族（官方 97728/97730/97731/97732/98111 自建 · 97806~97810/98099 第三方 · 97771~97774/98110 代开发） ———

    /// <summary>删除日历（日历管理员删除 API 创建的日历；携带 <c>CalId</c>；无 <c>ChangeType</c> 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97728">path 97728 删除日历事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97806">path 97806</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97771">path 97771</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteCalendar = "delete_calendar";

    /// <summary>修改日历（日历管理员修改 API 创建的日历；携带 <c>CalId</c>；无 <c>ChangeType</c> 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97730">path 97730 修改日历事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97808">path 97808</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97772">path 97772</see>（服务商代开发）。
    /// </remarks>
    public const string ModifyCalendar = "modify_calendar";

    /// <summary>修改日程（日程管理员修改 API 创建的日程；携带 <c>CalId</c> + <c>ScheduleId</c>；无 <c>ChangeType</c> 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97731">path 97731 修改日程事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97809">path 97809</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97773">path 97773</see>（服务商代开发）。
    /// </remarks>
    public const string ModifySchedule = "modify_schedule";

    /// <summary>删除日程（日程管理员在 API 创建的日历上删除日程；携带 <c>CalId</c> + <c>ScheduleId</c>；无 <c>ChangeType</c> 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97732">path 97732 删除日程事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97810">path 97810</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97774">path 97774</see>（服务商代开发）。
    /// </remarks>
    public const string DeleteSchedule = "delete_schedule";

    /// <summary>日程回执（参与人对 API 创建的日程回执：接受、待定、拒绝；携带 <c>CalId</c> + <c>ScheduleId</c>；
    /// 信封 <c>FromUserName</c> 为进行回执操作的成员；无 <c>ChangeType</c> 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98111">path 98111 日程回执事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98099">path 98099</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/98110">path 98110</see>（服务商代开发）。
    /// </remarks>
    public const string RespondSchedule = "respond_schedule";

    // ——— 会议族（官方 99081~99648 自建 · 97451 第三方 · 97459 代开发；Event = meeting_change / meeting_statistics） ———

    /// <summary>会议变更事件的 <c>Event</c> 信封值（具体变更类别看 <c>ChangeType</c>；仅 API 创建的会议触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99081">path 99081 修改会议事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97451">path 97451</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97459">path 97459</see>（服务商代开发）。
    /// </remarks>
    public const string MeetingChange = "meeting_change";

    /// <summary>修改会议（管理员对 API 创建的会议进行修改）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99081">path 99081 修改会议事件</see>（企业自建）。
    /// </remarks>
    public const string ModifyMeeting = "modify_meeting";

    /// <summary>取消会议（管理员取消 API 创建的会议）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99082">path 99082 取消会议事件</see>（企业自建）。
    /// </remarks>
    public const string CancelMeeting = "cancel_meeting";

    /// <summary>会议开始（API 创建的会议开始）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98333">path 98333 会议开始事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingStart = "meeting_start";

    /// <summary>会议结束（API 创建的会议结束）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98337">path 98337 会议结束事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingEnd = "meeting_end";

    /// <summary>会议全体静音（API 创建的会议开启全体静音）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98341">path 98341 会议全体静音事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingMuteAll = "meeting_mute_all";

    /// <summary>会议解除全体静音（可多次触发，与全体静音并非成对出现）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98345">path 98345 会议解除全体静音事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingUnmuteAll = "meeting_unmute_all";

    /// <summary>成员入会（每个与会者加入 API 创建的会议时各触发一次；与会者含普通与会者与主持人）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98348">path 98348 成员入会事件</see>（企业自建）。
    /// </remarks>
    public const string JoinMeeting = "join_meeting";

    /// <summary>成员离会（与会者离开 API 创建的会议）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98352">path 98352 成员离会事件</see>（企业自建）。
    /// </remarks>
    public const string QuitMeeting = "quit_meeting";

    /// <summary>成员等待主持人入会（需预定会议时勾选「允许成员在主持人进会前加入会议选项」）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98353">path 98353 成员等待主持人入会事件</see>（企业自建）。
    /// </remarks>
    public const string JoinMeetingBeforeHost = "join_meeting_before_host";

    /// <summary>成员进入等候室（与会者每次进入等候室均触发）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98354">path 98354 成员进入等候室事件</see>（企业自建）。
    /// </remarks>
    public const string JoinWaitingRoom = "join_waiting_room";

    /// <summary>成员离开等候室（主持人移出或与会者主动离开；携带 <c>OperatedUser</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98355">path 98355 成员离开等候室事件</see>（企业自建）。
    /// </remarks>
    public const string QuitWaitingRoom = "quit_waiting_room";

    /// <summary>成员从等候室进入会议（主持人允许与会者入会；携带 <c>OperatedUser</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98393">path 98393 成员从等候室进入会议事件</see>（企业自建）。
    /// </remarks>
    public const string JoinFromMeetingRoom = "join_from_meeting_room";

    /// <summary>成员从会议中被移入等候室（携带 <c>OperatedUser</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98394">path 98394 成员从会议中被移入等候室事件</see>（企业自建）。
    /// </remarks>
    public const string MoveToWaitingRoom = "move_to_waiting_room";

    /// <summary>共享屏幕开启（屏幕共享开始）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98395">path 98395 共享屏幕开启事件</see>（企业自建）。
    /// </remarks>
    public const string OpenScreenShare = "open_screen_share";

    /// <summary>共享屏幕结束（屏幕共享结束）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98396">path 98396 共享屏幕结束事件</see>（企业自建）。
    /// </remarks>
    public const string CloseScreenShare = "close_screen_share";

    /// <summary>会议成员角色变更（携带 <c>OperatedUser</c>，<c>UserRole</c> 值域 0~8）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98397">path 98397 会议成员角色变更事件</see>（企业自建）。
    /// </remarks>
    public const string RoleChange = "role_change";

    /// <summary>网络研讨会成员角色变更（携带 <c>OperatedUser</c>，<c>UserRole</c> 值域 0~8 + 30~34）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98771">path 98771 网络研讨会成员角色变更事件</see>（企业自建）。
    /// </remarks>
    public const string WebinarRoleChange = "webinar_role_change";

    /// <summary>网络研讨会暖场上传结果（携带 <c>WarmUpInfo</c>；系统触发，信封 <c>FromUserName</c> 固定 sys）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98773">path 98773 网络研讨会暖场上传结果</see>（企业自建）。
    /// </remarks>
    public const string WebinarWarmUpUpload = "webinar_warm_up_upload";

    /// <summary>PSTN 外呼状态更新（携带 <c>PstnStatus</c>；
    /// <b>官方拼写陷阱</b>：枚举值 <c>CANCLE_INVITE</c> 为官方原文拼写，勿「修正」）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98774">path 98774 PSTN 外呼状态更新事件</see>（企业自建）。
    /// </remarks>
    public const string PstnStatusUpdate = "pstn_status_update";

    /// <summary>素材上传结果（携带 <c>AllUploadStatus</c> 与 <c>UploadInfo</c> 元素列表
    /// ——根下重复同名兄弟元素、无包装容器；系统触发，信封 <c>FromUserName</c> 固定 sys）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98775">path 98775 素材上传结果</see>（企业自建）。
    /// </remarks>
    public const string MediumUpload = "medium_upload";

    /// <summary>会议统计事件的 <c>Event</c> 信封值（具体类别看 <c>ChangeType</c>；与 <see cref="MeetingChange"/> 是两个独立的 Event 值）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99648">path 99648 会议发起事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingStatistics = "meeting_statistics";

    /// <summary>会议发起（应用可见范围内成员发起快速会议，或作为首位参与者进入预约会议；
    /// 携带 <c>Status</c>：1 发起成功 / 2 发起失败）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99648">path 99648 会议发起事件</see>（企业自建）。
    /// </remarks>
    public const string StartMeeting = "start_meeting";

    /// <summary>开始云录制（会议开启云录制；主持人/联席主持人手动或企业管理员自动录制）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98398">path 98398 开始云录制事件</see>（企业自建）。
    /// </remarks>
    public const string StartRecording = "start_recording";

    /// <summary>暂停云录制（云录制开启后被主持人或联席主持人暂停）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98399">path 98399 暂停云录制事件</see>（企业自建）。
    /// </remarks>
    public const string PauseRecording = "pause_recording";

    /// <summary>恢复云录制（恢复之前暂停的云录制）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98400">path 98400 恢复云录制事件</see>（企业自建）。
    /// </remarks>
    public const string ResumeRecording = "resume_recording";

    /// <summary>停止云录制（被主持人或联席主持人停止，或会议自动结束云录制）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98401">path 98401 停止云录制事件</see>（企业自建）。
    /// </remarks>
    public const string StopRecording = "stop_recording";

    /// <summary>云录制已完成（会议结束且云录制转码完成）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98402">path 98402 云录制已完成事件</see>（企业自建）。
    /// </remarks>
    public const string RecordingComplete = "recording_complete";

    /// <summary>删除云录制（云录制文件被手动删除或经 API 接口删除）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98404">path 98404 删除云录制事件</see>（企业自建）。
    /// </remarks>
    public const string DeleteRecording = "delete_recording";

    /// <summary>用户报名（API 创建的会议或网络研讨会用户报名；携带 <c>EnrollId</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98781">path 98781 用户报名事件</see>（企业自建）。
    /// </remarks>
    public const string Enroll = "enroll";

    /// <summary>用户取消报名（携带 <c>EnrollId</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98782">path 98782 用户取消报名事件</see>（企业自建）。
    /// </remarks>
    public const string CancelEnroll = "cancel_enroll";

    /// <summary>会议室应答（API 创建的会议对会议室发起的呼叫有应答结果；
    /// 携带 <c>MeetingRoomId</c> 与 <c>MraAddress</c> 二选一 + <c>RoomResponseStatus</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/98783">path 98783 会议室应答事件</see>（企业自建）。
    /// </remarks>
    public const string MeetingRoomResponse = "meeting_room_response";

    // ——— 微盘族（官方 97898~97903 自建 · 97972~97978 第三方 · 97932~97937 代开发；三份文档逐字一致） ———

    /// <summary>微盘容量不足（企业微盘容量使用率超过 90% 时触发；无 <c>ChangeType</c> 分组段，信封外无业务字段）。
    /// <para><b>官方业务限制（不得弱化）</b>：非实时回调，每天定时检测触发，单个授权企业每天最多回调一次。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97898">path 97898 微盘容量不足事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97972">path 97972</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97932">path 97932</see>（服务商代开发）。
    /// </remarks>
    public const string WedriveInsufficientCapacity = "wedrive_insufficient_capacity";

    /// <summary>微盘空间变更事件的 <c>Event</c> 信封值（具体变更类别看 <c>ChangeType</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97899">path 97899 空间变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97973">path 97973</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97933">path 97933</see>（服务商代开发）。
    /// </remarks>
    public const string WedriveSpaceChange = "wedrive_space_change";

    /// <summary>解散空间（接口指定的管理员解散应用创建的空间；携带 <c>SpaceId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97901">path 97901 解散空间</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97976">path 97976</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97935">path 97935</see>（服务商代开发）。
    /// </remarks>
    public const string DismissSpace = "dismiss_space";

    /// <summary>修改空间成员（接口指定的管理员修改 API 创建的空间成员；携带 <c>SpaceId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97902">path 97902 修改空间成员</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97977">path 97977</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97936">path 97936</see>（服务商代开发）。
    /// </remarks>
    public const string SpaceMemberChange = "space_member_change";

    /// <summary>修改空间安全设置（接口指定的管理员修改应用创建的空间的安全设置；携带 <c>SpaceId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97903">path 97903 修改空间安全设置</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97978">path 97978</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97937">path 97937</see>（服务商代开发）。
    /// </remarks>
    public const string SpaceSecuritySettingsChange = "space_security_settings_change";

    /// <summary>微盘文件变更事件的 <c>Event</c> 信封值（具体变更类别看 <c>ChangeType</c>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97975">path 97975</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97934">path 97934</see>（服务商代开发）。
    /// </remarks>
    public const string WedriveFileChange = "wedrive_file_change";

    /// <summary>创建文件（携带 <c>FileId</c> 列表，官方明示可能有多个节点）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）。
    /// </remarks>
    public const string CreateFile = "create_file";

    /// <summary>重命名文件（携带 <c>FileId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）。
    /// </remarks>
    public const string RenameFile = "rename_file";

    /// <summary>更新文件内容（携带 <c>FileId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）。
    /// </remarks>
    public const string UpdateFile = "update_file";

    /// <summary>删除文件（携带 <c>FileId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）。
    /// </remarks>
    public const string DeleteFile = "delete_file";

    /// <summary>移动文件（携带 <c>FileId</c> 列表）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97900">path 97900 文件变更事件</see>（企业自建）。
    /// </remarks>
    public const string MoveFile = "move_file";

    // ——— 直播族（官方 94145 自建 / 94308 第三方 / 96842 代开发；三份 XML 逐字一致） ———

    /// <summary>直播状态变更（预约/开始/结束等状态变化；携带 <c>LivingId</c> + <c>Status</c> + <c>AgentID</c>；
    /// 无 <c>ChangeType</c> 分组段。仅 API 创建的直播才会回调）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/94145">path 94145 直播回调事件</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/94308">path 94308</see>（第三方）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/96842">path 96842</see>（服务商代开发）。
    /// </remarks>
    public const string LivingStatusChange = "living_status_change";

    // ——— OA 审批族（官方 91815 自建 / 92633 第三方 / 96508 代开发；自建与代开发全文逐字一致） ———

    /// <summary>审批申请状态变化（指定类型的审批单据流程变化时推送：催办、撤销、同意、驳回、转审、添加备注等；
    /// 无 <c>ChangeType</c> 分组段，业务载荷在 <c>ApprovalInfo</c> 包装节点内）。
    /// <para>与 <see cref="OpenApprovalChange"/>（90240 旧式审批状态通知）是两个独立事件。</para>
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/91815">path 91815 审批申请状态变化回调通知</see>（企业自建）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/92633">path 92633</see>（第三方，指令回调 URL）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/96508">path 96508</see>（服务商代开发）。
    /// </remarks>
    public const string SysApprovalChange = "sys_approval_change";
}
