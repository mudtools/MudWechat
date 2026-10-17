// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>商品类目查询条件（官方 <c>category</c> 对象，<c>promoter/get_promote_product_list</c> 请求体；定向计划不支持）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：<c>category_id</c> 参数表标注 number，但官方类目示例值（如 11099070301657575487）超出 <c>Int64</c> 上限、请求示例又写成字符串 ⇒ 按 string 建模；<c>category_ids_1/2/3</c> 参数表类型列为「-」，官方两处示例一为数字数组、一为字符串数组 ⇒ 按字符串数组建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterProductCategoryCondition
{
    /// <summary>获取或设置商品类目（官方 <c>category_id</c>）。</summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>获取或设置商品类目名称（官方 <c>category_name</c>，可填空）。</summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置一级类目列表（官方 <c>category_ids_1</c>）。</summary>
    [JsonPropertyName("category_ids_1")]
    public List<string>? CategoryIds1 { get; set; }

    /// <summary>获取或设置二级类目列表（官方 <c>category_ids_2</c>）。</summary>
    [JsonPropertyName("category_ids_2")]
    public List<string>? CategoryIds2 { get; set; }

    /// <summary>获取或设置三级类目列表（官方 <c>category_ids_3</c>）。</summary>
    [JsonPropertyName("category_ids_3")]
    public List<string>? CategoryIds3 { get; set; }
}
