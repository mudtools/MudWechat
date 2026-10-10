// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 群发结果事件载荷（<c>MASSSENDJOBFINISH</c>；P0-e 已核验：群发消息指南页「4、事件推送群发结果」节）。
/// </summary>
/// <remarks>
/// <para>
/// 官方引言原文：「由于群发任务提交后，群发任务可能在一定时间后才完成，因此，群发接口调用时，
/// 仅会给出群发任务是否提交成功的提示，若群发任务提交成功，则在群发任务结束时，会向开发者在
/// 公众平台填写的开发者 URL（callback URL）推送事件」「将会在群发任务<b>即将完成</b>的时候，
/// 就推送群发结果，此时的推送人数数据将会与实际情形存在一定误差」。
/// </para>
/// <para>
/// <b>官方文档失真（照录）</b>：①官方示例标注「发送成功时」但 Status 为 <c>err(30003)</c>
/// （原创校验被判转载不群发）——标题与示例自相矛盾；②AuditState 描述「0表示<b>尚未未</b>校验原创」
/// 为官方笔误；③Status 的 err 枚举列表全角/半角混用——解析勿依赖分隔符格式。
/// </para>
/// <para>
/// <b>大小写漂移</b>：官方现网 Event 值为大写 <c>MASSSENDJOBFINISH</c>；本键无小写双登记
/// （群发事件无历史小写报文佐证，模板侧有——见 <see cref="MpTemplateSendJobFinishPayload"/>）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpSendJobFinishEventTypes.MassSendJobFinish })]
public sealed partial class MpMassSendJobFinishPayload : MpCallbackPayload
{
    /// <summary>群发的消息 ID（官方 <c>MsgID</c>；对应群发接口响应的 <c>msg_id</c>）。</summary>
    [PayloadField("MsgID")]
    public long? MsgId { get; set; }

    /// <summary>
    /// 群发结果（官方 <c>Status</c>：「send success」/「send fail」/「err(num)」——
    /// err 为审核失败原因码（如 err(30001~30003) 原创校验、err(40001) 管理员拒绝、err(40002) 管理员 30 分钟超时等）；
    /// <b>send success 时仍可能因用户拒收、系统错误等造成少量用户接收失败</b>（官方原文）。
    /// </summary>
    [PayloadField("Status")]
    public string? Status { get; set; }

    /// <summary>tag_id 下粉丝数；或 touser 中的粉丝数（官方 <c>TotalCount</c>）。</summary>
    [PayloadField("TotalCount")]
    public long? TotalCount { get; set; }

    /// <summary>过滤后准备发送的粉丝数（官方 <c>FilterCount</c>；过滤含地区/性别/拒收/超 4 条——一般 FilterCount ≈ SentCount + ErrorCount）。</summary>
    [PayloadField("FilterCount")]
    public long? FilterCount { get; set; }

    /// <summary>发送成功的粉丝数（官方 <c>SentCount</c>）。</summary>
    [PayloadField("SentCount")]
    public long? SentCount { get; set; }

    /// <summary>发送失败的粉丝数（官方 <c>ErrorCount</c>）。</summary>
    [PayloadField("ErrorCount")]
    public long? ErrorCount { get; set; }

    /// <summary>原创校验结果（官方 <c>CopyrightCheckResult</c>；<b>仅图文群发携带</b>，节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("CopyrightCheckResult", Format = PayloadFieldFormat.Object)]
    public MpMassCopyrightCheckResult? CopyrightCheckResult { get; set; }

    /// <summary>群发文章 URL 结果（官方 <c>ArticleUrlResult</c>；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("ArticleUrlResult", Format = PayloadFieldFormat.Object)]
    public MpMassArticleUrlResult? ArticleUrlResult { get; set; }
}

/// <summary>原创校验结果（官方 <c>CopyrightCheckResult</c> 节点）。</summary>
/// <remarks>
/// 嵌套两层列表：<c>ResultList/item/...</c>（项元素名官方固定 <c>item</c>，经 <c>ItemsObject</c> 通道声明化——
/// 与菜单事件 <c>SendPicsInfo/PicList/item</c> 同形态）。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpMassCopyrightCheckResult
{
    /// <summary>校验条目数（官方 <c>Count</c>）。</summary>
    [PayloadField("Count")]
    public long? Count { get; set; }

    /// <summary>各个单图文校验结果列表（官方 <c>ResultList/item</c>）。</summary>
    [PayloadField("ResultList", Format = PayloadFieldFormat.ItemsObject, ItemName = "item")]
    public List<MpMassCopyrightItem>? ResultList { get; set; }

    /// <summary>整体校验结果（官方 <c>CheckState</c>：1 未被判为转载可群发 / 2 被判为转载可群发 / 3 被判为转载不能群发）。</summary>
    [PayloadField("CheckState")]
    public long? CheckState { get; set; }
}

/// <summary>单图文原创校验条目（官方 <c>CopyrightCheckResult/ResultList/item</c>）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpMassCopyrightItem
{
    /// <summary>群发文章的序号（官方 <c>ArticleIdx</c>，从 1 开始）。</summary>
    [PayloadField("ArticleIdx")]
    public long? ArticleIdx { get; set; }

    /// <summary>用户声明文章的状态（官方 <c>UserDeclareState</c>：0 未声明原创 / 1 声明原创 / 2 声明转载）。</summary>
    [PayloadField("UserDeclareState")]
    public long? UserDeclareState { get; set; }

    /// <summary>系统校验状态（官方 <c>AuditState</c>：0 尚未校验原创（官方原文多一「未」字笔误，照录）/
    /// 1 校验中 / 11 校验通过 / 其他为未通过）。</summary>
    [PayloadField("AuditState")]
    public long? AuditState { get; set; }

    /// <summary>相似原创文的 url（官方 <c>OriginalArticleUrl</c>）。</summary>
    [PayloadField("OriginalArticleUrl")]
    public string? OriginalArticleUrl { get; set; }

    /// <summary>相似原创文的类型（官方 <c>OriginalArticleType</c>：1 原创文 / 2 历史非原创文 / 3 新闻类 / 4 黑文类）。</summary>
    [PayloadField("OriginalArticleType")]
    public long? OriginalArticleType { get; set; }

    /// <summary>是否能转载（官方 <c>CanReprint</c>）。</summary>
    [PayloadField("CanReprint")]
    public long? CanReprint { get; set; }

    /// <summary>是否需要替换成原创文内容（官方 <c>NeedReplaceContent</c>）。</summary>
    [PayloadField("NeedReplaceContent")]
    public long? NeedReplaceContent { get; set; }

    /// <summary>是否需要注明转载来源（官方 <c>NeedShowReprintSource</c>）。</summary>
    [PayloadField("NeedShowReprintSource")]
    public long? NeedShowReprintSource { get; set; }
}

/// <summary>群发文章 URL 结果（官方 <c>ArticleUrlResult</c> 节点）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpMassArticleUrlResult
{
    /// <summary>条目数（官方 <c>Count</c>）。</summary>
    [PayloadField("Count")]
    public long? Count { get; set; }

    /// <summary>文章 URL 列表（官方 <c>ResultList/item</c>）。</summary>
    [PayloadField("ResultList", Format = PayloadFieldFormat.ItemsObject, ItemName = "item")]
    public List<MpMassArticleUrlItem>? ResultList { get; set; }
}

/// <summary>文章 URL 条目（官方 <c>ArticleUrlResult/ResultList/item</c>）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpMassArticleUrlItem
{
    /// <summary>群发文章的序号（官方 <c>ArticleIdx</c>，从 1 开始）。</summary>
    [PayloadField("ArticleIdx")]
    public long? ArticleIdx { get; set; }

    /// <summary>群发文章的 url（官方 <c>ArticleUrl</c>）。</summary>
    [PayloadField("ArticleUrl")]
    public string? ArticleUrl { get; set; }
}

/// <summary>
/// 模板消息发送结果事件载荷（<c>TEMPLATESENDJOBFINISH</c> / 小写 <c>templatesendjobfinish</c> 双键；
/// P0-e 已核验：服务号模板消息指南页「事件推送」节）。
/// </summary>
/// <remarks>
/// <para>
/// 官方引言原文：「在模版消息发送任务完成后，微信服务器会将是否送达成功作为通知，发送到开发者中心中
/// 填写的服务器配置地址中」（「模版」用字为官方原文，照录）。
/// </para>
/// <para>
/// <b>Status 三形态（官方三例 verbatim）</b>：<c>success</c>（送达成功）/<c>failed:user block</c>
/// （用户拒收——用户设置拒绝接收服务号消息）/<c>failed: system failed</c>（其他原因失败）。
/// </para>
/// <para>
/// <b>大小写漂移（官方文档失真，双键登记）</b>：官方现网示例 Event 值为<b>全大写</b>
/// <c>TEMPLATESENDJOBFINISH</c>；历史资料通行小写驼峰——两形态都登记，命中同一载荷。
/// </para>
/// <para>官方示例缺陷（照录）：示例二/三复用相同 CreateTime 与 MsgID，疑为复制粘贴产物。</para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpSendJobFinishEventTypes.TemplateSendJobFinish })]
[MpCallbackContract(EventTypes = new[] { MpSendJobFinishEventTypes.TemplateSendJobFinishLowered })]
public sealed partial class MpTemplateSendJobFinishPayload : MpCallbackPayload
{
    /// <summary>消息 id（官方 <c>MsgID</c>；对应发送模板消息响应的 <c>msgid</c>——注意本节点拼写为大写 ID）。</summary>
    [PayloadField("MsgID")]
    public long? MsgId { get; set; }

    /// <summary>发送状态（官方 <c>Status</c>：success / failed:user block / failed: system failed——见类型 remarks）。</summary>
    [PayloadField("Status")]
    public string? Status { get; set; }
}
