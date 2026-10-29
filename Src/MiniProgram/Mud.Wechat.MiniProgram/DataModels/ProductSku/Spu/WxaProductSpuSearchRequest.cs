// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>搜索商品请求（<c>POST /product/spu/search</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuSearchRequest
{
    /// <summary>商品状态，默认 5，取值见 <see cref="WxaProductSpuStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>商品来源（必填）：1 小商店自营商品；2 带货商品。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }

    /// <summary>第几页（必填，最小 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页数量（必填，不超过 10000）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>商品标题关键词（必填，模糊搜索）。</summary>
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }
}
