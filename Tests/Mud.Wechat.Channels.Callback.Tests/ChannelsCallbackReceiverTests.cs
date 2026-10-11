// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Callback.Tests;

/// <summary>
/// 小店回调接收器行为用例（CB-CH 系列）：URL 验证 / 验签 / 时效 / AppId 校验 / 指纹重放 /
/// 事件键解析 —— 全链以<b>真实 AES 加解密 fixture</b> 端到端驱动（Core 密码学同源）。
/// </summary>
public class WechatChannelsCallbackReceiverTests
{
    private const string Token = "cb-test-token";
    private const string AppId = "wxChannelsCbTestAppId";
    private const long NowSeconds = 1_700_000_000;

    private static readonly string EncodingAesKey =
        Convert.ToBase64String(new byte[32]).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>CB-CH-1：GET URL 验证 —— 验签通过返回 echostr 原文（不消费指纹）。</summary>
    [Fact]
    public async Task EchoAsync_ShouldReturnEchostr_WhenSignatureValid()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var query = BuildEchoQuery(Token, "echo-str-value");

        var echoed = await receiver.EchoAsync("shop-main", query);

        echoed.Should().Be("echo-str-value");
    }

    /// <summary>CB-CH-2：GET 验签失败（token 不符）抛 InvalidSignature（中间件映射 403）。</summary>
    [Fact]
    public async Task EchoAsync_ShouldReject_WhenSignatureMismatched()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var query = BuildEchoQuery("wrong-token", "echo-str-value");

        var act = () => receiver.EchoAsync("shop-main", query);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>CB-CH-3：POST 接收全链（真实密文）—— 验签/解密/AppId 校验通过后解析事件键。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldParseEvent_WhenFullChainValid()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var plain = BuildPlainJson("channels_ec_order_pay", "\"order_info\":{\"order_id\":3705115058471209000,\"pay_time\":1700000001}");
        var (query, body) = BuildEncryptedPost(Token, plain, AppId, nowSeconds: NowSeconds);

        var envelope = await receiver.ReceiveAsync("shop-main", query, body);

        envelope.EventTypeKey.Should().Be("channels_ec_order_pay");
        envelope.MsgType.Should().Be("event");
        envelope.AppKey.Should().Be("shop-main");
        envelope.DecryptedXml.Should().Contain("order_info");
        envelope.ToUserName.Should().NotBeNullOrEmpty();
    }

    /// <summary>CB-CH-4：明文 appid 与回调配置 AppId 不一致即拒绝（fail-closed，官方 appid 自校验语义）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenAppIdMismatched()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var plain = BuildPlainJson("channels_ec_order_pay", "\"order_info\":{\"order_id\":1,\"pay_time\":2}");
        var (query, body) = BuildEncryptedPost(Token, plain, "wxOtherAppId", nowSeconds: NowSeconds);

        var act = () => receiver.ReceiveAsync("shop-main", query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    /// <summary>CB-CH-5：timestamp 超出时效窗口（±300s）即拒（fail-closed）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampOutOfRange()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var plain = BuildPlainJson("channels_ec_order_pay", "\"order_info\":{\"order_id\":1,\"pay_time\":2}");
        var (query, body) = BuildEncryptedPost(Token, plain, AppId, nowSeconds: NowSeconds - 301);

        var act = () => receiver.ReceiveAsync("shop-main", query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    /// <summary>
    /// CB-CH-6：同密文二次接收被指纹闸拒绝（fail-closed 优先于 at-least-once，处理器须幂等）。
    /// </summary>
    [Fact]
    public async Task ReceiveAsync_ShouldRejectReplay_WhenSameFingerprintResubmitted()
    {
        var receiver = CreateReceiver(fixedNow: NowSeconds);
        var plain = BuildPlainJson("channels_ec_order_pay", "\"order_info\":{\"order_id\":1,\"pay_time\":2}");
        var (query, body) = BuildEncryptedPost(Token, plain, AppId, nowSeconds: NowSeconds);

        await receiver.ReceiveAsync("shop-main", query, body);
        var act = () => receiver.ReceiveAsync("shop-main", query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected);
    }

    /// <summary>
    /// CB-CH-7：契约登记闭环 —— <c>ChannelsPayloadContracts.RegisterAll</c> 注册键集必须与
    /// <c>ChannelsCallbackEventTypes</c> 常量全集一致（防「常量已声明但契约未登记」的静默盲区）。
    /// </summary>
    [Fact]
    public void PayloadContracts_ShouldCoverAllOfficialEventTypes()
    {
        var registry = new ChannelsPayloadContractRegistry();
        ChannelsPayloadContracts.RegisterAll(registry);

        var constants = typeof(ChannelsCallbackEventTypes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToHashSet(StringComparer.Ordinal);

        constants.Should().NotBeEmpty("事件键常量类不得为空（42 个官方回调事件）");
        registry.RegisteredKeys.Should().HaveCount(constants.Count,
            "契约登记键数必须与官方事件键常量数一致（多登记 / 漏登记都是漂移）");
        registry.RegisteredKeys.Should().OnlyContain(k => constants.Contains(k),
            "登记键必须全部来自官方事件键常量（手写字面量是禁止形态）");
    }

    /// <summary>CB-CH-8：类型化载荷反序列化（net8+）—— 按事件键从契约取出载荷类型并绑定强类型。</summary>
    [Fact]
    public void Contract_ShouldBindOrderPayPayloadType()
    {
        var registry = new ChannelsPayloadContractRegistry();
        ChannelsPayloadContracts.RegisterAll(registry);

        registry.TryResolve(ChannelsCallbackEventTypes.OrderPay, out var contract)
            .Should().BeTrue("订单支付事件键必须已登记契约");
        contract!.PayloadType.Should().Be(typeof(ChannelsOrderPayPayload));

        var json = "{\"ToUserName\":\"gh_x\",\"FromUserName\":\"o_x\",\"CreateTime\":1700000000," +
                   "\"MsgType\":\"event\",\"Event\":\"channels_ec_order_pay\"," +
                   "\"order_info\":{\"order_id\":3705115058471209000,\"pay_time\":1700000001}}";
        var typeInfo = Mud.Wechat.Channels.DataModels.Callback.CallbackJsonContext.Default
            .GetTypeInfo(contract.PayloadType);
        var payload = JsonSerializer.Deserialize(json, typeInfo);

        payload.Should().BeOfType<ChannelsOrderPayPayload>();
        var typed = (ChannelsOrderPayPayload)payload!;
        typed.OrderInfo!.OrderId.Should().Be(3705115058471209000);
        typed.OrderInfo!.PayTime.Should().Be(1700000001);
    }

    /// <summary>CB-CH-9：回调配置校验 —— 缺 AppId / AESKey 非 43 位 / 软超时 ≥ 5000ms 均启动期失败。</summary>
    [Fact]
    public void OptionsValidate_ShouldFailFast_OnInvalidCredential()
    {
        var options = new ChannelsCallbackOptions();
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*Apps*");

        options.Apps["shop-main"] = new ChannelsAppCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = "short-key",
            AppId = AppId,
        };
        options.Apps["shop-main"].PushEncodingAESKey = EncodingAesKey[..42];
        var act2 = () => options.Validate();
        act2.Should().Throw<InvalidOperationException>().WithMessage("*43*");

        options.Apps["shop-main"].PushEncodingAESKey = EncodingAesKey;
        options.EventHandlingTimeoutMs = 5_000;
        var act3 = () => options.Validate();
        act3.Should().Throw<InvalidOperationException>().WithMessage("*5 秒*");
    }

    // ---- fixtures ------------------------------------------------------------

    private static WechatChannelsCallbackReceiver CreateReceiver(double? fixedNow = null)
    {
        var options = new ChannelsCallbackOptions
        {
            EventHandlingTimeoutMs = 4_000,
        };
        options.Apps["shop-main"] = new ChannelsAppCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = EncodingAesKey,
            AppId = AppId,
        };

        return new WechatChannelsCallbackReceiver(
            new StaticOptionsMonitor(options),
            replayGuard: new InMemoryWechatCallbackReplayGuard(),
            utcNow: fixedNow is null ? null : () => DateTimeOffset.FromUnixTimeSeconds((long)fixedNow));
    }

    private static string BuildPlainJson(string @event, string extraFields)
        => "{\"ToUserName\":\"gh_shop\",\"FromUserName\":\"o_user\",\"CreateTime\":" + NowSeconds +
           ",\"MsgType\":\"event\",\"Event\":\"" + @event + "\"," + extraFields + "}";

    private static string BuildEchoQuery(string token, string echostr)
    {
        var timestamp = NowSeconds.ToString();
        var nonce = "nonce-1";
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce);
        return $"signature={signature}&timestamp={timestamp}&nonce={nonce}&echostr={echostr}";
    }

    private static (string Query, string Body) BuildEncryptedPost(
        string token, string plain, string receiveId, long nowSeconds)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(EncodingAesKey, plain, receiveId);
        var timestamp = nowSeconds.ToString();
        var nonce = "nonce-1";
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        var body = JsonSerializer.Serialize(new EncryptedEnvelope(encrypt));
        return ($"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}", body);
    }

    private sealed record EncryptedEnvelope(string Encrypt);

    /// <summary>测试用静态 <see cref="IOptionsMonitor{TOptions}"/>（无热更语义；回调测试不需要 ChangeToken）。</summary>
    private sealed class StaticOptionsMonitor : IOptionsMonitor<ChannelsCallbackOptions>
    {
        private readonly ChannelsCallbackOptions _current;

        public StaticOptionsMonitor(ChannelsCallbackOptions current)
        {
            _current = current;
        }

        public ChannelsCallbackOptions CurrentValue => _current;

        public ChannelsCallbackOptions Get(string? name) => _current;

        public IDisposable? OnChange(Action<ChannelsCallbackOptions, string?> listener) => null;
    }

}
