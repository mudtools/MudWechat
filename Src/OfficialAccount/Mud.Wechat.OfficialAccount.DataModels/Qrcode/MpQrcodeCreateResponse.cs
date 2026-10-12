// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Qrcode;

/// <summary>生成带参数的二维码（<c>qrcode/create</c>）响应。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：成功响应不含 errcode（缺省 0）；
/// <c>expire_seconds</c> 最大不超过 2592000（即 30 天）；<c>url</c> 为二维码图片解析后的地址，
/// 开发者可根据该地址自行生成需要的二维码图片。
/// </para>
/// <para>
/// <b>换图端点（官方「通过 ticket 换取二维码」原文，SDK 不代下载）</b>：
/// 「获取二维码 ticket 后，开发者可用 ticket 换取二维码图片。请注意，本接口无须登录态即可调用」——
/// <c>GET https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=TICKET</c>（TICKET 须 UrlEncode；
/// ticket 正确时 HTTP 200 返回图片，错误情况下返回 HTTP 404）。该端点域名在
/// mp.weixin.qq.com（非 api.weixin.qq.com），SDK 暂不建模（归下载通道的待评估项）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qrcode")]
public class MpQrcodeCreateResponse : MpResponse
{
    /// <summary>获取或设置二维码 ticket（官方 <c>ticket</c>；凭此在有效时间内换取二维码）。</summary>
    [JsonPropertyName("ticket")]
    public string? Ticket { get; set; }

    /// <summary>获取或设置该二维码有效时间（官方 <c>expire_seconds</c>，秒；最大 2592000）。</summary>
    [JsonPropertyName("expire_seconds")]
    public int? ExpireSeconds { get; set; }

    /// <summary>获取或设置二维码图片解析后的地址（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
