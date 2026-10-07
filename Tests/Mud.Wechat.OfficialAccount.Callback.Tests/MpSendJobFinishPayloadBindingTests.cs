// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 消息发送结果事件族（P0-e：群发 MASSSENDJOBFINISH + 模板 TEMPLATESENDJOBFINISH 大小写双形态）
/// 的载荷绑定用例 —— 官方 XML 报文示例原文回放。
/// </summary>
public class MpSendJobFinishPayloadBindingTests
{
    private const string Token = "test-token";
    private const string AppId = "wxFinishAppId";
    private const string AppKey = "mp-finish";

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

    /// <summary>SJ1：群发结果事件（官方示例 XML 原文回放——err(30003) 形态，含原创校验双层嵌套列表）。</summary>
    [Fact]
    public async Task MassSendJobFinish_ShouldBindCopyrightNestedItems()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>MASSSENDJOBFINISH</Event>" +
            "<FromUserName>oV5CrjpxgaGXNHIQigzNlgLTnwic</FromUserName>" +
            "<CreateTime>1481013459</CreateTime>" +
            "<MsgID>1000001625</MsgID>" +
            "<Status><![CDATA[err(30003)]]></Status>" +
            "<TotalCount>0</TotalCount><FilterCount>0</FilterCount><SentCount>0</SentCount><ErrorCount>0</ErrorCount>" +
            "<CopyrightCheckResult>" +
            "<Count>2</Count>" +
            "<ResultList>" +
            "<item><ArticleIdx>1</ArticleIdx><UserDeclareState>0</UserDeclareState><AuditState>2</AuditState>" +
            "<OriginalArticleUrl><![CDATA[Url_1]]></OriginalArticleUrl><OriginalArticleType>1</OriginalArticleType>" +
            "<CanReprint>1</CanReprint><NeedReplaceContent>1</NeedReplaceContent><NeedShowReprintSource>1</NeedShowReprintSource></item>" +
            "<item><ArticleIdx>2</ArticleIdx><UserDeclareState>0</UserDeclareState><AuditState>2</AuditState>" +
            "<OriginalArticleUrl><![CDATA[Url_2]]></OriginalArticleUrl><OriginalArticleType>1</OriginalArticleType>" +
            "<CanReprint>1</CanReprint><NeedReplaceContent>1</NeedReplaceContent><NeedShowReprintSource>1</NeedShowReprintSource></item>" +
            "</ResultList>" +
            "<CheckState>2</CheckState>" +
            "</CopyrightCheckResult>" +
            "<ArticleUrlResult><Count>1</Count><ResultList><item><ArticleIdx>1</ArticleIdx>" +
            "<ArticleUrl><![CDATA[Url]]></ArticleUrl></item></ResultList></ArticleUrlResult>");

        var result = reader.Read<MpMassSendJobFinishPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched, "官方现网 Event 为大写 MASSSENDJOBFINISH ⇒ 必须命中强类型载荷");
        result.Payload!.MsgId.Should().Be(1000001625);
        result.Payload.Status.Should().Be("err(30003)");
        result.Payload.TotalCount.Should().Be(0);
        result.Payload.FilterCount.Should().Be(0);
        result.Payload.SentCount.Should().Be(0);
        result.Payload.ErrorCount.Should().Be(0);

        result.Payload.CopyrightCheckResult.Should().NotBeNull();
        result.Payload.CopyrightCheckResult!.Count.Should().Be(2);
        result.Payload.CopyrightCheckResult.CheckState.Should().Be(2, "被判为转载可群发（官方 CheckState 语义）");
        result.Payload.CopyrightCheckResult.ResultList.Should().HaveCount(2, "ResultList/item 双层嵌套须逐项绑定");
        result.Payload.CopyrightCheckResult.ResultList[0].ArticleIdx.Should().Be(1);
        result.Payload.CopyrightCheckResult.ResultList[1].OriginalArticleUrl.Should().Be("Url_2");
        result.Payload.CopyrightCheckResult.ResultList[0].NeedShowReprintSource.Should().Be(1);

        result.Payload.ArticleUrlResult.Should().NotBeNull();
        result.Payload.ArticleUrlResult!.ResultList.Should().HaveCount(1);
        result.Payload.ArticleUrlResult.ResultList[0].ArticleUrl.Should().Be("Url");
    }

    /// <summary>SJ2：非图文群发（无 CopyrightCheckResult/ArticleUrlResult 节点）⇒ 嵌套属性为 null、顶层计数照常绑定。</summary>
    [Fact]
    public async Task MassSendJobFinish_WithoutCopyrightNodes_ShouldBindTopLevelOnly()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>MASSSENDJOBFINISH</Event>" +
            "<MsgID>2000001</MsgID>" +
            "<Status><![CDATA[send success]]></Status>" +
            "<TotalCount>100</TotalCount><FilterCount>98</FilterCount><SentCount>96</SentCount><ErrorCount>2</ErrorCount>");

        var result = reader.Read<MpMassSendJobFinishPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.SentCount.Should().Be(96);
        result.Payload.FilterCount.Should().Be(98);
        result.Payload.CopyrightCheckResult.Should().BeNull("非图文群发不携带原创校验节点");
        result.Payload.ArticleUrlResult.Should().BeNull();
    }

    /// <summary>SJ3：模板结果事件——官方大写形态命中（Status 三形态取 success 样例）。</summary>
    [Fact]
    public async Task TemplateSendJobFinish_UpperCaseEvent_ShouldBind()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>TEMPLATESENDJOBFINISH</Event>" +
            "<MsgID>200163836</MsgID>" +
            "<Status><![CDATA[success]]></Status>");

        var result = reader.Read<MpTemplateSendJobFinishPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched, "官方现网示例为大写 Event");
        result.Payload!.MsgId.Should().Be(200163836);
        result.Payload.Status.Should().Be("success");
    }

    /// <summary>SJ4：模板结果事件——历史小写形态同样命中（大小写双键登记）。</summary>
    [Fact]
    public async Task TemplateSendJobFinish_LowerCaseEvent_ShouldBindSamePayload()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>templatesendjobfinish</Event>" +
            "<MsgID>200163840</MsgID>" +
            "<Status><![CDATA[failed:user block]]></Status>");

        var result = reader.Read<MpTemplateSendJobFinishPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched, "历史通行小写键已双登记，两形态命中同一载荷");
        result.Payload!.MsgId.Should().Be(200163840);
        result.Payload.Status.Should().Be("failed:user block", "用户拒收为官方三形态之一");
    }

    /// <summary>SJ5：kf 会话三键（官方文档失存裁决）——不登记契约 ⇒ 降级通用载荷（不丢事件）。</summary>
    [Fact]
    public async Task KfSessionEvents_ShouldFallBackToGenericPayload()
    {
        var (envelope, reader) = await ReceiveAsync(
            "<Event>kf_create_session</Event><KfAccount><![CDATA[test1@test]]></KfAccount>");

        var result = reader.Read<GenericCallbackPayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.GenericFallback,
            "kf_* 事件官方文档已失存（2026-10-07 多路核验），按 V6 先例不做假设性建模——" +
            "降级为通用载荷保事件不丢，待官方恢复或实测报文后立项");
        result.Payload!.Values.Should().ContainKey("KfAccount");
        result.Payload.Values["KfAccount"].Should().Be("test1@test");
    }
}
