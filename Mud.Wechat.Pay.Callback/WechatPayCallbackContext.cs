// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 支付通知上下文：<b>已通过三道闸（验签 / 时效 / 一次性指纹）且 <c>resource</c> 已解密</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>本上下文是「可信边界」的产物</b>：拿到它即表示报文来源可信、未被篡改、不是重放。
/// 处理器只需关注业务（<c>out_trade_no</c> 幂等落库）。
/// </para>
/// <para>
/// <b>红线（PAY-B7）</b>：<see cref="ResourceJson"/> 是<b>解密后的明文</b>（含 <c>openid</c> 等），
/// <b>不得</b>写入日志 / 遥测 / 异常消息。
/// </para>
/// </remarks>
public sealed class WechatPayCallbackContext
{
    private readonly string _resourceJson;

    internal WechatPayCallbackContext(
        string merchantKey,
        WechatPayMerchantConfig merchant,
        WechatPayNotification notification,
        string resourceJson,
        DateTimeOffset receivedAt)
    {
        MerchantKey = merchantKey;
        Merchant = merchant;
        Notification = notification;
        _resourceJson = resourceJson;
        ReceivedAt = receivedAt;
    }

    /// <summary>命中的商户键（= 路由段 = <c>IWechatPayMerchantManager</c> 的键）。</summary>
    public string MerchantKey { get; }

    /// <summary>命中的商户配置（提供 <c>MchId</c> / <c>AuthorizationMchId</c> 等）。</summary>
    public WechatPayMerchantConfig Merchant { get; }

    /// <summary>通知信封（<c>event_type</c> / <c>resource_type</c> / <c>summary</c> / <c>id</c>）。</summary>
    public WechatPayNotification Notification { get; }

    /// <summary>通知事件类型（<c>event_type</c> 快捷方式）。</summary>
    public string? EventType => Notification.EventType;

    /// <summary>通知 ID（<c>id</c> 快捷方式；官方唯一标识，可作为幂等键的补充）。</summary>
    public string? NotificationId => Notification.Id;

    /// <summary>接收时间（本地时钟，用于诊断；<b>不是</b>官方时间）。</summary>
    public DateTimeOffset ReceivedAt { get; }

    /// <summary>
    /// 解密后的 <c>resource</c> JSON 明文。
    /// </summary>
    /// <remarks>
    /// <b>不得入日志</b>（PAY-B7）。首选 <see cref="GetTransaction"/> / <see cref="GetRefund"/> 取类型化载荷；
    /// 仅当通知载荷为尚未建模的类型时才直接读本属性。
    /// </remarks>
    public string ResourceJson => _resourceJson;

    /// <summary>把解密后的载荷按<b>交易</b>（<c>TRANSACTION.*</c>）类型化解析。</summary>
    /// <returns>解析失败或载荷非交易形态时返回 <c>null</c>。</returns>
    /// <remarks>走源生成 <c>JsonTypeInfo</c> 快车道（零反射、AOT 安全），不使用反射版 <c>Deserialize&lt;T&gt;</c>。</remarks>
    public WechatPayTransactionResource? GetTransaction()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayTransactionResource);

    /// <summary>把解密后的载荷按<b>退款</b>（<c>REFUND.*</c>）类型化解析。</summary>
    /// <returns>解析失败或载荷非退款形态时返回 <c>null</c>。</returns>
    public WechatPayRefundResource? GetRefund()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayRefundResource);

    /// <summary>
    /// 把解密后的载荷按<b>分账</b>（<c>original_type = profitsharing</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非分账形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>⚠️ 调用前必须自行判 <c>resource.original_type</c></b>：分账动态通知的 <c>event_type</c> 与
    /// 支付成功通知<b>同为</b> <c>TRANSACTION.SUCCESS</c>（官方原文），只看 <c>event_type</c> 会把交易载荷
    /// 按分账解析 —— 字段大面积为空但<b>不报错</b>，是最危险的静默错位。判别式见
    /// <see cref="WechatPayNotificationOriginalTypes.ProfitSharing"/>。
    /// </para>
    /// <para>
    /// 本上下文<b>刻意不做</b>「按 <c>event_type</c> 自动选载荷」的便利方法：官方事件类型存在同名复用，
    /// 任何自动选择都会在复用场景下静默选错；把判别显式留给调用方是唯一诚实的形态。
    /// </para>
    /// </remarks>
    public WechatPayProfitSharingResource? GetProfitSharing()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayProfitSharingResource);

    /// <summary>
    /// 把解密后的载荷按<b>支付分订单支付成功</b>（<c>event_type = PAYSCORE.USER_PAID</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非本形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <b>判别方式</b>：支付分通知有<b>自己的 <c>event_type</c> 前缀</b>（<c>PAYSCORE.</c>），
    /// 与分账通知「复用 <c>TRANSACTION.SUCCESS</c>、需靠 <c>original_type</c> 区分」的形态<b>不同</b>
    /// ⇒ 本方法可在判定 <c>event_type == PAYSCORE.USER_PAID</c> 后调用；
    /// 但若将来出现其它 <c>PAYSCORE.*</c> 事件，仍须结合载荷判别，勿假定「前缀相同即同形态」。
    /// </remarks>
    public WechatPayPayScorePaidResource? GetPayScorePaid()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayPayScorePaidResource);

    /// <summary>
    /// 把解密后的载荷按<b>支付分订单确认成功</b>（<c>event_type = PAYSCORE.USER_CONFIRM</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非本形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <b>判别方式</b>：凭 <c>event_type</c>（<b>大写</b> <c>PAYSCORE.USER_CONFIRM</c>，大小写敏感）。
    /// 本载荷<b>无</b> <c>collection</c> / <c>notify_url</c>，与支付成功载荷（<c>PAYSCORE.USER_PAID</c>）
    /// 是<b>两个</b>类型 —— 不要因为「都是 <c>PAYSCORE.*</c>」就按同一类型解析。
    /// </remarks>
    public WechatPayPayScoreConfirmResource? GetPayScoreConfirm()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayPayScoreConfirmResource);

    /// <summary>
    /// 把解密后的载荷按<b>支付分开启/解除授权</b>
    /// （<c>event_type = PAYSCORE.USER_OPEN_SERVICE</c> 或 <c>PAYSCORE.USER_CLOSE_SERVICE</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非本形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>两种事件共用一个载荷类型</b>（官方同一页给出两类通知的同一张字段表）：
    /// 授权成功与解除授权成功的差别体现在 <c>user_service_status</c>（及 <c>openorclose_time</c>），
    /// 而<b>不是</b>字段集合 ⇒ 分建两个类型只会让同一份事实在两处漂移。
    /// </para>
    /// <para>
    /// <b>判别方式</b>：凭 <c>event_type</c>（大写，见
    /// <see cref="WechatPayNotificationEventTypes.PayScoreUserOpenService"/>）。官方该页
    /// <b>未列出</b> <c>resource.original_type</c>（只有 <c>resource_type</c>）
    /// ⇒ 本载荷的判别<b>不可</b>依赖 <c>original_type</c>。
    /// </para>
    /// <para>
    /// <b>面（重要）</b>：本载荷字段表来自官方的<b>从业机构（支付机构）·支付分免确认模式</b>页，
    /// 含 <c>sub_appid</c> / <c>sub_mchid</c> / <c>channel_id</c> 等<b>从业机构侧</b>字段；
    /// 普通商户侧<b>未</b>出现该页 ⇒ 普通商户接入时应以自身收到的实际报文为准，
    /// 未填字段解析为 <c>null</c> 属正常。
    /// </para>
    /// </remarks>
    public WechatPayPayScoreAuthorizationResource? GetPayScoreAuthorization()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayPayScoreAuthorizationResource);

    /// <summary>
    /// 把解密后的载荷按<b>电子发票</b>（<c>FAPIAO.ISSUED</c> / <c>FAPIAO.CARD_INSERTED</c> /
    /// <c>FAPIAO.REVERSED</c> / <c>FAPIAO.CARD_DISCARDED</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非本形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>四类事件共用本载荷</b>（官方四页字段表逐项一致）：拿到载荷后须再看
    /// <c>fapiao_information[].fapiao_status</c> / <c>card_status</c> 才能判断到了哪一步。
    /// </para>
    /// <para>
    /// <b>判别只能靠 <c>event_type</c></b>：这些通知的 <c>resource</c> <b>没有</b>
    /// <c>original_type</c>（与分账通知相反）⇒ 不得依赖该字段。
    /// </para>
    /// </remarks>
    public WechatPayFapiaoResource? GetFapiao()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayFapiaoResource);

    /// <summary>
    /// 把解密后的载荷按<b>用户发票抬头填写完成</b>（<c>event_type = FAPIAO.USER_APPLIED</c>）类型化解析。
    /// </summary>
    /// <returns>解析失败或载荷非本形态时返回 <c>null</c>。</returns>
    /// <remarks>
    /// <b>与其余四类发票通知<b>不是</b>同一形态</b>：本载荷无 <c>fapiao_information</c>，
    /// 只有 <c>mchid</c> / <c>fapiao_apply_id</c> / <c>apply_time</c> ⇒ 不可用
    /// <see cref="GetFapiao"/> 代替：反序列化<b>不会</b>失败（<c>mchid</c> / <c>fapiao_apply_id</c> 恰好同名），
    /// 但 <c>FapiaoInformation</c> 恒为 <c>null</c> —— 这是典型的「解析成功但形态错」静默错位
    /// （用例 <c>GetFapiao_ShouldYieldEmptyInformation_WhenUsedOnUserAppliedPayload</c> 已锁死该形态）。
    /// </remarks>
    public WechatPayFapiaoUserAppliedResource? GetFapiaoUserApplied()
        => TryDeserialize(WechatPayCallbackJsonContext.Default.WechatPayFapiaoUserAppliedResource);

    private T? TryDeserialize<T>(System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
        where T : class
    {
        try
        {
            return JsonSerializer.Deserialize(_resourceJson, typeInfo);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
