// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FluentAssertions;
using Mud.Wechat.Pay.DataModels.Callback;
using Mud.Wechat.Pay.DataModels.Combine;
using Xunit;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 合单支付通知用例（<c>TRANSACTION.SUCCESS</c> 的<b>合单形态</b>载荷）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013462574"/>
/// （合单订单支付成功回调通知，2026-10-09 逐字段核验；更新时间 2025.01.16）。
/// </para>
/// <para>
/// <b>本组用例的重点不是「能解析」，而是「解析错也不会报错」这件事</b>：
/// 官方把合单支付通知的 <c>event_type</c> 定为 <c>TRANSACTION.SUCCESS</c>、
/// <c>original_type</c> 定为 <c>transaction</c> —— 与<b>普通支付成功通知完全同值</b>。
/// 而两类载荷的字段名<b>没有一个重叠</b> ⇒ 用错访问器<b>不抛异常</b>、只得到「字段全空」的对象。
/// 这正是本仓最危险的静默错位类型，故用<b>同信封双载荷</b>的用例把它钉死。
/// </para>
/// </remarks>
public class CombineNotificationTests
{
    /// <summary>合单支付通知的解密明文（字段名照官方原文）。</summary>
    private const string CombineResourceJson =
        "{\"combine_appid\":\"wx-combine\",\"combine_mchid\":\"1900000000\"," +
        "\"combine_out_trade_no\":\"COMBINE-1\"," +
        "\"scene_info\":{\"device_id\":\"DEV-1\"}," +
        "\"sub_orders\":[{" +
        "\"mchid\":\"1900000000\",\"trade_type\":\"JSAPI\",\"trade_state\":\"SUCCESS\"," +
        "\"bank_type\":\"OTHERS\",\"attach\":\"A1\",\"success_time\":\"2026-10-09T12:00:00+08:00\"," +
        "\"transaction_id\":\"4200001111\",\"out_trade_no\":\"SUB-1\",\"sub_mchid\":\"1900000001\"," +
        "\"sub_appid\":\"wx-sub\",\"sub_openid\":\"o-sub\"," +
        "\"amount\":{\"total_amount\":100,\"currency\":\"CNY\",\"payer_amount\":90," +
        "\"payer_currency\":\"CNY\",\"settlement_rate\":100000000}," +
        "\"promotion_detail\":[{\"coupon_id\":\"C1\",\"name\":\"满减券\",\"scope\":\"SINGLE\"," +
        "\"type\":\"CASH\",\"amount\":10,\"stock_id\":\"S1\",\"wechatpay_contribute\":10," +
        "\"currency\":\"CNY\",\"goods_detail\":[{\"goods_id\":\"G1\",\"quantity\":1," +
        "\"unit_price\":100,\"discount_amount\":10}]}]}]," +
        "\"combine_payer_info\":{\"openid\":\"o-combine\"}}";

    /// <summary>合单支付成功通知应能取到完整的分层载荷（含子单 / 金额 / 优惠 / 支付者）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldExposeCombineTransactionPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            CombineResourceJson,
            eventType: WechatPayNotificationEventTypes.TransactionSuccess,
            originalType: WechatPayNotificationOriginalTypes.Transaction);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.EventType.Should().Be("TRANSACTION.SUCCESS");

        var payload = context.GetCombineTransaction()!;
        payload.CombineAppId.Should().Be("wx-combine");
        payload.CombineOutTradeNo.Should().Be("COMBINE-1");
        payload.SceneInfo!.DeviceId.Should().Be("DEV-1",
            "通知的 scene_info 只有 device_id（payer_client_ip 是下单入参，不回吐）");
        payload.CombinePayerInfo!.OpenId.Should().Be("o-combine");

        var sub = payload.SubOrders!.Single();
        sub.TradeState.Should().Be(CombineTradeStates.Success);
        sub.TradeType.Should().Be(CombineTradeTypes.JsApi);
        sub.SubMchId.Should().Be("1900000001", "合单的每笔商品单有自己的子商户号");
        sub.BankType.Should().Be("OTHERS",
            "官方原文：非银行卡统一返回 OTHERS —— 它不是「未知」也不是异常");
        sub.PromotionDetail!.Single().CouponId.Should().Be("C1");
        sub.PromotionDetail!.Single().GoodsDetail.Should().HaveCount(1,
            "promotion_detail 复用支付分域类型，其内已含 goods_detail 单品列表");
    }

    /// <summary>
    /// <b>同信封双载荷</b>：<c>TRANSACTION.SUCCESS</c> 既可承载普通支付载荷、也可承载合单载荷，
    /// 只能靠<b>解密后的字段形态</b>判别 —— 用错访问器<b>不会抛异常</b>，只得到「字段全空」。
    /// </summary>
    /// <remarks>
    /// 本用例<b>刻意</b>把两类载荷放进<b>完全相同的信封</b>（同 <c>event_type</c>、同 <c>original_type</c>），
    /// 断言「错用访问器时对象非 null 但关键字段为 null」——这就是消费侧<b>必须做存在性校验</b>的原因；
    /// 若将来有人把两类载荷合并成一个类型，或去掉这层校验，本用例会失败。
    /// </remarks>
    [Fact]
    public async Task SameEnvelope_ShouldRequirePayloadShapeDiscrimination()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        // ① 普通支付载荷 + 普通信封 ⇒ 用合单访问器会得到「非 null 但字段全空」的对象。
        var (normalHeaders, normalBody) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson,
            eventType: WechatPayNotificationEventTypes.TransactionSuccess,
            originalType: WechatPayNotificationOriginalTypes.Transaction);
        var normalContext = await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, normalHeaders, normalBody);

        var misaligned = normalContext.GetCombineTransaction();
        misaligned.Should().NotBeNull(
            "字段名不重叠 ⇒ 反序列化不会失败 —— 这正是静默错位的危险之处");
        misaligned!.CombineOutTradeNo.Should().BeNull(
            "合单标识为空 ⇒ 消费侧必须据此判定「这不是合单通知」，再走普通交易分支");

        // ② 合单载荷 + 同一信封 ⇒ 用普通交易访问器同样「非 null 但字段全空」。
        var (combineHeaders, combineBody) = fixture.CreateNotification(
            CombineResourceJson,
            eventType: WechatPayNotificationEventTypes.TransactionSuccess,
            originalType: WechatPayNotificationOriginalTypes.Transaction);
        var combineContext = await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, combineHeaders, combineBody);

        var reversed = combineContext.GetTransaction();
        reversed.Should().NotBeNull();
        reversed!.OutTradeNo.Should().BeNull();
        reversed.AppId.Should().BeNull("普通支付载荷的 appid 与合单载荷的 combine_appid 是不同字段名");

        // ③ 正解：合单载荷用合单访问器才拿得到标识。
        combineContext.GetCombineTransaction()!.CombineOutTradeNo.Should().Be("COMBINE-1");
    }

    /// <summary>合单的交易状态与交易类型取值锁官方原文（注意 H5 场景的取值是 <c>MWEB</c>）。</summary>
    [Fact]
    public void CombineTradeValues_ShouldMatchOfficialValues()
    {
        var states = typeof(CombineTradeStates)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        states.Should().BeEquivalentTo(new[] { "SUCCESS", "NOTPAY", "CLOSED" });

        var trades = typeof(CombineTradeTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        trades.Should().BeEquivalentTo(new[] { "JSAPI", "NATIVE", "APP", "MWEB" });

        CombineTradeTypes.MWeb.Should().Be("MWEB",
            "H5 下单端点叫 /h5，但官方 trade_type 取值是 MWEB —— 按端点名去匹配回调会永远不命中");
        CombineTradeTypes.MWeb.Should().NotBe("H5");

        // 回调里必须显式判定 SUCCESS：官方取值表里还有 NOTPAY / CLOSED。
        CombineTradeStates.Success.Should().NotBe(CombineTradeStates.NotPay);
        CombineTradeStates.Success.Should().NotBe(CombineTradeStates.Closed);
    }

    /// <summary>
    /// <b>合单退款通知不另设事件类型</b>：退款逐子单走退款域 ⇒ 通知即标准退款结果通知。
    /// </summary>
    /// <remarks>
    /// 官方《订单退款》开发指引明示合单订单<b>只能按子单退款</b>（守卫 CB6 固化），
    /// 退款结果通知因此复用 <c>REFUND.SUCCESS</c> / <c>REFUND.ABNORMAL</c> / <c>REFUND.CLOSED</c>
    /// —— 本用例既锁定这三个取值，也锁定「事件类型里<b>不存在</b>合单专属的退款事件」。
    /// </remarks>
    [Fact]
    public void CombineRefundNotification_ShouldReuseStandardRefundEvents()
    {
        WechatPayNotificationEventTypes.RefundSuccess.Should().Be("REFUND.SUCCESS");
        WechatPayNotificationEventTypes.RefundAbnormal.Should().Be("REFUND.ABNORMAL");
        WechatPayNotificationEventTypes.RefundClosed.Should().Be("REFUND.CLOSED");

        var refundEvents = typeof(WechatPayNotificationEventTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .Where(static v => v.StartsWith("REFUND.", StringComparison.Ordinal))
            .ToArray();

        refundEvents.Should().BeEquivalentTo(new[] { "REFUND.SUCCESS", "REFUND.ABNORMAL", "REFUND.CLOSED" },
            "合单退款没有专属事件类型 —— 若有人新增 REFUND.COMBINE_* 之类的常量，本用例会失败");
    }
}
