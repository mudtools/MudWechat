// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Callback.Tests;

/// <summary>
/// 结构化应答回写用例（CB-CH-10~12）：加密信封组装 / 解密往返 / 签名验证 / 应答 DTO 源生成序列化。
/// </summary>
public class WechatChannelsCallbackReplyTests
{
    private const string Token = "cb-reply-token";
    private const string AppId = "wxChannelsReplyAppId";

    private static readonly string EncodingAesKey =
        Convert.ToBase64String(new byte[32]).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>
    /// CB-CH-10：回写信封端到端往返 —— WriteReply 产出的 <c>{"Encrypt","MsgSignature","TimeStamp","Nonce"}</c>
    /// 经官方同款解密还原出明文 JSON，且 MsgSignature 四参验证通过、Nonce 回填 URL 参数值。
    /// </summary>
    [Fact]
    public void WriteReply_ShouldProduceVerifiableEncryptedEnvelope()
    {
        var app = new ChannelsAppCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = EncodingAesKey,
            AppId = AppId,
        };
        var plainJson = "{\"errcode\":0,\"errmsg\":\"ok\",\"info\":{\"is_vip\":true,\"identity\":\"Lv.1\"}}";
        var urlNonce = "url-nonce-42";

        var envelopeJson = WechatChannelsCallbackReplyWriter.WriteReply(
            app, ChannelsCallbackReply.Create(plainJson), urlNonce, DateTimeOffset.FromUnixTimeSeconds(1_700_000_000));

        using var document = JsonDocument.Parse(envelopeJson);
        var root = document.RootElement;
        root.TryGetProperty("Encrypt", out var encrypt).Should().BeTrue("官方回包信封必须含 Encrypt");
        root.TryGetProperty("MsgSignature", out var signature).Should().BeTrue("官方回包信封必须含 MsgSignature");
        root.TryGetProperty("TimeStamp", out var timeStamp).Should().BeTrue();
        root.TryGetProperty("Nonce", out var nonce).Should().BeTrue();

        // Nonce 回填 URL 参数中的 nonce（官方 dev_before/message_push 原文）。
        nonce.GetString().Should().Be(urlNonce);

        // MsgSignature 四参验证（token + TimeStamp + Nonce + Encrypt）。
        var encryptText = encrypt.GetString()!;
        var expected = WechatCallbackCrypto.ComputeSignature(
            Token, timeStamp.GetRawText(), nonce.GetString()!, encryptText);
        signature.GetString().Should().Be(expected, "回包签名必须可被官方按同款四参 SHA1 验证");

        // Encrypt 解密往返：还原明文 = 应答 JSON，尾部 receiveid = AppId。
        var decrypted = WechatCallbackCrypto.Decrypt(EncodingAesKey, encryptText, out var receiveId);
        decrypted.Should().Be(plainJson);
        receiveId.Should().Be(AppId);
    }

    /// <summary>CB-CH-11：结构化应答 DTO 经源生成上下文序列化（会员信息查询应答全字段绑定）。</summary>
    [Fact]
    public void VipUserInfoGetReply_ShouldSerializeViaSourceGenContext()
    {
        var reply = new ChannelsVipUserInfoGetReply
        {
            ErrCode = 0,
            ErrMsg = "ok",
            Info = new ChannelsVipUserInfoGetReplyInfo
            {
                IsVip = true,
                Identity = "Lv.1",
                OuterUserId = "outer-1",
            },
        };

        var json = JsonSerializer.Serialize(reply, CallbackJsonContext.Default.ChannelsVipUserInfoGetReply);

        json.Should().Contain("\"is_vip\":true", "官方字段名照抄（snake_case）");
        json.Should().Contain("\"identity\":\"Lv.1\"");
        json.Should().Contain("\"outer_userid\":\"outer-1\"");

        var roundTrip = JsonSerializer.Deserialize(json, CallbackJsonContext.Default.ChannelsVipUserInfoGetReply);
        roundTrip!.Info!.IsVip.Should().BeTrue();
    }

    /// <summary>CB-CH-12：下单确认应答 DTO（result 1 允许 / 2 拦截）常量与序列化锁定。</summary>
    [Fact]
    public void OrderUseVipDiscountReply_ShouldSerializeResult()
    {
        var reply = new ChannelsOrderUseVipDiscountReply
        {
            ErrCode = 0,
            ErrMsg = "ok",
            Result = ChannelsOrderUseVipDiscountResults.Allow,
        };

        var json = JsonSerializer.Serialize(reply, CallbackJsonContext.Default.ChannelsOrderUseVipDiscountReply);

        json.Should().Be("{\"errcode\":0,\"errmsg\":\"ok\",\"result\":1}",
            "官方返回示例逐字对齐（字段序 errcode/errmsg/result）");

        ChannelsOrderUseVipDiscountResults.Allow.Should().Be(1);
        ChannelsOrderUseVipDiscountResults.Deny.Should().Be(2);
    }
}
