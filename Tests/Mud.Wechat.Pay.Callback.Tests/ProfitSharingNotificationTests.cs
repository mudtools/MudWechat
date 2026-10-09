// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Callback;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 分账动态通知用例（<b>分账动态通知</b>，官方 <c>chapter8_1_10</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组锁定的核心事实</b>：官方把「分账」与「分账回退」两种通知的 <c>event_type</c>
/// <b>都</b>标为 <c>TRANSACTION.SUCCESS</c> —— 与支付成功通知<b>同值</b>。
/// 因此载荷判别<b>必须</b>走 <c>resource.original_type = profitsharing</c>。
/// </para>
/// <para>
/// 若误按 <c>event_type</c> 选载荷，交易解析器会「成功」返回一个<b>字段全空</b>的对象
/// （未知属性被静默忽略，<b>不抛异常</b>）——这就是本组第二条用例要钉死的静默错位形态。
/// </para>
/// </remarks>
public class ProfitSharingNotificationTests
{
    /// <summary>合法的分账资源明文（字段名照官方原文）。</summary>
    private const string ProfitSharingResourceJson =
        "{\"mchid\":\"1900000000\",\"transaction_id\":\"4200001234\"," +
        "\"order_id\":\"3008450740201411110007820472\",\"out_order_no\":\"PS-ORDER-1\"," +
        "\"receiver\":{\"type\":\"MERCHANT_ID\",\"account\":\"1900000109\",\"amount\":100," +
        "\"description\":\"分给商户1900000109\"}," +
        "\"success_time\":\"2026-10-09T12:00:00+08:00\"}";

    /// <summary>分账通知（三闸全过）⇒ <c>GetProfitSharing()</c> 给出类型化载荷。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldExposeProfitSharingPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            ProfitSharingResourceJson,
            eventType: WechatPayNotificationEventTypes.TransactionSuccess,
            associatedData: WechatPayNotificationOriginalTypes.ProfitSharing,
            originalType: WechatPayNotificationOriginalTypes.ProfitSharing);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.Notification.Resource!.OriginalType.Should().Be("profitsharing",
            "分账通知的判别式是 original_type，而非 event_type");
        context.EventType.Should().Be("TRANSACTION.SUCCESS", "官方分账通知的 event_type 与支付成功同值");

        var payload = context.GetProfitSharing();
        payload.Should().NotBeNull();
        payload!.OutOrderNo.Should().Be("PS-ORDER-1", "业务幂等键");
        payload.OrderId.Should().Be("3008450740201411110007820472");
        payload.Receiver!.Account.Should().Be("1900000109");
        payload.Receiver.Amount.Should().Be(100, "单位分");
    }

    /// <summary>
    /// <b>反例（钉死静默错位）</b>：仅凭 <c>event_type</c> 判定 ⇒ 交易解析器「成功」但字段全空。
    /// </summary>
    /// <remarks>
    /// 该用例的存在意义：证明「按 event_type 自动选载荷」这条看似便利的路<b>不可走</b> ——
    /// 它不报错、不漏异常，只是把结果静默变成空对象，线上表现为「回调收到了但业务字段都读不到」。
    /// </remarks>
    [Fact]
    public async Task EventTypeAlone_IsNotEnough_ToPickTheRightPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            ProfitSharingResourceJson,
            eventType: WechatPayNotificationEventTypes.TransactionSuccess,
            associatedData: WechatPayNotificationOriginalTypes.ProfitSharing,
            originalType: WechatPayNotificationOriginalTypes.ProfitSharing);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        // 交易解析器对分账载荷**不抛异常**，而是返回字段全空的对象 —— 这正是危险之处。
        var asTransaction = context.GetTransaction();
        asTransaction.Should().NotBeNull("未知属性被静默忽略，反序列化不会失败");
        asTransaction!.OutTradeNo.Should().BeNull("分账载荷里没有 out_trade_no ⇒ 按交易解析只能得到空壳");
        asTransaction.Amount.Should().BeNull();
    }

    /// <summary>官方取值常量锁定（含 <c>TRANSACTION.SUCCESS</c> 被两类通知共用这一事实）。</summary>
    [Fact]
    public void NotificationTypeConstants_ShouldMatchOfficialValues()
    {
        WechatPayNotificationEventTypes.TransactionSuccess.Should().Be("TRANSACTION.SUCCESS");
        WechatPayNotificationEventTypes.RefundSuccess.Should().Be("REFUND.SUCCESS");

        WechatPayNotificationOriginalTypes.Transaction.Should().Be("transaction");
        WechatPayNotificationOriginalTypes.Refund.Should().Be("refund");
        WechatPayNotificationOriginalTypes.ProfitSharing.Should().Be("profitsharing",
            "官方原文：分账动账通知的 original_type 为 profitsharing");
    }
}
