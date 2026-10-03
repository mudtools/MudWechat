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
/// 客户联系/获客助手族以<b>族事件值</b>为事件键（<c>Event</c> 信封取 <c>Event</c> 节点、
/// 第三方套件信封取 <c>InfoType</c> 节点，具体类别由 <c>ChangeType</c> 判别）。
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
/// 消息与事件（关注/菜单/地理位置/审批/共享/模板卡片/应用状态）<see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240</see>（企业内部开发）/
/// <see href="https://developer.work.weixin.qq.com/document/path/90376">path 90376</see>（第三方）/ <see href="https://developer.work.weixin.qq.com/document/path/96468">path 96468</see>（服务商代开发）——
/// 三份文档正文逐字一致，故同一事件键在三种应用模式下共用一个常量。
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
}
