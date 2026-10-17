using FluentAssertions;
using Mud.Wechat.Abstractions.Contracts;
using Mud.Wechat.AI.Agent.Results;
using Xunit;

namespace Mud.Wechat.AI.Tests.Results;

/// <summary>
/// 层 1 统一信封读取器：成功投影 / 失败回填 / 空载荷 fail-closed / 码表不猜语义。
/// </summary>
public class WechatApiResultReaderTests
{
    [Fact]
    public void Read_Envelope_WhenSuccess_ShouldProjectResponseAsData()
    {
        var response = new FakeResponse(errorCode: 0, errorMessage: null, payload: "pong");

        var outcome = WechatApiResultReader.Read(response);

        outcome.Ok.Should().BeTrue();
        outcome.Data.Should().BeSameAs(response);
        outcome.Code.Should().BeNull();
        outcome.ErrorText.Should().BeNull();
    }

    [Fact]
    public void Read_Envelope_WhenFailed_ShouldCarryCodeAndReadableText()
    {
        var response = new FakeResponse(errorCode: 40013, errorMessage: "invalid corpid", payload: null);

        var outcome = WechatApiResultReader.Read(response);

        outcome.Ok.Should().BeFalse();
        outcome.Data.Should().BeNull();
        outcome.Code.Should().Be(40013);
        outcome.ErrorText.Should().Be("errcode=40013, errmsg=invalid corpid");
        // 码级读数留空 ⇒ 中立层回落 api/upstream_error（不猜语义）。
        outcome.Failure.Should().BeNull();
    }

    [Fact]
    public void Read_EnvelopeAndPayload_WhenSuccess_ShouldReturnPayload()
    {
        var envelope = new FakeResponse(errorCode: 0, errorMessage: null, payload: null);

        var outcome = WechatApiResultReader.Read(envelope, new Payload("token-1"));

        outcome.Ok.Should().BeTrue();
        outcome.Data!.Value.Should().Be("token-1");
    }

    [Fact]
    public void Read_EnvelopeAndPayload_WhenEnvelopeFails_ShouldNotLeakPayload()
    {
        var envelope = new FakeResponse(errorCode: 45033, errorMessage: "api freq out of limit", payload: null);

        var outcome = WechatApiResultReader.Read(envelope, new Payload("token-1"));

        outcome.Ok.Should().BeFalse();
        outcome.Data.Should().BeNull();
        outcome.Code.Should().Be(45033);
    }

    [Fact]
    public void Read_EnvelopeAndPayload_WhenSuccessButPayloadMissing_ShouldFailClosed()
    {
        var envelope = new FakeResponse(errorCode: 0, errorMessage: null, payload: null);

        var outcome = WechatApiResultReader.Read<Payload>(envelope, null);

        outcome.Ok.Should().BeFalse();
        outcome.ErrorText.Should().Contain("empty_payload");
    }

    [Fact]
    public void Read_ShouldRejectNullArguments()
    {
        var readNullResponse = () => WechatApiResultReader.Read<FakeResponse>(null!);
        var readNullEnvelope = () => WechatApiResultReader.Read<Payload>(null!, new Payload("x"));

        readNullResponse.Should().Throw<ArgumentNullException>();
        readNullEnvelope.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Read_WhenErrorMessageMissing_ShouldUseNeutralDescription()
    {
        var response = new FakeResponse(errorCode: 50001, errorMessage: "  ", payload: null);

        var outcome = WechatApiResultReader.Read(response);

        outcome.ErrorText.Should().Be("errcode=50001, errmsg=未提供错误信息");
    }

    private sealed class Payload
    {
        public Payload(string value) => Value = value;

        public string Value { get; }
    }

    private sealed class FakeResponse : IWechatApiResponse
    {
        private readonly object? _payload;

        public FakeResponse(int errorCode, string? errorMessage, object? payload)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            _payload = payload;
        }

        public int ErrorCode { get; }

        public string? ErrorMessage { get; }

        public bool IsSuccess => ErrorCode == 0;

        public override string ToString() => _payload?.ToString() ?? string.Empty;
    }
}
