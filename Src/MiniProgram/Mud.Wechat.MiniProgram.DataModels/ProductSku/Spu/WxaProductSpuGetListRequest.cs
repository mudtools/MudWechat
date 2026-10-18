// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>获取商品列表请求（<c>POST /product/spu/get_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuGetListRequest
{
    /// <summary>商品状态（必填），取值见 <see cref="WxaProductSpuStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>第几页（必填，最小 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页数量（必填，不超过 300）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>0 获取线上数据（默认）；1 获取草稿数据。</summary>
    [JsonPropertyName("need_edit_spu")]
    public int? NeedEditSpu { get; set; }

    /// <summary>商品来源（默认 1）：1 小商店自营；2 AMS；4 联盟；8 推荐官推荐；16 闪电购。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }
}
