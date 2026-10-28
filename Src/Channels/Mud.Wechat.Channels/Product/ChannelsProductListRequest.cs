// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 获取商品列表（<c>product/list/get</c> / <c>product/gift/list/get</c>）请求体（两端点字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductListRequest
{
    /// <summary>
    /// 获取或设置商品状态（官方 <c>status</c>，不填默认拉全部商品（不包含从未上架的草稿和回收站商品）；
    /// gift/list 语义：0 初始值，5 上架，6 回收站（仅 status==6 才返回回收站），11 所有下架商品）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填：默认 10，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，由上次请求返回；不传默认获取第一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
