// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场类目筛选条件（官方 <c>category</c> 对象：类目 ID / 名称 与一二三级类目 ID 列表并存）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段集为「<c>category_id</c> + <c>category_name</c> + <c>category_ids_1/2/3</c>」，均可能为空，照录不裁剪。
/// <c>category_id</c> 官方类型 number、示例为超长值（如 <c>8261345894708739273</c>）⇒ 用 <see cref="long"/>；
/// <c>category_ids_N</c> 官方类型 <c>numarray</c>（数字数组）。
/// </para>
/// <para>本对象整体<b>定向计划（<c>plan_type = 1</c>）不支持</b>；SDK 不做本地拦截，越界由官方 <c>10024000</c> 表达。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierSelectionCategoryCondition
{
    /// <summary>获取或设置商品类目 ID（官方 <c>category_id</c>，number）。</summary>
    [JsonPropertyName("category_id")]
    public long? CategoryId { get; set; }

    /// <summary>获取或设置商品类目名称（官方 <c>category_name</c>，可填空）。</summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置一级类目列表（官方 <c>category_ids_1</c>，numarray）。</summary>
    [JsonPropertyName("category_ids_1")]
    public List<long>? CategoryIds1 { get; set; }

    /// <summary>获取或设置二级类目列表（官方 <c>category_ids_2</c>，numarray）。</summary>
    [JsonPropertyName("category_ids_2")]
    public List<long>? CategoryIds2 { get; set; }

    /// <summary>获取或设置三级类目列表（官方 <c>category_ids_3</c>，numarray）。</summary>
    [JsonPropertyName("category_ids_3")]
    public List<long>? CategoryIds3 { get; set; }
}
