// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Callback;

/// <summary>
/// 微信支付 APIv3 通知信封（回调 POST 报文体；<b>JSON</b>，与企微 / 公众号的 XML 信封完全不同）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：支付通知为通用信封，具体载荷由 <see cref="EventType"/> 决定
/// （交易成功 <c>TRANSACTION.SUCCESS</c>、退款结果 <c>REFUND.SUCCESS</c> 等）。
/// </para>
/// <para>
/// <b>⚠️ <c>event_type</c> 不足以唯一确定载荷</b>：官方把<b>分账动态通知</b>（分账 / 分账回退）
/// <b>也</b>标为 <c>TRANSACTION.SUCCESS</c>，与支付成功通知<b>同值</b>。可靠区分须结合
/// <c>resource.original_type</c>（分账为 <c>profitsharing</c>）—— 见
/// <see cref="WechatPayNotificationResource.OriginalType"/> 与
/// <see cref="WechatPayNotificationOriginalTypes"/>。
/// </para>
/// <para>
/// <b>字段名照官方原文</b>（<c>create_time</c> / <c>event_type</c> / <c>resource_type</c>）。
/// </para>
/// <para>
/// <b>验签在前、反序列化在后</b>：报文原文（原始字节）须先经
/// <c>WechatPaySignatureMessages.BuildVerifyMessage</c> + 平台证书验签，<b>通过后</b>才允许反序列化。
/// 先反序列化再验签会让攻击者用畸形 JSON 触发解析分支（且违反官方 FAQ 的「报文体必须是原始字节」要求）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayNotification
{
    /// <summary>通知 ID（<c>id</c>，string(36)），官方唯一标识，可用于幂等去重。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>通知创建时间（<c>create_time</c>，string(32)，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>通知类型（<c>resource_type</c>，string(32)），目前固定 <c>encrypt-resource</c>。</summary>
    [JsonPropertyName("resource_type")]
    public string? ResourceType { get; set; }

    /// <summary>
    /// 通知事件类型（<c>event_type</c>，string(32)），取值见 <see cref="WechatPayNotificationEventTypes"/>。
    /// </summary>
    /// <remarks>
    /// <b>不得单独用它选载荷</b>：<c>TRANSACTION.SUCCESS</c> 同时用于「支付成功」与「分账动态通知」，
    /// 须结合 <see cref="WechatPayNotificationResource.OriginalType"/> 判定。
    /// </remarks>
    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    /// <summary>通知简要说明（<c>summary</c>，string(64)）。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>通知资源数据（<c>resource</c>，必填），含 <c>AEAD_AES_256_GCM</c> 密文，见 <see cref="WechatPayNotificationResource"/>。</summary>
    [JsonPropertyName("resource")]
    public WechatPayNotificationResource? Resource { get; set; }
}

/// <summary>通知资源数据（<c>resource</c>；密文三元组 + 算法标识）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayNotificationResource
{
    /// <summary>原始类型（<c>original_type</c>，string(32)），如 <c>transaction</c> / <c>refund</c>。</summary>
    [JsonPropertyName("original_type")]
    public string? OriginalType { get; set; }

    /// <summary>加密算法（<c>algorithm</c>，string(32)），固定 <c>AEAD_AES_256_GCM</c>（其它取值一律拒绝）。</summary>
    [JsonPropertyName("algorithm")]
    public string? Algorithm { get; set; }

    /// <summary>密文（<c>ciphertext</c>，string，Base64）。<b>不得入日志 / 遥测 / 异常消息</b>（PAY-B7）。</summary>
    [JsonPropertyName("ciphertext")]
    public string? CipherText { get; set; }

    /// <summary>附加数据（<c>associated_data</c>，string(16)），参与 GCM 认证。</summary>
    [JsonPropertyName("associated_data")]
    public string? AssociatedData { get; set; }

    /// <summary>随机串（<c>nonce</c>，string(12)），即 GCM 的 12 字节 IV 的字符串表达。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}

/// <summary>
/// 通知事件类型（官方 <c>event_type</c> 取值）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 同名复用</b>：<see cref="TransactionSuccess"/> 同时是「支付成功」与「分账动态通知」
/// （分账 / 分账回退）的 <c>event_type</c> —— 官方原文如此。区分载荷必须再看
/// <see cref="WechatPayNotificationOriginalTypes"/>。
/// </para>
/// <para>取值来源：交易/退款通知页与本仓已核验的各回调页（2026-10-09）。</para>
/// </remarks>
public static class WechatPayNotificationEventTypes
{
    /// <summary>交易成功（官方 <c>TRANSACTION.SUCCESS</c>）；<b>分账动态通知也取此值</b>。</summary>
    public const string TransactionSuccess = "TRANSACTION.SUCCESS";

    /// <summary>退款成功（官方 <c>REFUND.SUCCESS</c>）。</summary>
    public const string RefundSuccess = "REFUND.SUCCESS";

    /// <summary>退款异常（官方 <c>REFUND.ABNORMAL</c>）。</summary>
    public const string RefundAbnormal = "REFUND.ABNORMAL";

    /// <summary>退款关闭（官方 <c>REFUND.CLOSED</c>）。</summary>
    public const string RefundClosed = "REFUND.CLOSED";

    /// <summary>
    /// 支付分订单支付成功（官方 <c>PAYSCORE.USER_PAID</c>）。
    /// </summary>
    /// <remarks>
    /// <b>与分账通知的形态不同</b>：分账动态通知<b>复用</b>了交易成功的 <c>TRANSACTION.SUCCESS</c>（需靠
    /// <c>original_type</c> 判别），而支付分通知有<b>自己的前缀</b> <c>PAYSCORE.</c> ——
    /// 即 <c>event_type</c> 在不同产品线的复用情况<b>不一致</b>，消费侧不应假定某种统一规则，
    /// 一律按「先看判别字段、再看具体取值」处理。
    /// </remarks>
    public const string PayScoreUserPaid = "PAYSCORE.USER_PAID";

    /// <summary>
    /// 支付分订单确认成功（官方 <c>PAYSCORE.USER_CONFIRM</c>）。
    /// </summary>
    /// <remarks>
    /// <b>⚠️ 大小写敏感</b>：官方页面逐字为<b>大写</b> <c>PAYSCORE.USER_CONFIRM</c>
    /// （检索摘要里曾呈现为全小写 <c>payscore.user_confirm</c>，属二手来源的失真）。
    /// <c>event_type</c> 是字符串等值匹配 ⇒ 按小写实现会<b>静默不命中</b>回调。
    /// </remarks>
    public const string PayScoreUserConfirm = "PAYSCORE.USER_CONFIRM";

    /// <summary>
    /// 支付分<b>授权成功</b>（官方 <c>PAYSCORE.USER_OPEN_SERVICE</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012086446"/>
    /// （开启/解除授权服务回调通知，2026-10-09 逐字核验；更新时间 2025.04.22）。
    /// 官方原文：「微信支付分回调通知的类型 1、授权成功通知的类型为 <c>PAYSCORE.USER_OPEN_SERVICE</c>」。
    /// </para>
    /// <para>
    /// <b>⚠️ 大小写的坑（再次实测）</b>：检索摘要里该值呈现为<b>全小写</b>
    /// <c>payscore.user_open_service</c>，而<b>官方页面逐字为大写</b> <c>PAYSCORE.USER_OPEN_SERVICE</c>
    /// —— 这是本仓第三次遇到「二手摘要把 <c>event_type</c> 小写化」的失真
    /// （前两次：<c>PAYSCORE.USER_CONFIRM</c>、<c>PAYSCORE.USER_PAID</c>）。
    /// <c>event_type</c> 是大小写敏感的等值匹配 ⇒ 按小写实现会<b>静默不命中</b>。
    /// </para>
    /// </remarks>
    public const string PayScoreUserOpenService = "PAYSCORE.USER_OPEN_SERVICE";

    /// <summary>
    /// 支付分<b>解除授权成功</b>（官方 <c>PAYSCORE.USER_CLOSE_SERVICE</c>）。
    /// </summary>
    /// <remarks>
    /// 官方文档同 <see cref="PayScoreUserOpenService"/>。官方原文：解除授权成功通知的类型为
    /// <c>PAYSCORE.USER_CLOSE_SERVICE</c>（<b>同样是大写</b>）。
    /// </remarks>
    public const string PayScoreUserCloseService = "PAYSCORE.USER_CLOSE_SERVICE";

    /// <summary>
    /// 电子发票：用户完成发票抬头填写（官方 <c>FAPIAO.USER_APPLIED</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012286009"/>
    /// （用户发票抬头填写完成通知，2026-10-09 逐字核验；更新时间 2025.09.26）。
    /// </para>
    /// <para>
    /// <b>反直觉命名（留档）</b>：该通知的官方名是「用户发票<b>抬头填写完成</b>」，
    /// 而 <c>event_type</c> 却是 <c>USER_APPLIED</c>（<b>申请</b>）—— <b>不是</b>
    /// <c>USER_FILLED</c> 之类的直觉名 ⇒ SDK 一律以官方原文为准。
    /// </para>
    /// </remarks>
    public const string FapiaoUserApplied = "FAPIAO.USER_APPLIED";

    /// <summary>
    /// 电子发票：发票开具成功（官方 <c>FAPIAO.ISSUED</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012286057"/>
    /// （发票开具成功通知，2026-10-09 逐字核验；更新时间 2025.09.26）。
    /// </para>
    /// <para>
    /// <b>⚠️ 与「发票卡券已作废」只差一个前缀词</b>：开具成功是 <c>FAPIAO.ISSUED</c>，
    /// 而发票<b>状态</b>里的 <c>ISSUED</c> 是同一拼写但语域不同（一个是事件、一个是状态值）
    /// ⇒ 二者都在本仓出现，<b>不得</b>互相赋值。
    /// </para>
    /// </remarks>
    public const string FapiaoIssued = "FAPIAO.ISSUED";

    /// <summary>
    /// 电子发票：发票插入用户卡包成功（官方 <c>FAPIAO.CARD_INSERTED</c>）。
    /// </summary>
    /// <remarks>
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012286082"/>
    /// （2026-10-09 逐字核验；更新时间 2025.09.26）。与
    /// <see cref="FapiaoReversed"/> / <see cref="FapiaoCardDiscarded"/> <b>共用同一载荷类型</b>
    /// （官方三页字段表逐项一致）—— 三者的差别只体现在 <c>card_status</c> / <c>fapiao_status</c> 的取值上。
    /// </remarks>
    public const string FapiaoCardInserted = "FAPIAO.CARD_INSERTED";

    /// <summary>
    /// 电子发票：发票冲红成功（官方 <c>FAPIAO.REVERSED</c>）。
    /// </summary>
    /// <remarks>
    /// 官方文档：<c>…/docs/merchant/apis/fapiao/fapiao-applications/invoice-flush-success-notice.html</c>
    /// （2026-10-09 逐字核验；更新时间 2025.09.26）。<b>⚠️ 命名不对称</b>：接口叫「冲红」（reverse），
    /// 而事件类型用的是 <c>REVERSED</c>（英文页面 slug 却用 <c>flush</c>）—— 三者不一致，照官方原文取用。
    /// </remarks>
    public const string FapiaoReversed = "FAPIAO.REVERSED";

    /// <summary>
    /// 电子发票：发票卡券作废（官方 <c>FAPIAO.CARD_DISCARDED</c>）。
    /// </summary>
    /// <remarks>
    /// 官方文档：<c>…/docs/merchant/apis/fapiao/fapiao-card-template/invoice-card-cancel-notice.html</c>
    /// （2026-10-09 逐字核验；更新时间 2025.09.26）。页面标题用「作废」、slug 用 <c>cancel</c>、
    /// 事件类型用 <c>DISCARDED</c> —— 三处用词各不相同，只有 <c>DISCARDED</c> 是判别用的真值。
    /// </remarks>
    public const string FapiaoCardDiscarded = "FAPIAO.CARD_DISCARDED";
}

/// <summary>
/// 电子发票通知的两组状态枚举（官方原文，<b>两套不同维度的状态</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：电子发票三类通知页（<c>4012286082</c> 插卡成功 / 冲红成功 /
/// <c>invoice-card-cancel-notice</c> 卡券作废）的明文字段说明，2026-10-09 逐字核验；
/// 三页给出的取值<b>完全一致</b>。
/// </para>
/// <para>
/// <b>⚠️ 两组状态必须分开判断</b>：<c>fapiao_status</c> 描述<b>开票/冲红</b>这条线，
/// <c>card_status</c> 描述<b>卡券</b>这条线；同一张发票可以「已开具但还没插卡」，
/// 也可以「已作废但仍处于已开具」—— 用一组去判断另一组的状态会得出错误结论。
/// </para>
/// </remarks>
public static class FapiaoStatuses
{
    /// <summary>开票请求已受理（官方 <c>ISSUE_ACCEPTED</c>）：非终态。</summary>
    public const string IssueAccepted = "ISSUE_ACCEPTED";

    /// <summary>发票已开具（官方 <c>ISSUED</c>）：<b>只有</b>此状态才能取到下载链接。</summary>
    public const string Issued = "ISSUED";

    /// <summary>冲红申请已受理（官方 <c>REVERSE_ACCEPTED</c>）：非终态。</summary>
    public const string ReverseAccepted = "REVERSE_ACCEPTED";

    /// <summary>发票已冲红（官方 <c>REVERSED</c>）：终态。</summary>
    public const string Reversed = "REVERSED";
}

/// <summary>
/// 电子发票卡券状态枚举（官方原文 <c>card_status</c>）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="FapiaoStatuses"/> 是两套</b>（维度不同，勿混用）；
/// 其中 <see cref="Inserted"/> 才表示发票真正进了用户卡包。
/// </remarks>
public static class FapiaoCardStatuses
{
    /// <summary>插卡申请已受理（官方 <c>INSERT_ACCEPTED</c>）：非终态。</summary>
    public const string InsertAccepted = "INSERT_ACCEPTED";

    /// <summary>已插入用户卡包（官方 <c>INSERTED</c>）：终态。</summary>
    public const string Inserted = "INSERTED";

    /// <summary>作废申请已受理（官方 <c>DISCARD_ACCEPTED</c>）：非终态。</summary>
    public const string DiscardAccepted = "DISCARD_ACCEPTED";

    /// <summary>发票卡券已作废（官方 <c>DISCARDED</c>）：终态。</summary>
    public const string Discarded = "DISCARDED";
}

/// <summary>
/// 原始回调类型（官方 <c>resource.original_type</c> 取值）—— <b>载荷形态的真正判别式</b>。
/// </summary>
/// <remarks>
/// 官方原文：「加密前的对象类型」；分账动账通知的类型为 <c>profitsharing</c>。
/// 当 <c>event_type</c> 出现同名复用时（如 <c>TRANSACTION.SUCCESS</c>），本字段是<b>唯一可靠</b>的区分依据。
/// </remarks>
public static class WechatPayNotificationOriginalTypes
{
    /// <summary>交易（官方 <c>transaction</c>）。</summary>
    public const string Transaction = "transaction";

    /// <summary>退款（官方 <c>refund</c>）。</summary>
    public const string Refund = "refund";

    /// <summary>分账动账（官方 <c>profitsharing</c>）。</summary>
    public const string ProfitSharing = "profitsharing";

    /// <summary>
    /// 支付分（官方 <c>payscore</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587960"/>
    /// （支付成功回调通知，2026-10-09 逐字段核验；更新时间 2025.09.15）。
    /// 官方原文：<c>resource.original_type</c> 取值为 <c>payscore</c>（「加密前的对象类型为 <c>payscore</c>」）。
    /// </para>
    /// <para>
    /// <b>本常量补上了一处此前的「未核验项」</b>：支付分通知落地时该取值<b>未取得</b>，
    /// 当时按纪律<b>不</b>对 <c>original_type</c> 做任何断言（以免把猜测固化成契约）；
    /// 现已由官方页核实为<b>小写</b> <c>payscore</c> —— 与 <c>event_type</c> 的<b>大写</b>前缀
    /// （<c>PAYSCORE.</c>）<b>大小写不一致</b>，同一产品线两处取值风格不同，勿互相类推。
    /// </para>
    /// <para>
    /// <b>适用面</b>：支付分<b>订单类</b>通知（<c>PAYSCORE.USER_PAID</c> / <c>PAYSCORE.USER_CONFIRM</c>）页
    /// 明确列出本字段；而<b>授权类</b>通知页（<c>…/partner/4012086446</c>）
    /// <b>只列出 <c>resource_type</c>、未列出 <c>original_type</c></b> ⇒ 对授权类通知<b>不得</b>
    /// 假定该字段存在（消费侧须容忍缺失）。
    /// </para>
    /// </remarks>
    public const string PayScore = "payscore";
}
