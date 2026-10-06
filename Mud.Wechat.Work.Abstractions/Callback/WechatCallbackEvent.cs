// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback;

/// <summary>
/// 企业微信回调事件信封（解密后的结构化结果；v1 方案 §5.4.1）。
/// </summary>
/// <remarks>
/// <para>
/// 统一承载各官方事件族的回调报文：授权族（<c>InfoType</c>）、通讯录/上下游/客户联系变更族
/// （<c>Event</c> 或套件信封 <c>InfoType</c> + <c>ChangeType</c>）、异步任务族与消息与事件族（<c>Event</c>）。
/// 信封仅定义<b>字段契约</b>（全部 <see cref="string"/>/<see cref="bool"/>），
/// 不引入 XML 类型——「XML → 信封」的解析由 <c>Mud.Wechat.Work.Callback.WechatCallbackReceiver</c> 完成（CB7 守卫）。
/// </para>
/// <para>
/// 本类型从 <c>Mud.Wechat.Work.Callback</c> 上移至 <c>Abstractions</c>（v1 方案 D2）：使宿主业务层
/// 「定义处理器」与回调包「装配分发」解耦。项目未发布，上移一步到位、无旧命名空间垫片。
/// </para>
/// </remarks>
public class WechatCallbackEvent
{
    // ——— 通用信封（解密后明文 XML 顶层） ———

    /// <summary>接收方企业号/套件号（ToUserName 节点：企业 CorpId 或 SuiteId）。</summary>
    public string? ToUserName { get; set; }

    /// <summary>发送方（FromUserName 节点：通讯录变更固定为 sys，异步任务为发起成员 UserID）。</summary>
    public string? FromUserName { get; set; }

    /// <summary>消息创建时间（CreateTime 节点，Unix 秒）。</summary>
    public string? CreateTime { get; set; }

    /// <summary>消息类型（MsgType 节点；事件回调恒为 event）。</summary>
    public string? MsgType { get; set; }

    /// <summary>事件类型（Event 节点：change_contact / batch_job_result）。</summary>
    public string? Event { get; set; }

    /// <summary>变更类型（ChangeType 节点，仅通讯录变更族携带；取值见 <see cref="WechatCallbackEventTypes"/>）。</summary>
    public string? ChangeType { get; set; }

    /// <summary>应用 AgentId（AgentID 节点；通讯录同步助手回调不带）。</summary>
    public string? AgentID { get; set; }

    /// <summary>上下游空间 id（ChainId 节点；仅上下游变更族 change_chain 携带，官方 95796）。</summary>
    public string? ChainId { get; set; }

    // ——— 授权族（既有字段，语义不变） ———

    /// <summary>事件类型（解密后 XML 的 InfoType 节点：suite_ticket / change_auth / cancel_auth / create_auth 等）。</summary>
    public string? InfoType { get; set; }

    /// <summary>服务商 SuiteId（SuiteId 节点）。</summary>
    public string? SuiteId { get; set; }

    /// <summary>最新推送的 suite_ticket（InfoType = suite_ticket 时非空）。</summary>
    /// <remarks><b>敏感凭据</b>：不得写入日志、遥测或异常消息（驱动 <c>get_suite_token</c> 换取套件令牌）。</remarks>
    public string? SuiteTicket { get; set; }

    /// <summary>授权方（企业）CorpId（AuthCorpId 节点；部分事件模板中为 FromUserName）。</summary>
    /// <remarks>
    /// R11：<c>create_auth</c> / <c>reset_permanent_code</c> 报文本体不含该节点，
    /// 故此时恒为 <c>null</c>（解析侧禁止用 <c>FromUserName</c> 兜底伪造）。
    /// <b>D10</b>：FromUserName 兜底仅限「InfoType 非空且非 authcode 族」——通讯录变更事件的
    /// <c>FromUserName</c> 固定为 <c>sys</c>，无差别兜底会伪造授权企业。
    /// </remarks>
    public string? AuthCorpId { get; set; }

    /// <summary>临时授权码（InfoType = create_auth 时非空，用于 get_permanent_code 换取永久授权码）。</summary>
    /// <remarks><b>敏感凭据</b>：不得写入日志、遥测或异常消息（10 分钟有效且一次性，P0-2 补偿面依赖）。</remarks>
    public string? AuthCode { get; set; }

    // ——— 原始载体 ———

    /// <summary>解密后的原始 XML 明文（供业务侧经 <c>WechatCallbackEventParser</c> 解析扩展字段）。</summary>
    /// <remarks>
    /// <b>敏感凭据</b>：可能包含 <see cref="SuiteTicket"/> / <see cref="AuthCode"/> 等凭据节点，
    /// 不得整体写入日志、遥测或异常消息（与 <c>encoding_aeskey</c> 同款警示）。
    /// </remarks>
    public string? DecryptedXml { get; set; }

    /// <summary>时间戳（URL 查询参数，验签用）。</summary>
    public string? TimeStamp { get; set; }

    /// <summary>随机数（URL 查询参数，验签用）。</summary>
    public string? Nonce { get; set; }

    // ——— 事件归属（v2.2 ADR-16）———

    /// <summary>
    /// 事件归属应用键（路由路径段 <c>/{前缀}/{AppKey}</c>）。
    /// </summary>
    /// <remarks>
    /// <b>权威来源</b>：宿主可据此经 <c>IWechatAppManager.ConfiguredConfigs</c> 做<b>非物化</b>反查。
    /// 未命中回调凭据时为 <c>null</c>（不抛）。
    /// </remarks>
    public string? AppKey { get; internal set; }

    /// <summary>
    /// 当前回调条目的应用类型（<b>便捷字段</b>，由接收器按 <see cref="AppKey"/> 解析）。
    /// </summary>
    /// <remarks>
    /// 消除「处理器按模式分支时只能复制 3 个 handler 类」的重复工作量：
    /// 一个处理器即可读 <c>evt.AppType</c> 分支。<b>配置权威仍是</b>
    /// <c>IWechatAppManager</c> / <c>WechatCallbackOptions</c>，本属性只是事件路由事实的<b>只读快照</b>。
    /// </remarks>
    public WechatAppType? AppType { get; internal set; }

    /// <summary>当前回调条目的回调通道（便捷字段，同 <see cref="AppType"/>）。</summary>
    public WechatCallbackChannel? Channel { get; internal set; }

    /// <summary>
    /// 事件类型键（处理器匹配键，v1 方案 D4）：
    /// 客户联系/获客族与邮箱族以<b>族事件值</b>为键（<c>Event</c> 优先、套件信封回退 <c>InfoType</c>）；
    /// 其余按 <c>InfoType</c> → <c>ChangeType</c> → <c>Event</c> 优先级；三者皆空返回空串（仅兜底处理器可见）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>客户联系/获客族为何以族事件值为键</b>：官方 <c>change_external_chat</c> / <c>change_external_tag</c> 的
    /// <c>ChangeType</c> 为裸 <c>create</c>/<c>update</c>/<c>delete</c>（跨族同名），且 <c>del_follow_user</c> 同时是
    /// <c>change_external_contact</c> 与 <c>customer_acquisition</c> 的 <c>ChangeType</c> —— 逐 <c>ChangeType</c> 键
    /// 无法消歧，故这两族以族事件值为事件键、<c>ChangeType</c> 经信封判别（ADR-1 结构族模型）。
    /// 第三方应用的指令回调（套件信封）无 <c>Event</c> 节点，外层事件值在 <c>InfoType</c>，
    /// 与 <c>Event</c> 信封产出<b>同一事件键</b>（三模式键统一，ADR-14）。
    /// </para>
    /// <para>
    /// <b>邮箱族同理</b>：应用邮箱（<c>app_email_change</c>，官方 97495/97517/97506）与公共邮箱
    /// （<c>public_email_change</c>，官方 100180）两族的 <c>ChangeType</c> 同为裸 <c>receive_email</c>，
    /// 逐 <c>ChangeType</c> 键无法消歧 ⇒ 以族事件值为键。
    /// </para>
    /// <para>netstandard2.0 的 <c>string.IsNullOrEmpty</c> 无 <c>[NotNullWhen]</c> 标注，改用显式判空收窄。</para>
    /// </remarks>
    public string EventTypeKey
    {
        get
        {
            var outerEvent = OuterEventValue;
            if (outerEvent != null && IsFamilyKeyEventValue(outerEvent))
            {
                return outerEvent;
            }

            if (InfoType != null && InfoType.Length > 0)
            {
                return InfoType;
            }

            if (ChangeType != null && ChangeType.Length > 0)
            {
                return ChangeType;
            }

            return Event ?? string.Empty;
        }
    }

    /// <summary>
    /// 事件族（<see cref="WechatCallbackEventFamily"/>；客户联系/获客族按外层事件值归类，
    /// 其余授权族按 <c>InfoType</c>、业务族按 <c>Event</c> 归类，驱动「应用类型 × 回调通道」的开放面合法性闸，
    /// 见 <c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>）。
    /// </summary>
    /// <remarks>
    /// <para>判别优先级与 <see cref="EventTypeKey"/> 对齐：客户联系/获客族（外层事件值命中）最优先，
    /// 其次授权族（<c>InfoType</c> 非空），再次按 <c>Event</c> 值归入上下游/通讯录/异步/安全/微信客服五族，
    /// 均未命中返回 <see cref="WechatCallbackEventFamily.Unknown"/>（不拦截）。</para>
    /// </remarks>
    public WechatCallbackEventFamily EventFamily
    {
        get
        {
            var outerEvent = OuterEventValue;
            if (outerEvent != null && IsExternalContactFamilyEventValue(outerEvent))
            {
                return string.Equals(outerEvent, WechatCallbackEventTypes.CustomerAcquisition, StringComparison.Ordinal) ||
                       string.Equals(outerEvent, WechatCallbackEventTypes.CustomerAcquisitionPermitChange, StringComparison.Ordinal)
                    ? WechatCallbackEventFamily.CustomerAcquisition
                    : WechatCallbackEventFamily.ExternalContactChange;
            }

            if (InfoType != null && InfoType.Length > 0)
            {
                return WechatCallbackEventFamily.Authorization;
            }

            if (IsChangeChain)
            {
                return WechatCallbackEventFamily.ChainChange;
            }

            if (IsChangeContact)
            {
                return WechatCallbackEventFamily.ContactChange;
            }

            if (IsBatchJobResult)
            {
                return WechatCallbackEventFamily.BatchJob;
            }

            if (IsSecurityEvent)
            {
                return WechatCallbackEventFamily.SecurityChange;
            }

            if (IsKfEvent)
            {
                return WechatCallbackEventFamily.KfEvent;
            }

            return WechatCallbackEventFamily.Unknown;
        }
    }

    /// <summary>
    /// 外层事件值：<c>Event</c> 节点优先，套件信封（第三方指令回调，无 <c>Event</c> 节点）回退 <c>InfoType</c>。
    /// </summary>
    private string? OuterEventValue
    {
        get
        {
            if (Event != null && Event.Length > 0)
            {
                return Event;
            }

            return InfoType != null && InfoType.Length > 0 ? InfoType : null;
        }
    }

    /// <summary>外层事件值是否属于客户联系/获客族（这两族以族事件值为事件键）。</summary>
    private static bool IsExternalContactFamilyEventValue(string eventValue)
    {
        return string.Equals(eventValue, WechatCallbackEventTypes.ChangeExternalContact, StringComparison.Ordinal) ||
               string.Equals(eventValue, WechatCallbackEventTypes.ChangeExternalChat, StringComparison.Ordinal) ||
               string.Equals(eventValue, WechatCallbackEventTypes.ChangeExternalTag, StringComparison.Ordinal) ||
               string.Equals(eventValue, WechatCallbackEventTypes.CustomerAcquisition, StringComparison.Ordinal) ||
               string.Equals(eventValue, WechatCallbackEventTypes.CustomerAcquisitionPermitChange, StringComparison.Ordinal);
    }

    /// <summary>
    /// 外层事件值是否以「族事件值」为事件键（<see cref="IsExternalContactFamilyEventValue"/> 的
    /// 客户联系/获客族 + 邮箱族：应用邮箱 <c>app_email_change</c> 与公共邮箱 <c>public_email_change</c>
    /// 的 <c>ChangeType</c> 同为裸 <c>receive_email</c>，跨族同名，逐 <c>ChangeType</c> 键无法消歧）。
    /// </summary>
    /// <remarks>
    /// 仅 <see cref="EventTypeKey"/> 使用本判定；<see cref="EventFamily"/> 仍按
    /// <see cref="IsExternalContactFamilyEventValue"/> 归类（邮箱族无独立事件族，
    /// 落 <see cref="WechatCallbackEventFamily.Unknown"/> 由族闸放行、事件键级开放面闸承载判定）。
    /// </remarks>
    private static bool IsFamilyKeyEventValue(string eventValue)
    {
        return IsExternalContactFamilyEventValue(eventValue) ||
               string.Equals(eventValue, WechatCallbackEventTypes.AppEmailChange, StringComparison.Ordinal) ||
               string.Equals(eventValue, WechatCallbackEventTypes.PublicEmailChange, StringComparison.Ordinal);
    }

    /// <summary>是否为授权成功事件（create_auth；携带一次性 auth_code）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487 授权通知事件</see>。
    /// </remarks>
    public bool IsCreateAuth => string.Equals(InfoType, WechatCallbackEventTypes.CreateAuth, StringComparison.Ordinal);

    /// <summary>是否为重置永久授权码事件（reset_permanent_code；代开发 secret 重置，携带 auth_code）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487 授权通知事件</see>。
    /// </remarks>
    public bool IsResetPermanentCode =>
        string.Equals(InfoType, WechatCallbackEventTypes.ResetPermanentCode, StringComparison.Ordinal);

    /// <summary>是否为单纯携带 auth_code 的事件（create_auth / reset_permanent_code）。</summary>
    /// <remarks>
    /// R11：这两类报文体<b>不含 AuthCorpId</b>（<c>reset_permanent_code</c> 仅含
    /// <c>SuiteId</c> / <c>AuthCode</c> / <c>InfoType</c> / <c>TimeStamp</c>），
    /// 授权企业须由 <c>auth_code</c> 换码后经 <c>auth_corp_info.corpid</c> 反查。
    /// </remarks>
    public bool IsAuthCodeEvent => IsCreateAuth || IsResetPermanentCode;

    /// <summary>是否为 suite_ticket 推送事件。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90628">path 90628 推送 suite_ticket</see>。
    /// </remarks>
    public bool IsSuiteTicket => string.Equals(InfoType, WechatCallbackEventTypes.SuiteTicket, StringComparison.Ordinal);

    /// <summary>是否为授权变更事件（change_auth）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100964">path 100964 修改授权通知事件</see>。
    /// </remarks>
    public bool IsChangeAuth => string.Equals(InfoType, WechatCallbackEventTypes.ChangeAuth, StringComparison.Ordinal);

    /// <summary>是否为取消授权事件（cancel_auth / del_auth）。</summary>
    /// <remarks>
    /// 官方文档：服务商应用授权事件回调（<see href="https://developer.work.weixin.qq.com/document/path/99487">path 99487 授权通知事件</see>）。
    /// </remarks>
    public bool IsCancelAuth =>
        string.Equals(InfoType, WechatCallbackEventTypes.CancelAuth, StringComparison.Ordinal) ||
        string.Equals(InfoType, WechatCallbackEventTypes.DelAuth, StringComparison.Ordinal);

    /// <summary>是否为通讯录变更事件（Event = change_contact；具体变更类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90967">path 90967 通讯录回调概述</see>。
    /// </remarks>
    public bool IsChangeContact =>
        string.Equals(Event, WechatCallbackEventTypes.ChangeContact, StringComparison.Ordinal);

    /// <summary>是否为上下游变更事件（Event = change_chain；具体变更类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 仅自建应用可配置接收（配置到「上下游-可调用接口的应用」并开启「上下游变更回调」）；
    /// 由上下游系统应用触发的变更不回调。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>。
    /// </remarks>
    public bool IsChangeChain =>
        string.Equals(Event, WechatCallbackEventTypes.ChangeChain, StringComparison.Ordinal);

    /// <summary>是否为异步任务完成事件（Event = batch_job_result）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90973">path 90973 异步任务完成通知</see>（通讯录）/
    /// <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797 异步任务完成通知</see>（上下游，BatchJob 包装布局）。
    /// </remarks>
    public bool IsBatchJobResult =>
        string.Equals(Event, WechatCallbackEventTypes.BatchJobResult, StringComparison.Ordinal);

    /// <summary>是否为安全管理事件（Event = security；具体变更类别看 <see cref="ChangeType"/>，官方 100080）。</summary>
    /// <remarks>
    /// 仅自建应用可配置接收（配置到「我的企业 - 设置 - 域名IP - 可调用API的应用」）；
    /// 第三方 / 代开发应用暂不支持。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080 域名IP变更事件</see>。
    /// </remarks>
    public bool IsSecurityEvent =>
        string.Equals(Event, WechatCallbackEventTypes.Security, StringComparison.Ordinal);

    /// <summary>
    /// 是否为微信客服族事件（<see cref="WechatCallbackEventTypes.KfMsgOrEvent"/> 新消息通知 /
    /// <see cref="WechatCallbackEventTypes.KfAccountAuthChange"/> 客服账号授权变更）。
    /// </summary>
    /// <remarks>
    /// 三类应用均可接收（自建配置到「微信客服-可调用接口的应用」；第三方/代开发需微信客服权限）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/94670">path 94670 接收消息和事件</see>/
    /// <see href="https://developer.work.weixin.qq.com/document/path/97712">path 97712 回调通知</see>。
    /// </remarks>
    public bool IsKfEvent =>
        string.Equals(Event, WechatCallbackEventTypes.KfMsgOrEvent, StringComparison.Ordinal) ||
        string.Equals(Event, WechatCallbackEventTypes.KfAccountAuthChange, StringComparison.Ordinal);

    /// <summary>是否为客户联系·企业客户变更事件（外层事件值 = change_external_contact；具体类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 套件信封（第三方指令回调）的该值在 <c>InfoType</c> 节点，本属性同样命中。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>
    /// （第三方 <see href="https://developer.work.weixin.qq.com/document/path/92277">92277</see> / 代开发
    /// <see href="https://developer.work.weixin.qq.com/document/path/96361">96361</see>）。
    /// </remarks>
    public bool IsChangeExternalContact =>
        OuterEventValue != null &&
        string.Equals(OuterEventValue, WechatCallbackEventTypes.ChangeExternalContact, StringComparison.Ordinal);

    /// <summary>是否为客户群变更事件（外层事件值 = change_external_chat；具体类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>。
    /// </remarks>
    public bool IsChangeExternalChat =>
        OuterEventValue != null &&
        string.Equals(OuterEventValue, WechatCallbackEventTypes.ChangeExternalChat, StringComparison.Ordinal);

    /// <summary>是否为企业客户标签变更事件（外层事件值 = change_external_tag；具体类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件格式</see>。
    /// </remarks>
    public bool IsChangeExternalTag =>
        OuterEventValue != null &&
        string.Equals(OuterEventValue, WechatCallbackEventTypes.ChangeExternalTag, StringComparison.Ordinal);

    /// <summary>是否为获客助手事件通知（外层事件值 = customer_acquisition；具体类别看 <see cref="ChangeType"/>）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/97299">path 97299 获客助手事件通知</see>
    /// （第三方 <see href="https://developer.work.weixin.qq.com/document/path/97402">97402</see> / 组件
    /// <see href="https://developer.work.weixin.qq.com/document/path/99485">99485</see> / 代开发
    /// <see href="https://developer.work.weixin.qq.com/document/path/98958">98958</see>）。
    /// </remarks>
    public bool IsCustomerAcquisition =>
        OuterEventValue != null &&
        string.Equals(OuterEventValue, WechatCallbackEventTypes.CustomerAcquisition, StringComparison.Ordinal);

    /// <summary>是否为客户可建联成员范围变动事件（无 ChangeType 分组段）。</summary>
    /// <remarks>
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277 事件格式</see>（第三方）。
    /// </remarks>
    public bool IsCustomerAcquisitionPermitChange =>
        OuterEventValue != null &&
        string.Equals(OuterEventValue, WechatCallbackEventTypes.CustomerAcquisitionPermitChange, StringComparison.Ordinal);
}
