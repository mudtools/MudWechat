// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 卡券事件族（V2，12 键）与用户授权信息变更事件族（V5，3 键）的载荷绑定用例 —— P8 第二 / 第三批。
/// </summary>
public class MpCardAndAuthorizationPayloadBindingTests
{
    private const string Token = "test-token";
    private const string AppId = "wxCardAppId";
    private const string AppKey = "mp-card";

    private static async Task<(MpCallbackEnvelope Envelope, MpCallbackPayloadReader Reader)> ReceiveAsync(string eventXml)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        const string nonce = "n1";
        var body = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>event</MsgType>" + eventXml + "</xml>";
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);

        var options = new MpCallbackOptions
        {
            Apps =
            {
                [AppKey] = new MpAppCallbackOptions
                {
                    PushToken = Token,
                    AppId = AppId,
                    SecurityMode = MpCallbackSecurityMode.Plain,
                },
            },
        };

        var receiver = new MpCallbackReceiver(
            new TestOptionsMonitor<MpCallbackOptions>(options),
            new InMemoryWechatCallbackReplayGuard());

        var envelope = await receiver.ReceiveAsync(
            AppKey, "timestamp=" + timestamp + "&nonce=" + nonce + "&signature=" + signature, body);

        var registry = new MpPayloadContractRegistry();
        MpPayloadContracts.RegisterAll(registry);
        return (envelope, new MpCallbackPayloadReader(registry));
    }

    /// <summary>审核不通过：<c>RefuseReason</c> 可读（审核通过键共用同一载荷）。</summary>
    [Fact]
    public async Task CardNotPassCheck_ShouldBindRefuseReason()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>card_not_pass_check</Event><CardId>pZI8Fjwsy5fVPRBeD78J4RmqVvBc</CardId>" +
            "<RefuseReason>非法代制</RefuseReason>");
        var result = reader.Read<MpCardCheckEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.CardId.Should().Be("pZI8Fjwsy5fVPRBeD78J4RmqVvBc");
        result.Payload.RefuseReason.Should().Be("非法代制");
    }

    /// <summary>领取事件：数值字段（<c>IsGiveByFriend</c>）与 <c>UnionId</c>（大小写与授权族的 UnionID 不同）可读。</summary>
    [Fact]
    public async Task UserGetCard_ShouldBindFlagsAndUnionId()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>user_get_card</Event><CardId>pZI8Fj</CardId><IsGiveByFriend>1</IsGiveByFriend>" +
            "<UserCardCode>226009850808</UserCardCode><FriendUserName>oFriend</FriendUserName><OuterId>0</OuterId>" +
            "<OldUserCardCode>old-code</OldUserCardCode><OuterStr>12b</OuterStr>" +
            "<IsRestoreMemberCard>0</IsRestoreMemberCard><IsRecommendByFriend>0</IsRecommendByFriend>" +
            "<UnionId>o6_bmasdasdsad6_2sgVt7hMZOPfL</UnionId>");
        var result = reader.Read<MpCardGetEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.IsGiveByFriend.Should().Be(1);
        result.Payload.FriendUserName.Should().Be("oFriend");
        result.Payload.OldUserCardCode.Should().Be("old-code");
        result.Payload.UnionId.Should().Be("o6_bmasdasdsad6_2sgVt7hMZOPfL");
        result.Payload.IsRestoreMemberCard.Should().Be(0);
    }

    /// <summary>核销事件：<c>ConsumeSource</c> 值域与核销员字段可读。</summary>
    [Fact]
    public async Task UserConsumeCard_ShouldBindConsumeSourceAndStaff()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>user_consume_card</Event><CardId>pZI8Fj8y</CardId><UserCardCode>452998530302</UserCardCode>" +
            "<ConsumeSource>FROM_MOBILE_HELPER</ConsumeSource><LocationName>门店A</LocationName>" +
            "<StaffOpenId>oZ***nJ3bPJu_Rtjkw4c</StaffOpenId><OuterStr>xxxxx</OuterStr>");
        var result = reader.Read<MpCardConsumeEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.ConsumeSource.Should().Be("FROM_MOBILE_HELPER");
        result.Payload.LocationName.Should().Be("门店A");
        result.Payload.StaffOpenId.Should().Be("oZ***nJ3bPJu_Rtjkw4c");
    }

    /// <summary>买单事件：金额字段按整数（单位分）绑定。</summary>
    [Fact]
    public async Task UserPayFromPayCell_ShouldBindAmountsAsNumbers()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>user_pay_from_pay_cell</Event><CardId>po2VNuCuRo</CardId><UserCardCode>38050000000</UserCardCode>" +
            "<TransId>10022403432015000000000</TransId><LocationId>291710000</LocationId>" +
            "<Fee>10000</Fee><OriginalFee>10000</OriginalFee>");
        var result = reader.Read<MpCardPayCellEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.TransId.Should().Be("10022403432015000000000");
        result.Payload.LocationId.Should().Be(291710000);
        result.Payload.Fee.Should().Be(10000);
        result.Payload.OriginalFee.Should().Be(10000);
    }

    /// <summary>券点流水：该事件**无 CardId**，字段为订单与券点数量。</summary>
    [Fact]
    public async Task CardPayOrder_ShouldBindOrderFieldsWithoutCardId()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>card_pay_order</Event><OrderId>404091456</OrderId>" +
            "<Status>ORDER_STATUS_FINANCE_SUCC</Status><CreateOrderTime>1453295737</CreateOrderTime>" +
            "<PayFinishTime>0</PayFinishTime><FreeCoinCount>200</FreeCoinCount><PayCoinCount>0</PayCoinCount>" +
            "<RefundFreeCoinCount>0</RefundFreeCoinCount><RefundPayCoinCount>0</RefundPayCoinCount>" +
            "<OrderType>ORDER_TYPE_SYS_ADD</OrderType><Memo>开通账户奖励</Memo>");
        var result = reader.Read<MpCardPayOrderEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.OrderId.Should().Be("404091456");
        result.Payload.Status.Should().Be("ORDER_STATUS_FINANCE_SUCC");
        result.Payload.FreeCoinCount.Should().Be(200);
        result.Payload.OrderType.Should().Be("ORDER_TYPE_SYS_ADD");
        result.Payload.Memo.Should().Be("开通账户奖励");
    }

    /// <summary>简式卡券事件三键共用载荷（删除 / 从卡券进入会话 / 会员卡激活）。</summary>
    [Theory]
    [InlineData("user_del_card")]
    [InlineData("user_enter_session_from_card")]
    [InlineData("submit_membercard_user_info")]
    public async Task CardSimpleEvents_ShouldSharePayload(string eventName)
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>" + eventName + "</Event><CardId>card-id</CardId><UserCardCode>12312312</UserCardCode>");
        var result = reader.Read<MpCardSimpleEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.CardId.Should().Be("card-id");
        result.Payload.UserCardCode.Should().Be("12312312");
    }

    /// <summary>库存报警：<c>Detail</c> 可读（该事件发送方为「微信」）。</summary>
    [Fact]
    public async Task CardSkuRemind_ShouldBindDetail()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>card_sku_remind</Event><CardId>pa3LFuAh2P65</CardId>" +
            "<Detail>the card's quantity is equal to 0</Detail>");
        var result = reader.Read<MpCardSkuRemindEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Detail.Should().Be("the card's quantity is equal to 0");
    }

    /// <summary>用户授权资料撤回：<c>RevokeInfo</c> 值域可读（用于精确清理范围）。</summary>
    [Fact]
    public async Task UserAuthorizationRevoke_ShouldBindRevokeInfo()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>user_authorization_revoke</Event><OpenID>owAqB1nqaOYYWl0Ng484G2z5NIwU</OpenID>" +
            "<UnionID>union-id</UnionID><AppID>" + AppId + "</AppID><RevokeInfo>203</RevokeInfo>");
        var result = reader.Read<MpAuthorizationEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.OpenId.Should().Be("owAqB1nqaOYYWl0Ng484G2z5NIwU");
        result.Payload.UnionId.Should().Be("union-id");
        result.Payload.AppId.Should().Be(AppId);
        result.Payload.RevokeInfo.Should().Be("203", "203 = 卡券信息（取值表口径，官方 XML 示例值有出入）");
    }

    /// <summary>授权族三键共用载荷（资料变更 / 撤回 / 完成注销）。</summary>
    [Theory]
    [InlineData("user_info_modified")]
    [InlineData("user_authorization_revoke")]
    [InlineData("user_authorization_cancellation")]
    public async Task AuthorizationEvents_ShouldSharePayload(string eventName)
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>" + eventName + "</Event><OpenID>open-id</OpenID><AppID>" + AppId + "</AppID>");
        var result = reader.Read<MpAuthorizationEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.OpenId.Should().Be("open-id");
    }
}
