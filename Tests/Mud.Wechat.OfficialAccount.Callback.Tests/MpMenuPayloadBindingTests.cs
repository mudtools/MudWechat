// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 菜单事件嵌套载荷绑定用例（F18：<c>ScanCodeInfo</c> / <c>SendPicsInfo/PicList/item</c> / <c>SendLocationInfo</c>）。
/// </summary>
/// <remarks>
/// 走明文模式（最小化夹具，聚焦<b>绑定语义</b>而非验签链路 —— 后者已由 <c>MpCallbackReceiverTests</c> 覆盖）：
/// 嵌套单对象经 <c>Object&lt;TSingle&gt;</c>、三层列表经 <c>ItemsObject&lt;TItem&gt;</c> + 叶层「容器 + item」豁免表。
/// </remarks>
public class MpMenuPayloadBindingTests
{
    private const string Token = "test-token";
    private const string AppId = "wxMenuAppId";
    private const string AppKey = "mp-menu";

    private static async Task<MpMenuEventPayload?> ReadMenuPayloadAsync(string plainXml)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        const string nonce = "n1";
        var body = "<xml><ToUserName>" + AppId + "</ToUserName>" + plainXml + "</xml>";
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
        var reader = new MpCallbackPayloadReader(registry);

        var result = reader.Read<MpMenuEventPayload>(envelope);
        return result.Status == MpPayloadReadStatus.Matched ? result.Payload : null;
    }

    /// <summary>扫码菜单事件：<c>ScanCodeInfo</c> 单对象嵌套可读（ScanType/ScanResult）。</summary>
    [Fact]
    public async Task ScanCodePush_ShouldBindNestedScanCodeInfo()
    {
        var payload = await ReadMenuPayloadAsync(
            "<MsgType>event</MsgType><Event>scancode_push</Event><EventKey>KEY_SCAN</EventKey>" +
            "<ScanCodeInfo><ScanType>qrcode</ScanType><ScanResult>http://weixin.qq.com/r/abc</ScanResult></ScanCodeInfo>");

        payload.Should().NotBeNull();
        payload!.EventTypeKey.Should().Be("scancode_push");
        payload.ScanCodeInfo.Should().NotBeNull();
        payload.ScanCodeInfo!.ScanType.Should().Be("qrcode");
        payload.ScanCodeInfo.ScanResult.Should().Be("http://weixin.qq.com/r/abc");
    }

    /// <summary>发图菜单事件：三层嵌套 <c>SendPicsInfo/PicList/item/PicMd5Sum</c> 可读（含 Count）。</summary>
    [Fact]
    public async Task PicWeixin_ShouldBindNestedPicListItems()
    {
        var payload = await ReadMenuPayloadAsync(
            "<MsgType>event</MsgType><Event>pic_weixin</Event><EventKey>KEY_PIC</EventKey>" +
            "<SendPicsInfo><Count>2</Count><PicList>" +
            "<item><PicMd5Sum>md5-one</PicMd5Sum></item>" +
            "<item><PicMd5Sum>md5-two</PicMd5Sum></item>" +
            "</PicList></SendPicsInfo>");

        payload.Should().NotBeNull();
        payload!.SendPicsInfo.Should().NotBeNull();
        payload.SendPicsInfo!.Count.Should().Be(2);
        payload.SendPicsInfo.PicList.Should().HaveCount(2);
        payload.SendPicsInfo.PicList[0].PicMd5Sum.Should().Be("md5-one");
        payload.SendPicsInfo.PicList[1].PicMd5Sum.Should().Be("md5-two");
    }

    /// <summary>地理位置选择事件：<c>SendLocationInfo</c> 可读，且 <c>Poiname</c> 官方拼写被正确映射。</summary>
    [Fact]
    public async Task LocationSelect_ShouldBindNestedSendLocationInfo()
    {
        var payload = await ReadMenuPayloadAsync(
            "<MsgType>event</MsgType><Event>location_select</Event><EventKey>KEY_LOC</EventKey>" +
            "<SendLocationInfo><Location_X>23.134521</Location_X><Location_Y>113.358803</Location_Y>" +
            "<Scale>20</Scale><Label>广东省广州市</Label><Poiname>XX大厦</Poiname></SendLocationInfo>");

        payload.Should().NotBeNull();
        payload!.SendLocationInfo.Should().NotBeNull();
        payload.SendLocationInfo!.Latitude.Should().Be("23.134521");
        payload.SendLocationInfo.Longitude.Should().Be("113.358803");
        payload.SendLocationInfo.Scale.Should().Be("20");
        payload.SendLocationInfo.Label.Should().Be("广东省广州市");
        payload.SendLocationInfo.PoiName.Should().Be("XX大厦", "官方拼写为 Poiname（非 PoiName），不得顺手修正");
    }

    /// <summary>未携带嵌套节点的菜单事件（<c>CLICK</c>）⇒ 三个嵌套属性均为 <c>null</c>（处理器不得假设必有值）。</summary>
    [Fact]
    public async Task Click_ShouldLeaveNestedNodesNull()
    {
        var payload = await ReadMenuPayloadAsync(
            "<MsgType>event</MsgType><Event>CLICK</Event><EventKey>KEY_MENU</EventKey>");

        payload.Should().NotBeNull();
        payload!.EventKey.Should().Be("KEY_MENU");
        payload.ScanCodeInfo.Should().BeNull();
        payload.SendPicsInfo.Should().BeNull();
        payload.SendLocationInfo.Should().BeNull();
    }
}
