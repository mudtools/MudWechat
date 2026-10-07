// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 通用事件（<c>subscribe</c>/<c>unsubscribe</c>/<c>SCAN</c>/<c>LOCATION</c>）载荷绑定用例 —— P8 第一批。
/// </summary>
/// <remarks>
/// 字段依据：官方「接收事件推送」页（V1 已核验）：
/// 关注/取关无专有字段（扫码关注额外带 <c>EventKey</c>=<c>qrscene_*</c> + <c>Ticket</c>）、
/// <c>SCAN</c> 带 <c>EventKey</c>+<c>Ticket</c>、<c>LOCATION</c> 带 <c>Latitude</c>/<c>Longitude</c>/<c>Precision</c>。
/// </remarks>
public class MpCommonEventPayloadBindingTests
{
    private const string Token = "test-token";
    private const string AppId = "wxCommonAppId";
    private const string AppKey = "mp-common";

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

    /// <summary>普通关注：无专有字段 ⇒ <c>EventKey</c>/<c>Ticket</c> 均为 <c>null</c>。</summary>
    [Fact]
    public async Task Subscribe_Plain_ShouldBindWithoutEventKeyOrTicket()
    {
        var (envelope, reader) = await ReceiveAsync("<Event>subscribe</Event>");
        var result = reader.Read<MpSubscribeEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.EventKey.Should().BeNull();
        result.Payload.Ticket.Should().BeNull();
    }

    /// <summary>扫码关注（未关注）：同键 <c>subscribe</c>，但携带 <c>qrscene_</c> 前缀 EventKey + Ticket。</summary>
    [Fact]
    public async Task Subscribe_ScanQrCode_ShouldBindEventKeyAndTicket()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>subscribe</Event><EventKey>qrscene_123123</EventKey><Ticket>TICKET-XYZ</Ticket>");
        var result = reader.Read<MpSubscribeEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.EventKey.Should().StartWith("qrscene_", "官方以 qrscene_ 前缀区分「普通关注」与「扫码关注」");
        result.Payload.Ticket.Should().Be("TICKET-XYZ");
    }

    /// <summary>取消关注：官方要求处理器<b>必须删除该用户所有信息</b>（此处仅锁字段形态 + 键判别）。</summary>
    [Fact]
    public async Task Unsubscribe_ShouldUseSamePayloadAndExposeKey()
    {
        var (envelope, reader) = await ReceiveAsync("<Event>unsubscribe</Event>");
        var result = reader.Read<MpSubscribeEventPayload>(envelope);

        envelope.EventTypeKey.Should().Be("unsubscribe");
        result.Status.Should().Be(MpPayloadReadStatus.Matched);
    }

    /// <summary>已关注用户扫码：<c>SCAN</c> 的场景值**不带** <c>qrscene_</c> 前缀。</summary>
    [Fact]
    public async Task Scan_ShouldBindSceneValueAndTicketWithoutPrefix()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>SCAN</Event><EventKey>SCENE_VALUE</EventKey><Ticket>TICKET-002</Ticket>");
        var result = reader.Read<MpScanEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.EventKey.Should().Be("SCENE_VALUE");
        result.Payload.EventKey.Should().NotStartWith("qrscene_");
        result.Payload.Ticket.Should().Be("TICKET-002");
    }

    /// <summary>
    /// 上报地理位置：<c>LOCATION</c> 事件用 <c>Latitude</c>/<c>Longitude</c>/<c>Precision</c>，
    /// 与普通 <c>location</c> 消息（<c>Location_X</c>/<c>Location_Y</c>/<c>Scale</c>/<c>Label</c>）**结构不同**。
    /// </summary>
    [Fact]
    public async Task LocationEvent_ShouldBindLatitudeLongitudePrecision()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>LOCATION</Event><Latitude>23.137466</Latitude><Longitude>113.352425</Longitude>" +
            "<Precision>119.385040</Precision>");
        var result = reader.Read<MpLocationEventPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Latitude.Should().Be("23.137466");
        result.Payload.Longitude.Should().Be("113.352425");
        result.Payload.Precision.Should().Be("119.385040");
    }

    /// <summary>
    /// 大小写与结构不可混用（官方明确区分）：<c>LOCATION</c> 事件的键与 <c>location</c> 消息的键是**两个不同键**，
    /// 且各自的载荷类型不同（契约不匹配必须被读取器拒绝，而非静默错绑）。
    /// </summary>
    [Fact]
    public async Task LocationEventAndLocationMessage_ShouldBeDistinctContracts()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>LOCATION</Event><Latitude>23.1</Latitude><Longitude>113.3</Longitude><Precision>10</Precision>");

        var mismatch = reader.Read<MpLocationMessagePayload>(envelope);

        mismatch.Status.Should().Be(MpPayloadReadStatus.ContractMismatch,
            "LOCATION 事件的契约载荷是 MpLocationEventPayload，请求普通消息载荷属宿主接线错误");
        mismatch.Payload.Should().BeNull();
    }
}
