// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 订阅通知事件族（V3，3 键）与微信认证事件族（V4，6 键）的载荷绑定用例 —— P8 第四 / 第五批。
/// </summary>
/// <remarks>
/// 重点覆盖 <c>List</c> 项列表的**两形态**：单项（未触发同名兄弟合并）与多项（合并为合成容器）——
/// 这是本仓库投影器语义下最容易出错的一环（见 <c>MpPayloadConverter.RepeatSubscribeMsgItems</c> 备注）。
/// </remarks>
public class MpSubscriptionAndVerificationPayloadBindingTests
{
    private const string Token = "test-token";
    private const string AppId = "wxSubAppId";
    private const string AppKey = "mp-sub";

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

    /// <summary>订阅弹窗（单项）：<c>List</c> 未触发合并 ⇒ 恰 1 项，字段齐全。</summary>
    [Fact]
    public async Task SubscriptionPopup_SingleItem_ShouldBindOneItem()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>subscribe_msg_popup_event</Event>" +
            "<SubscribeMsgPopupEvent><List>" +
            "<TemplateId>TPL-ONE</TemplateId><SubscribeStatusString>accept</SubscribeStatusString><PopupScene>1</PopupScene>" +
            "</List></SubscribeMsgPopupEvent>");
        var result = reader.Read<MpSubscribeMsgPopupPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Items.Should().HaveCount(1);
        result.Payload.Items[0].TemplateId.Should().Be("TPL-ONE");
        result.Payload.Items[0].SubscribeStatusString.Should().Be("accept");
        result.Payload.Items[0].PopupScene.Should().Be("1");
    }

    /// <summary>订阅弹窗（多项，官方示例形态）：同名 <c>List</c> 兄弟被合并 ⇒ 必须仍解析出 **2 项**（不得退化为 1 项或 0 项）。</summary>
    [Fact]
    public async Task SubscriptionPopup_MultipleItems_ShouldBindAllItems()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>subscribe_msg_popup_event</Event>" +
            "<SubscribeMsgPopupEvent>" +
            "<List><TemplateId>TPL-A</TemplateId><SubscribeStatusString>accept</SubscribeStatusString><PopupScene>2</PopupScene></List>" +
            "<List><TemplateId>TPL-B</TemplateId><SubscribeStatusString>reject</SubscribeStatusString><PopupScene>2</PopupScene></List>" +
            "</SubscribeMsgPopupEvent>");
        var result = reader.Read<MpSubscribeMsgPopupPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Items.Should().HaveCount(2, "官方「一次订阅可能有多条通知，带有多个 id」");
        result.Payload.Items.Select(i => i.TemplateId).Should().Equal("TPL-A", "TPL-B");
    }

    /// <summary>发送结果事件：项内带 <c>MsgID</c>/<c>ErrorCode</c>/<c>ErrorStatus</c>（官方拼写 MsgID）。</summary>
    [Fact]
    public async Task SubscriptionSent_ShouldBindResultFields()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>subscribe_msg_sent_event</Event>" +
            "<SubscribeMsgSentEvent><List>" +
            "<TemplateId>TPL-C</TemplateId><MsgID>1700827132819554304</MsgID>" +
            "<ErrorCode>0</ErrorCode><ErrorStatus>success</ErrorStatus>" +
            "</List></SubscribeMsgSentEvent>");
        var result = reader.Read<MpSubscribeMsgSentPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Items.Should().HaveCount(1);
        result.Payload.Items[0].MsgId.Should().Be("1700827132819554304");
        result.Payload.Items[0].ErrorCode.Should().Be("0");
        result.Payload.Items[0].ErrorStatus.Should().Be("success");
        result.Payload.Items[0].PopupScene.Should().BeNull("change/sent 事件的项内不含 PopupScene");
    }

    /// <summary>订阅管理事件：仅推送拒收（<c>reject</c>）。</summary>
    [Fact]
    public async Task SubscriptionChange_ShouldBindReject()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>subscribe_msg_change_event</Event>" +
            "<SubscribeMsgChangeEvent><List>" +
            "<TemplateId>TPL-D</TemplateId><SubscribeStatusString>reject</SubscribeStatusString>" +
            "</List></SubscribeMsgChangeEvent>");
        var result = reader.Read<MpSubscribeMsgChangePayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Items.Should().HaveCount(1);
        result.Payload.Items[0].SubscribeStatusString.Should().Be("reject");
    }

    /// <summary>认证成功类四键共用载荷，<c>ExpiredTime</c> 为整型时间戳。</summary>
    [Theory]
    [InlineData("qualification_verify_success")]
    [InlineData("naming_verify_success")]
    [InlineData("annual_renew")]
    [InlineData("verify_expired")]
    public async Task VerificationSuccessFamily_ShouldBindExpiredTime(string eventName)
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>" + eventName + "</Event><ExpiredTime>1442401156</ExpiredTime>");
        var result = reader.Read<MpVerificationEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.ExpiredTime.Should().Be(1442401156);
    }

    /// <summary>认证失败类两键共用载荷：<c>FailTime</c> + <c>FailReason</c>。</summary>
    [Theory]
    [InlineData("qualification_verify_fail")]
    [InlineData("naming_verify_fail")]
    public async Task VerificationFailFamily_ShouldBindFailFields(string eventName)
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>" + eventName + "</Event><FailTime>1442401122</FailTime><FailReason>by time</FailReason>");
        var result = reader.Read<MpVerificationFailEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.FailTime.Should().Be(1442401122);
        result.Payload.FailReason.Should().Be("by time");
    }

    /// <summary>失败类载荷不含 <c>ExpiredTime</c>、成功类载荷不含 <c>FailTime</c>（两族不同构，不得错绑）。</summary>
    [Fact]
    public async Task VerificationFamilies_ShouldNotBeInterchangeable()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>qualification_verify_fail</Event><FailTime>1442401122</FailTime><FailReason>by time</FailReason>");

        reader.Read<MpVerificationEventPayload>(envelope).Status
            .Should().Be(MpPayloadReadStatus.ContractMismatch,
                "失败事件的契约载荷是 MpVerificationFailEventPayload");
    }
}
