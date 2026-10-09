// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Callback;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 电子发票五类通知用例（官方 <c>FAPIAO.*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本轮核验的核心事实</b>（官方商户文档中心，更新时间均为 2025.09.26）：
/// </para>
/// <list type="bullet">
/// <item><c>FAPIAO.USER_APPLIED</c> 用户发票抬头填写完成（<c>4012286009</c>）——<b>三</b>字段载荷；</item>
/// <item><c>FAPIAO.ISSUED</c> 发票开具成功（<c>4012286057</c>）；</item>
/// <item><c>FAPIAO.CARD_INSERTED</c> 发票插入用户卡包成功（<c>4012286082</c>）；</item>
/// <item><c>FAPIAO.REVERSED</c> 发票冲红成功（<c>…/fapiao-applications/invoice-flush-success-notice.html</c>）；</item>
/// <item><c>FAPIAO.CARD_DISCARDED</c> 发票卡券作废（<c>…/fapiao-card-template/invoice-card-cancel-notice.html</c>）。</item>
/// </list>
/// <para>
/// <b>两条反直觉事实（本组用例重点锁定）</b>：
/// ① 后<b>四</b>类事件<b>共用同一张</b>载荷字段表（差别只在两个状态字段的取值），故共用<b>一个</b> DTO；
/// ② 这五类通知的 <c>resource</c> <b>都没有</b> <c>original_type</c> —— 判别<b>只能</b>靠
/// <c>event_type</c>，与分账通知「必须靠 <c>original_type</c>」的形态<b>正好相反</b>。
/// </para>
/// </remarks>
public class FapiaoNotificationTests
{
    /// <summary>四类共用载荷的合法明文（字段名照官方原文）。</summary>
    private const string FapiaoResourceJson =
        "{\"mchid\":\"1900000000\",\"fapiao_apply_id\":\"FA-1\"," +
        "\"fapiao_information\":[{\"fapiao_id\":\"MP-1\",\"fapiao_status\":\"ISSUED\"," +
        "\"card_status\":\"INSERTED\"}]}";

    /// <summary>抬头填写完成通知的明文（三字段，与其余四类<b>不同形态</b>）。</summary>
    private const string UserAppliedResourceJson =
        "{\"mchid\":\"1900000000\",\"fapiao_apply_id\":\"4200001234\"," +
        "\"apply_time\":\"2026-10-09T12:00:00+08:00\"}";

    /// <summary>
    /// <b>四类事件共用一个载荷类型</b>：四种 <c>event_type</c> 都能被
    /// <c>GetFapiao()</c> 正确解析（官方四页字段表逐项一致）。
    /// </summary>
    [Theory]
    [InlineData("FAPIAO.ISSUED")]
    [InlineData("FAPIAO.CARD_INSERTED")]
    [InlineData("FAPIAO.REVERSED")]
    [InlineData("FAPIAO.CARD_DISCARDED")]
    public async Task ReceiveAsync_ShouldExposeFapiaoPayload_ForAllFourEvents(string eventType)
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(FapiaoResourceJson, eventType: eventType);
        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.EventType.Should().Be(eventType);

        var payload = context.GetFapiao();
        payload.Should().NotBeNull();
        payload!.FapiaoApplyId.Should().Be("FA-1");

        var information = payload.FapiaoInformation!.Single();
        information.FapiaoId.Should().Be("MP-1");
        information.FapiaoStatus.Should().Be(FapiaoStatuses.Issued);
        information.CardStatus.Should().Be(FapiaoCardStatuses.Inserted);
    }

    /// <summary>
    /// <b>判别不依赖 <c>original_type</c></b>：官方这几类通知的 <c>resource</c> 里<b>没有</b>该字段，
    /// 故即便报文带了别的值（甚至缺省），按 <c>event_type</c> 仍必须解析正确。
    /// </summary>
    [Fact]
    public async Task ReceiveAsync_ShouldNotDependOnOriginalType_ForFapiaoNotifications()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        // 刻意给一个**不属于**电子发票的 original_type（官方此族通知根本不带它）。
        var (headers, body) = fixture.CreateNotification(
            FapiaoResourceJson,
            eventType: WechatPayNotificationEventTypes.FapiaoIssued,
            originalType: WechatPayNotificationOriginalTypes.Transaction);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.GetFapiao().Should().NotBeNull(
            "电子发票通知的判别式是 event_type —— 不得因为 original_type 不是 fapiao 就解析失败");
    }

    /// <summary>抬头填写完成通知：三字段载荷，走专用访问器。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldExposeFapiaoUserAppliedPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            UserAppliedResourceJson,
            eventType: WechatPayNotificationEventTypes.FapiaoUserApplied);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        var payload = context.GetFapiaoUserApplied();
        payload.Should().NotBeNull();
        payload!.FapiaoApplyId.Should().Be("4200001234",
            "微信支付场景下该字段就是**微信支付订单号**（官方原文）");
        payload.ApplyTime.Should().Be("2026-10-09T12:00:00+08:00");
    }

    /// <summary>
    /// <b>错用访问器的形态（静默错位演示）</b>：用四类共用的 <c>GetFapiao()</c> 去解抬头载荷<b>不会</b>抛异常，
    /// 但 <c>fapiao_information</c> 恒为 <c>null</c> ⇒ 消费侧<b>必须</b>先判 <c>event_type</c> 再选访问器。
    /// </summary>
    [Fact]
    public async Task GetFapiao_ShouldYieldEmptyInformation_WhenUsedOnUserAppliedPayload()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        var (headers, body) = fixture.CreateNotification(
            UserAppliedResourceJson,
            eventType: WechatPayNotificationEventTypes.FapiaoUserApplied);

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        var wrongShape = context.GetFapiao();
        wrongShape.Should().NotBeNull("反序列化不会失败（字段恰好兼容）");
        wrongShape!.FapiaoInformation.Should().BeNull(
            "这正是「反序列化成功但形态错」的静默错位：取舍必须由 event_type 决定");
    }

    /// <summary>事件类型常量锁定（官方原文逐字）。</summary>
    [Fact]
    public void FapiaoEventTypes_ShouldMatchOfficialValues()
    {
        WechatPayNotificationEventTypes.FapiaoUserApplied.Should().Be("FAPIAO.USER_APPLIED");
        WechatPayNotificationEventTypes.FapiaoIssued.Should().Be("FAPIAO.ISSUED");
        WechatPayNotificationEventTypes.FapiaoCardInserted.Should().Be("FAPIAO.CARD_INSERTED");
        WechatPayNotificationEventTypes.FapiaoReversed.Should().Be("FAPIAO.REVERSED");
        WechatPayNotificationEventTypes.FapiaoCardDiscarded.Should().Be("FAPIAO.CARD_DISCARDED");

        // 全部带 FAPIAO. 前缀（与 PAYSCORE. 同款形态：自带前缀，不靠 original_type）。
        foreach (var value in new[]
                 {
                     WechatPayNotificationEventTypes.FapiaoUserApplied,
                     WechatPayNotificationEventTypes.FapiaoIssued,
                     WechatPayNotificationEventTypes.FapiaoCardInserted,
                     WechatPayNotificationEventTypes.FapiaoReversed,
                     WechatPayNotificationEventTypes.FapiaoCardDiscarded,
                 })
        {
            value.Should().StartWith("FAPIAO.");
        }

        // 「开具成功」是**事件**（FAPIAO.ISSUED），与「已开具」这个**状态值**（ISSUED）不同 ⇒ 不得互相赋值。
        WechatPayNotificationEventTypes.FapiaoIssued.Should().NotBe(FapiaoStatuses.Issued);
        WechatPayNotificationEventTypes.FapiaoIssued.Should().EndWith(FapiaoStatuses.Issued,
            "事件名只是**恰好**以同名状态值结尾（FAPIAO. + ISSUED）—— 拼写相近但语域完全不同，" +
            "故断言「不相等」而非「相等」：把事件名当状态值用即判错失败");
    }

    /// <summary>
    /// <b>两组状态枚举</b>：<c>fapiao_status</c>（开票/冲红线）与 <c>card_status</c>（卡券线）
    /// 是两套不同维度的值域，官方四页给出的取值<b>完全一致</b>。
    /// </summary>
    [Fact]
    public void FapiaoStatusEnums_ShouldMatchOfficialValues()
    {
        FapiaoStatuses.IssueAccepted.Should().Be("ISSUE_ACCEPTED");
        FapiaoStatuses.Issued.Should().Be("ISSUED");
        FapiaoStatuses.ReverseAccepted.Should().Be("REVERSE_ACCEPTED");
        FapiaoStatuses.Reversed.Should().Be("REVERSED");

        FapiaoCardStatuses.InsertAccepted.Should().Be("INSERT_ACCEPTED");
        FapiaoCardStatuses.Inserted.Should().Be("INSERTED");
        FapiaoCardStatuses.DiscardAccepted.Should().Be("DISCARD_ACCEPTED");
        FapiaoCardStatuses.Discarded.Should().Be("DISCARDED");

        // 两套枚举**取值集合不相交**（防后来者「合并成一套」）：
        // 唯一同名风险点是「已开具 / 已插卡」都表示完成，但拼写不同，故不会误判。
        var fapiaoValues = typeof(FapiaoStatuses)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
        var cardValues = typeof(FapiaoCardStatuses)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

        fapiaoValues.Should().HaveCount(4, "官方 fapiao_status 为 4 值");
        cardValues.Should().HaveCount(4, "官方 card_status 为 4 值");
        fapiaoValues.Intersect(cardValues, StringComparer.Ordinal).Should().BeEmpty(
            "两套状态值域不相交 ⇒ 用错一组会得到「永远不命中」的死分支");
    }
}
