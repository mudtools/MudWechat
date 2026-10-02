// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调接收与事件分发测试：抗重放（时效窗口 + 一次性指纹）、指纹闸后移（P1-1）、suite_ticket 入库、
/// cancel_auth/change_auth 的 SuiteId 命中集清理与级联失效（详细设计 §18.4；P0-2 / P0-3）。
/// v1.2：配置迁移到 <see cref="WechatCallbackOptions.Apps"/> 多应用形态（通配键承接），
/// 信封扩展（通讯录变更/异步任务）与 D10 兜底修正同批覆盖。
/// </summary>
public class WechatCallbackReceiverAndHandlerTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string CorpId = "ww-corp";
    private const string AppKey = WechatCallbackOptions.WildcardAppKey;

    /// <summary>当前时间戳（秒）；回调时间窗为 ±300s，报文必须使用当前时间。</summary>
    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    /// <summary>生成唯一 nonce（P0-2 后同一 nonce 的第二次调用会被判为重放）。</summary>
    private static string NewNonce() => "nonce-" + Guid.NewGuid().ToString("N");

    private static WechatCallbackOptions CreateOptions(string receiveId = CorpId)
        => new()
        {
            Apps =
            {
                [AppKey] = new WechatAppCallbackOptions
                {
                    PushToken = Token,
                    PushEncodingAESKey = AesKey,
                    ReceiveId = receiveId,
                },
            },
        };

    private static WechatCallbackReceiver CreateReceiver(WechatCallbackOptions? options = null)
        => new(new TestOptionsMonitor<WechatCallbackOptions>(options ?? CreateOptions()));

    private static string QueryFor(string encrypt, string timestamp, string nonce)
    {
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        return $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}";
    }

    private static (string Encrypt, string Body) EncryptBody(string plainXml, string receiveId = CorpId)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, receiveId);
        return (encrypt, $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>");
    }

    // ---------------------------------------------------------------- 基础解析

    [Fact]
    public async Task ReceiveAsync_ShouldParseSuiteTicketEvent()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody(
            "<xml><SuiteId>ww-suite</SuiteId><InfoType>suite_ticket</InfoType><SuiteTicket>ticket-abc</SuiteTicket></xml>");

        var evt = await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsSuiteTicket.Should().BeTrue();
        evt.SuiteId.Should().Be("ww-suite");
        evt.SuiteTicket.Should().Be("ticket-abc");
        evt.EventTypeKey.Should().Be(WechatCallbackEventTypes.SuiteTicket);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenSignatureMismatch()
    {
        var receiver = CreateReceiver();
        var (_, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var badQuery = $"msg_signature=0000000000000000000000000000000000000000&timestamp={Now()}&nonce={NewNonce()}";

        var act = async () => await receiver.ReceiveAsync(AppKey, badQuery, body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*msg_signature 不匹配*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature, "P2-2：失败类别可供宿主映射应答");
        thrown.Which.Should().BeAssignableTo<InvalidOperationException>("D7：既有 catch (InvalidOperationException) 块零破坏");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenSignatureMissing()
    {
        var receiver = CreateReceiver();
        var (_, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");

        var act = async () => await receiver.ReceiveAsync(
            AppKey, $"timestamp={Now()}&nonce={NewNonce()}", body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*msg_signature*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingSignature);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenEncryptMissing()
    {
        var receiver = CreateReceiver();
        var act = async () => await receiver.ReceiveAsync(
            AppKey, $"msg_signature=abc&timestamp={Now()}&nonce={NewNonce()}", "<xml><Nothing/></xml>");
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*Encrypt*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingEncrypt);
    }

    // ---------------------------------------------------------------- P0-2 抗重放

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampExpired()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds + 1)).ToString();

        var act = async () => await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, stale, NewNonce()), body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*超出时效窗口*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldPass_WhenTimestampWithinWindow()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var nearBoundary = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds - 1)).ToString();

        var act = async () => await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, nearBoundary, NewNonce()), body);
        await act.Should().NotThrowAsync("±299s 在容差内（边界 -1s 留出执行抖动余量）");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampMissing()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var nonce = NewNonce();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, string.Empty, nonce, encrypt);

        var act = async () => await receiver.ReceiveAsync(AppKey, $"msg_signature={signature}&nonce={nonce}", body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*timestamp 缺失或非数字*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingTimestamp);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenNonceMissing()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var timestamp = Now();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, string.Empty, encrypt);

        var act = async () => await receiver.ReceiveAsync(AppKey, $"msg_signature={signature}&timestamp={timestamp}", body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*nonce 缺失*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingNonce);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenNonceReplayed()
    {
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var query = QueryFor(encrypt, Now(), NewNonce());

        await receiver.ReceiveAsync(AppKey, query, body);

        var act = async () => await receiver.ReceiveAsync(AppKey, query, body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*疑似重放*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenReceiveIdMismatch()
    {
        // 套件回调场景：接收方 ID 应为 SuiteId；此处配置错误的企业 CorpId 触发明文完整性拒绝。
        var receiver = CreateReceiver(CreateOptions(receiveId: "ww-wrong-receiveid"));
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");

        var act = async () => await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*receiveid*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldSkipReceiveIdCheck_WhenCorpIdNotConfigured()
    {
        var receiver = CreateReceiver(CreateOptions(receiveId: string.Empty));
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");

        var act = async () => await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);
        await act.Should().NotThrowAsync("未配置接收方 ID 时跳过 receiveid 校验（仅一次性告警，通讯录同步助手形态）");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldSkipReceiveIdCheck_WhenPlaintextReceiveIdEmpty()
    {
        // P3-2：明文未携带 receiveid（官方「个人主体第三方为空串」形态，90968）时跳过校验。
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>", receiveId: string.Empty);

        var act = async () => await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);
        await act.Should().NotThrowAsync("明文 receiveid 为空时跳过校验（个人主体第三方兼容）");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldWarnOnce_WhenPlaintextReceiveIdEmpty()
    {
        // P3-2（M6）：明文未携带 receiveid 时跳过校验并一次性告警——该兼容路径的唯一可观测面。
        var logger = new CapturingLogger<WechatCallbackReceiver>();
        var receiver = new WechatCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()), logger: logger);

        var first = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>", receiveId: string.Empty);
        await receiver.ReceiveAsync(AppKey, QueryFor(first.Encrypt, Now(), NewNonce()), first.Body);
        var second = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>", receiveId: string.Empty);
        await receiver.ReceiveAsync(AppKey, QueryFor(second.Encrypt, Now(), NewNonce()), second.Body);

        logger.Entries.Count(e => e.Level == LogLevel.Warning && e.Message.Contains("明文未携带 receiveid"))
            .Should().Be(1, "P3-2：跳过校验仅首次命中输出一次性告警");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldNotConsumeFingerprint_WhenDecryptFails()
    {
        // P1-1/D2：解密失败不得消耗指纹——错钥接收器失败后，同参数报文对正确凭据接收器仍可进入管线
        //（官方重试语义；两接收器显式共享同一 IWechatCallbackReplayGuard）。
        // 错钥取「43 位但非 Base64 字符」：在 Decrypt 内确定性触发 DecryptFailed（避免错钥随机明文
        // 偶发通过填充校验造成 flake）。
        var invalidBase64Key = new string('!', 43);
        invalidBase64Key.Length.Should().Be(43, "错钥必须通过 app.Validate 的 43 位长度检查，使失败精确发生在 Decrypt 内");

        var guard = new InMemoryWechatCallbackReplayGuard();
        var wrongKeyOptions = CreateOptions();
        wrongKeyOptions.Apps[AppKey].PushEncodingAESKey = invalidBase64Key;
        var wrongKeyReceiver = new WechatCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(wrongKeyOptions), guard);
        var rightKeyReceiver = new WechatCallbackReceiver(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()), guard);

        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");
        var query = QueryFor(encrypt, Now(), NewNonce());

        var failed = async () => await wrongKeyReceiver.ReceiveAsync(AppKey, query, body);
        var thrown = await failed.Should().ThrowAsync<WechatCallbackException>("错钥在解密入口即失败");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);

        var retried = async () => await rightKeyReceiver.ReceiveAsync(AppKey, query, body);
        await retried.Should().NotThrowAsync("解密失败不消耗指纹：同一报文换正确凭据重试可重新进入管线");
    }

    // ---------------------------------------------------------------- 多应用凭据（v1.2）

    [Fact]
    public async Task ReceiveAsync_ShouldResolveCredentials_WithExactKeyPriority()
    {
        // app1 有专属凭据、通配键凭据不同：精确键必须优先（多应用凭据选择，v1 方案 §5.3）。
        const string otherKey = "abcdefghijklmnopqrstuvwxyz0123456789abcdefg";
        var options = new WechatCallbackOptions
        {
            Apps =
            {
                ["app1"] = new WechatAppCallbackOptions { PushToken = "token-app1", PushEncodingAESKey = AesKey, ReceiveId = "ww-corp1" },
                [AppKey] = new WechatAppCallbackOptions { PushToken = "token-wild", PushEncodingAESKey = AesKey, ReceiveId = "ww-corp1" },
            },
        };
        var receiver = CreateReceiver(options);

        var plainXml = "<xml><InfoType>change_contact</InfoType><ChangeType>delete_party</ChangeType><Id>9</Id></xml>";
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, "ww-corp1");
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";
        var timestamp = Now();
        var nonce = NewNonce();
        var signature = WechatCallbackCrypto.ComputeSignature("token-app1", timestamp, nonce, encrypt);

        var act = async () => await receiver.ReceiveAsync("app1", $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}", body);
        await act.Should().NotThrowAsync("app1 命中专属凭据 token-app1，验签必须通过");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenAppUnknownAndNoWildcard()
    {
        var options = CreateOptions();
        options.Apps.Remove(AppKey);
        options.Apps["real-app"] = new WechatAppCallbackOptions { PushToken = Token, PushEncodingAESKey = AesKey };
        var receiver = CreateReceiver(options);
        var (encrypt, body) = EncryptBody("<xml><InfoType>suite_ticket</InfoType></xml>");

        var act = async () => await receiver.ReceiveAsync("unknown-app", QueryFor(encrypt, Now(), NewNonce()), body);
        var thrown = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*未命中应用*");
        thrown.Which.Kind.Should().Be(WechatCallbackFailureKind.UnknownReceiver);
    }

    // ---------------------------------------------------------------- 信封扩展 + D10（v1.2）

    [Fact]
    public async Task ReceiveAsync_ShouldParseChangeContactEnvelope_WithoutFakingAuthCorpId()
    {
        // D10：change_contact 的 FromUserName 固定为 sys，禁止兜底进 AuthCorpId。
        var receiver = CreateReceiver();
        var plainXml = "<xml><ToUserName><![CDATA[ww-corp]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
            "<CreateTime>1700000000</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
            "<Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_party]]></ChangeType>" +
            "<Id>2</Id><Name>rd</Name><ParentId>1</ParentId></xml>";
        var (encrypt, body) = EncryptBody(plainXml);

        var evt = await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsChangeContact.Should().BeTrue("Event = change_contact 信封判别");
        evt.ChangeType.Should().Be("create_party");
        evt.EventTypeKey.Should().Be("create_party");
        evt.ToUserName.Should().Be(CorpId);
        evt.FromUserName.Should().Be("sys");
        evt.MsgType.Should().Be("event");
        evt.CreateTime.Should().Be("1700000000");
        evt.AuthCorpId.Should().BeNull("D10：通讯录变更事件禁止 FromUserName 兜底伪造授权企业");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldFallbackAuthCorpId_OnlyForAuthFamily()
    {
        // D10：授权族（非 authcode 族）保留 FromUserName 兜底；authcode 族（R11）禁止。
        var receiver = CreateReceiver();
        var (encrypt, body) = EncryptBody(
            "<xml><InfoType>change_auth</InfoType><SuiteId>ww-suite</SuiteId>" +
            "<FromUserName><![CDATA[tencent]]></FromUserName></xml>");

        var evt = await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsChangeAuth.Should().BeTrue();
        evt.AuthCorpId.Should().Be("tencent", "授权族事件保留 FromUserName 兜底（既有语义）");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldParseBatchJobResultEnvelope()
    {
        var receiver = CreateReceiver();
        var plainXml = "<xml><ToUserName><![CDATA[ww-corp]]></ToUserName><FromUserName><![CDATA[zhangsan]]></FromUserName>" +
            "<CreateTime>1700000000</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
            "<Event><![CDATA[batch_job_result]]></Event><JobId><![CDATA[job-100]]></JobId>" +
            "<JobType><![CDATA[sync_user]]></JobType><ErrCode>0</ErrCode><ErrMsg>ok</ErrMsg></xml>";
        var (encrypt, body) = EncryptBody(plainXml);

        var evt = await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsBatchJobResult.Should().BeTrue();
        evt.EventTypeKey.Should().Be(WechatCallbackEventTypes.BatchJobResult);
        evt.AgentID.Should().BeNull("异步任务事件（通讯录域）不带 AgentID");
        evt.DecryptedXml.Should().Contain("job-100", "DecryptedXml 保留全量明文供业务解析");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldParseChangeChainEnvelope()
    {
        // 官方 95796：上下游变更事件（Event = change_chain），EventTypeKey 取 ChangeType（corp_join）。
        var receiver = CreateReceiver();
        var plainXml = "<xml><ToUserName><![CDATA[ww-corp]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
            "<CreateTime>1700000000</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
            "<Event><![CDATA[change_chain]]></Event><ChangeType><![CDATA[corp_join]]></ChangeType>" +
            "<ChainId><![CDATA[chain-xyz]]></ChainId>" +
            "<CorpIds><CorpId><![CDATA[ww-a1b2c3]]></CorpId></CorpIds></xml>";
        var (encrypt, body) = EncryptBody(plainXml);

        var evt = await receiver.ReceiveAsync(AppKey, QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsChangeChain.Should().BeTrue();
        evt.EventTypeKey.Should().Be(WechatCallbackEventTypes.CorpJoin, "上下游事件的匹配键 = ChangeType（D4）");
        evt.ChainId.Should().Be("chain-xyz", "信封解析 ChainId（95796）");
        evt.FromUserName.Should().Be("sys");
        evt.AuthCorpId.Should().BeNull("D10：上下游事件 FromUserName=sys 不得兜底进授权企业字段");
    }

    // ---------------------------------------------------------------- 事件分发（兜底处理器）

    [Fact]
    public async Task Handler_ShouldExposeFallbackContract()
    {
        // D6：内置授权族处理器以空键兜底注册；分发器未精确命中时才调用。
        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), new InMemoryWechatCorpAuthStore(),
            NullLogger<WechatCallbackHandler>.Instance);

        handler.SupportedEventType.Should().BeEmpty("D6：兜底处理器语义（SupportedEventType 空）");
        handler.Should().BeAssignableTo<IWechatCallbackEventHandler>();
    }

    [Fact]
    public async Task Handler_ShouldWriteSuiteTicketToStore()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "suite_ticket",
            SuiteId = "ww-suite",
            SuiteTicket = "ticket-1",
        });

        (await store.GetAsync("ww-suite")).Should().Be("ticket-1", "suite_ticket 事件应写入仓储并驱动 get_suite_token");
    }

    [Fact]
    public async Task Handler_ShouldNotDeleteAnyAuth_OnCancelAuth_WhenSuiteIdMissing()
    {
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = "suite-app",
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var appManager = CreateAppManager("suite-app", "ww-suite");
        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore,
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("suite-app", "corp-X")).Should().NotBeNull(
            "P0-3：SuiteId 缺失时无法定位归属应用，宁可不清理也不越权删除");
        appManager.Verify(
            m => m.InvalidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never, "未命中时不得级联失效任何应用的企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldRemoveCorpAuth_AndInvalidateCorpToken_OnCancelAuth()
    {
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = "suite-app",
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var appManager = CreateAppManager("suite-app", "ww-suite");
        appManager
            .Setup(m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore,
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            SuiteId = "ww-suite",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("suite-app", "corp-X")).Should().BeNull("取消授权后应清理永久授权码");
        appManager.Verify(
            m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()),
            Times.Once, "取消授权应级联失效该企业的授权企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldInvalidateCorpToken_OnChangeAuth()
    {
        var appManager = CreateAppManager("suite-app", "ww-suite");

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), new InMemoryWechatCorpAuthStore(),
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "change_auth",
            SuiteId = "ww-suite",
            AuthCorpId = "corp-Y",
        });

        appManager.Verify(
            m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-Y"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldIgnoreUnknownInfoType()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        var act = async () => await handler.HandleAsync(new WechatCallbackEvent { InfoType = "unknown_event" });
        await act.Should().NotThrowAsync("未知事件仅记录日志，不抛出");
    }

    [Fact]
    public async Task Handler_ShouldSkipEmptySuiteTicket()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "suite_ticket",
            SuiteId = "ww-suite",
        });

        (await store.GetAsync("ww-suite")).Should().BeNull("SuiteTicket 为空的事件不应覆盖仓储");
    }

    /// <summary>
    /// 构造仅暴露配置快照的 <see cref="IWechatAppManager"/>（P1-6 后处理器只读
    /// <c>ConfiguredConfigs</c>，不得依赖 <c>TryGetApp</c> 物化上下文）。
    /// </summary>
    private static Mock<IWechatAppManager> CreateAppManager(string appKey, string suiteId)
    {
        var config = new WechatAppConfig
        {
            AppKey = appKey,
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = suiteId,
            SuiteSecret = "suite-secret",
        };

        var appManager = new Mock<IWechatAppManager>();
        appManager.Setup(m => m.ConfiguredAppKeys).Returns(new[] { appKey });
        appManager.Setup(m => m.ConfiguredConfigs).Returns(new[] { config });

        WechatAppConfig? outConfig = config;
        appManager.Setup(m => m.TryGetConfig(appKey, out outConfig)).Returns(true);
        return appManager;
    }
}
