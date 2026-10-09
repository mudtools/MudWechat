// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Callback;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 支付分订单支付成功通知用例（官方 <c>PAYSCORE.USER_PAID</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组锁定的核心事实</b>：官方原文「支付分订单支付成功通知为 <c>PAYSCORE.USER_PAID</c>」——
/// 支付分通知有<b>自己的 <c>event_type</c> 前缀</b>，这与分账动态通知「复用
/// <c>TRANSACTION.SUCCESS</c>」的形态<b>完全不同</b>（本仓两条都已留档，勿套用规则）。
/// </para>
/// <para>
/// <b>未核验项（诚实记录）</b>：本通知的 <c>resource.original_type</c> 取值本轮<b>未</b>取得
/// （核验覆盖了 <c>event_type</c> 与解密后字段表，未含该字段取值）⇒ 用例<b>不</b>对其做断言，
/// 以免把猜测固化成契约。后续核验后再补守卫。
/// </para>
/// </remarks>
public class PayScoreNotificationTests
{
    /// <summary>合法的支付分支付成功资源明文（字段名照官方原文）。</summary>
    private const string PaidResourceJson =
        "{\"appid\":\"wx-test\",\"mchid\":\"1900000000\",\"out_order_no\":\"PS-ORDER-1\"," +
        "\"service_id\":\"123456\",\"openid\":\"o-test\",\"state\":\"DONE\",\"total_amount\":100," +
        "\"service_introduction\":\"测试服务\",\"need_collection\":true," +
        "\"notify_url\":\"https://example.com/notify\",\"order_id\":\"3008450740201411110007820472\"," +
        "\"attach\":\"biz-1\"," +
        "\"collection\":{\"state\":\"USER_PAID\",\"total_amount\":100,\"paying_amount\":0,\"paid_amount\":100," +
        "\"details\":[{\"seq\":1,\"amount\":100,\"paid_type\":\"NEWTON\"," +
        "\"paid_time\":\"2026-10-09T12:00:00+08:00\",\"transaction_id\":\"4200001234\"," +
        "\"promotion_detail\":[{\"coupon_id\":\"C1\",\"name\":\"满减券\",\"scope\":\"GLOBAL\"," +
        "\"type\":\"CASH\",\"amount\":10,\"stock_id\":\"S1\",\"wechatpay_contribute\":5," +
        "\"merchant_contribute\":5,\"other_contribute\":0,\"currency\":\"CNY\"," +
        "\"goods_detail\":[{\"goods_id\":\"G1\",\"quantity\":1,\"unit_price\":100," +
        "\"discount_amount\":10,\"goods_remark\":\"备注\"}]}]}]}}";

    /// <summary>通知经三闸后 ⇒ <c>GetPayScorePaid()</c> 给出类型化载荷（含三层嵌套）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldExposePayScorePaidPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            PaidResourceJson,
            eventType: WechatPayNotificationEventTypes.PayScoreUserPaid);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.EventType.Should().Be("PAYSCORE.USER_PAID");

        var payload = context.GetPayScorePaid();
        payload.Should().NotBeNull();
        payload!.OutOrderNo.Should().Be("PS-ORDER-1", "业务幂等键");
        payload.TotalAmount.Should().Be(100);
        payload.Collection!.State.Should().Be("USER_PAID");
        payload.Collection.PaidAmount.Should().Be(100);
    }

    /// <summary>
    /// <b>结构锁定</b>：本载荷的 <c>promotion_detail</c> 嵌在 <c>collection.details[]</c> <b>项内</b>
    /// （既不在顶层，也不直接在 <c>collection</c> 下）—— 三种形态里最"深"的一种。
    /// </summary>
    [Fact]
    public async Task PayScorePaidPayload_ShouldNestPromotionDetailInsideEachDetail()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            PaidResourceJson,
            eventType: WechatPayNotificationEventTypes.PayScoreUserPaid);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);
        var payload = context.GetPayScorePaid()!;

        var detail = payload.Collection!.Details!.Single();
        detail.PaidType.Should().Be("NEWTON", "官方 paid_type 取值之一");
        detail.TransactionId.Should().Be("4200001234");

        var promotion = detail.PromotionDetail!.Single();
        promotion.CouponId.Should().Be("C1");
        promotion.WechatpayContribute.Should().Be(5);
        promotion.MerchantContribute.Should().Be(5);
        promotion.GoodsDetail!.Single().GoodsId.Should().Be("G1",
            "优惠的单品明细比收款明细再深一层（三层嵌套全链路可解析）");
    }

    /// <summary>确认订单通知：<c>PAYSCORE.USER_CONFIRM</c>，载荷无 <c>collection</c> 而有 <c>state_description</c>。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldExposePayScoreConfirmPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        const string confirmJson =
            "{\"appid\":\"wx-test\",\"mchid\":\"1900000000\",\"out_order_no\":\"PS-ORDER-2\"," +
            "\"service_id\":\"123456\",\"openid\":\"o-test\",\"state\":\"DOING\"," +
            "\"state_description\":\"USER_CONFIRM\",\"total_amount\":100," +
            "\"service_introduction\":\"测试服务\",\"need_collection\":true," +
            "\"order_id\":\"3008450740201411110007820473\"}";

        var (headers, body) = fixture.CreateNotification(
            confirmJson,
            eventType: WechatPayNotificationEventTypes.PayScoreUserConfirm);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        var payload = context.GetPayScoreConfirm();
        payload.Should().NotBeNull();
        payload!.OutOrderNo.Should().Be("PS-ORDER-2");
        payload.State.Should().Be("DOING");
        payload.StateDescription.Should().Be("USER_CONFIRM");
    }

    /// <summary>事件类型常量锁定（含「支付分自带前缀」这一与分账不同的形态）。</summary>
    [Fact]
    public void PayScoreEventType_ShouldMatchOfficialValue()
    {
        WechatPayNotificationEventTypes.PayScoreUserPaid.Should().Be("PAYSCORE.USER_PAID");

        // 大小写敏感：官方页面原文为大写（检索摘要里的全小写是失真）。
        WechatPayNotificationEventTypes.PayScoreUserConfirm.Should().Be("PAYSCORE.USER_CONFIRM");
        WechatPayNotificationEventTypes.PayScoreUserConfirm.Should().NotBe("payscore.user_confirm",
            "event_type 是字符串等值匹配 ⇒ 大小写错即静默不命中回调");

        // 与分账通知的形态差异：分账复用 TRANSACTION.SUCCESS，支付分用自己的前缀。
        WechatPayNotificationEventTypes.PayScoreUserPaid
            .Should().NotBe(WechatPayNotificationEventTypes.TransactionSuccess);
        WechatPayNotificationEventTypes.PayScoreUserPaid.Should().StartWith("PAYSCORE.");
    }
}
