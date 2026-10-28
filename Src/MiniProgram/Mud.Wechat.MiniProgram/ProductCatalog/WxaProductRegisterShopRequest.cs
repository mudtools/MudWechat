// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 开通店铺请求体（<c>POST /product/register/register_shop</c>）。
/// </summary>
/// <remarks>
/// <para><c>id_card_name</c> 与 <c>id_card_number</c> 属敏感个人信息；<c>api_openstore_type</c> 目前仅 2（组件开放）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductRegisterShopRequest
{
    /// <summary>微信号（必填）。</summary>
    [JsonPropertyName("wx_name")]
    public string? WxName { get; set; }

    /// <summary>身份证姓名（敏感）。</summary>
    [JsonPropertyName("id_card_name")]
    public string? IdCardName { get; set; }

    /// <summary>身份证号（敏感）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>渠道号。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>开放类型（2 组件开放）。</summary>
    [JsonPropertyName("api_openstore_type")]
    public int? ApiOpenstoreType { get; set; }

    /// <summary>授权页 url。</summary>
    [JsonPropertyName("auth_page_url")]
    public string? AuthPageUrl { get; set; }
}
