// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>商品二维码（<c>qrcode/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductQrcodeRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置二维码类型（官方 <c>qrcode_type</c>，缺省 1）：1 二维码，2 标准物料，3 送礼物物料。</summary>
    [JsonPropertyName("qrcode_type")]
    public int? QrcodeType { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}
