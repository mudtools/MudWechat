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
/// 统一承载三类回调事件：授权族（<c>InfoType</c>）、通讯录变更族（<c>Event = change_contact</c> + <c>ChangeType</c>）、
/// 异步任务族（<c>Event = batch_job_result</c>）。信封仅定义<b>字段契约</b>（全部 <see cref="string"/>/<see cref="bool"/>），
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
    /// 事件类型键（处理器匹配键，v1 方案 D4）：<c>InfoType</c>（非空）优先，其次 <c>ChangeType</c>，再次 <c>Event</c>；
    /// 三者皆空返回空串（仅兜底处理器可见）。
    /// </summary>
    /// <remarks>netstandard2.0 的 <c>string.IsNullOrEmpty</c> 无 <c>[NotNullWhen]</c> 标注，改用显式判空收窄。</remarks>
    public string EventTypeKey
    {
        get
        {
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
    /// 事件族（<see cref="WechatCallbackEventFamily"/>；按 <c>InfoType</c> → <c>Event</c> 归类，
    /// 驱动「应用类型 × 回调通道」的开放面合法性闸，见 <c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>）。
    /// </summary>
    /// <remarks>
    /// <para>判别优先级与 <see cref="EventTypeKey"/> 对齐：授权族（<c>InfoType</c> 非空）最优先，
    /// 其次按 <c>Event</c> 值归入上下游/通讯录/异步三族，均未命中返回 <see cref="WechatCallbackEventFamily.Unknown"/>（不拦截）。</para>
    /// </remarks>
    public WechatCallbackEventFamily EventFamily
    {
        get
        {
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

            return WechatCallbackEventFamily.Unknown;
        }
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
}
