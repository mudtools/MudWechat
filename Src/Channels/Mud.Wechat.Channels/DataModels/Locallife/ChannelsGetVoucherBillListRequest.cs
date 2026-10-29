// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>查询券账单（<c>getvoucherbilllist</c>）请求体。</summary>
/// <remarks>官方契约：页大小 <c>1 ≤ page_size ≤ 100</c>；<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherBillListRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填；通过本地生活 api 创建的商品 id）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置账单日期（官方 <c>bill_date</c>，必填；券发起核销时间，官方示例形态 <c>2023-12-03</c>）。</summary>
    [JsonPropertyName("bill_date")]
    public string BillDate { get; set; } = string.Empty;

    /// <summary>获取或设置页大小（官方 <c>page_size</c>，必填，1 ≤ value ≤ 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求值为上次请求的返回）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}
