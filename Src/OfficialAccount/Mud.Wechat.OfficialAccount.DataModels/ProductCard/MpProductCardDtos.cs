// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.ProductCard;

/// <summary>获取商品卡片的 DOM 结构（<c>channels/ec/service/product/getcardinfo</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：三字段均必填；「不同文章类型支持的卡片类型范围不同」——
/// 图片消息（newspic）支持小卡/文字链接/条卡；图文消息（news）支持大卡/小卡/文字链接（**不支持条卡**）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCard")]
public class MpProductCardInfoRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置文章类型（官方 <c>article_type</c>，必填：newspic / news）。</summary>
    [JsonPropertyName("article_type")]
    public string ArticleType { get; set; } = string.Empty;

    /// <summary>获取或设置卡片类型（官方 <c>card_type</c>，必填：0 大卡 / 1 小卡 / 2 文字链接 / 3 条卡；按 article_type 选型）。</summary>
    [JsonPropertyName("card_type")]
    public int CardType { get; set; }
}

/// <summary>获取商品卡片的 DOM 结构（<c>channels/ec/service/product/getcardinfo</c>）响应。</summary>
/// <remarks>
/// <para>
/// <b>端点位置特殊</b>：路径前缀为 <c>/channels/ec/</c>（视频号小店域），<b>非 /cgi-bin/</b>——
/// 官方把它归在「草稿管理和商品卡片」分组，照抄原文路径。不支持第三方平台调用。
/// </para>
/// <para>官方原文：「product_key 和 DOM 只会在需要该字段的文章类型及卡片类型的请求中返回」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCard")]
public class MpProductCardInfoResponse : MpResponse
{
    /// <summary>获取或设置商品 key（官方 <c>product_key</c>；部分文章类型插入商品卡片需要该 key）。</summary>
    [JsonPropertyName("product_key")]
    public string? ProductKey { get; set; }

    /// <summary>获取或设置商品卡 DOM 结构（官方 <c>DOM</c>；多数文章类型插入商品卡片需要 DOM 结构——属性名照官方大写形态）。</summary>
    [JsonPropertyName("DOM")]
    public string? Dom { get; set; }
}
