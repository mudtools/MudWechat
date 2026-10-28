// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>获取商品列表请求（<c>POST /shop/spu/get_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ShopSpu")]
public class WxaShopSpuGetListRequest
{
    /// <summary>商品状态（参数表标必填；官方示例注释为「选填，不填时获取所有状态商品」⇒ 按示例可空建模并留档）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>开始创建时间，与 <c>end_create_time</c> 成对。</summary>
    [JsonPropertyName("start_create_time")]
    public string? StartCreateTime { get; set; }

    /// <summary>结束创建时间，与 <c>start_create_time</c> 成对。</summary>
    [JsonPropertyName("end_create_time")]
    public string? EndCreateTime { get; set; }

    /// <summary>开始更新时间，与 <c>end_update_time</c> 成对。</summary>
    [JsonPropertyName("start_update_time")]
    public string? StartUpdateTime { get; set; }

    /// <summary>结束更新时间，与 <c>start_update_time</c> 成对。</summary>
    [JsonPropertyName("end_update_time")]
    public string? EndUpdateTime { get; set; }

    /// <summary>0 获取线上数据（默认）；1 获取草稿数据。</summary>
    [JsonPropertyName("need_edit_spu")]
    public int? NeedEditSpu { get; set; }

    /// <summary>页号（必填）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>页面大小（必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}
