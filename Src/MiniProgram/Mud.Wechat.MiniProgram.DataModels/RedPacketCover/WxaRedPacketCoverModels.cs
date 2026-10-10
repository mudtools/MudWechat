// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.RedPacketCover;

/// <summary>获取微信红包封面请求体（<c>POST /redpacketcover/wxapp/cover_url/get_by_token</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>red-packet-cover/api_getredpacketcoverurl.html</c>。</para>
/// <para>
/// 本接口获得<b>指定用户</b>可领取的红包封面链接；<c>ctoken</c> 在微信红包封面开放平台获取
/// （官方原文：获取参数 ctoken 参考微信红包封面开放平台），属于<b>发放凭据</b>，
/// 禁止写入日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "RedPacketCover")]
public class WxaRedPacketCoverRequest
{
    /// <summary>可领取用户的 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>红包封面平台的发放 CToken（<c>ctoken</c>，必填；敏感凭据，禁止落日志）。</summary>
    [JsonPropertyName("ctoken")]
    public string? CToken { get; set; }
}

/// <summary>获取微信红包封面应答（<c>data.url</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "RedPacketCover")]
public class WxaRedPacketCoverResponse : WxaResponse
{
    /// <summary>返回数据（<c>data</c>），见 <see cref="WxaRedPacketCoverData"/>。</summary>
    [JsonPropertyName("data")]
    public WxaRedPacketCoverData? Data { get; set; }
}

/// <summary>微信红包封面返回数据（<c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "RedPacketCover")]
public class WxaRedPacketCoverData
{
    /// <summary>指定用户可领取的红包封面链接（<c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}