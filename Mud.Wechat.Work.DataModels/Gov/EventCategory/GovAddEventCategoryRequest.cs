// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 添加事件类别请求体（<c>/cgi-bin/report/grid/add_cata</c>，政民沟通配置事件类别域）。
/// <para>
/// 可以添加一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。
/// 官方业务限制：分类名称不能超过 30 个字，同一一级分类下的二级分类名字不能一样；
/// 分类层级只能传 1 或者 2，level 为 2 时 parent_category_id 必传。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovAddEventCategoryRequest
{
    /// <summary>
    /// 获取或设置分类名称（官方必填）。不能超过 30 个字，同一一级分类下的二级分类名字不能一样。
    /// </summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置分类层级（官方必填）。这里只能传 1 或者 2。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置所属的一级分类的 id。level 为 2 的话必传。</summary>
    [JsonPropertyName("parent_category_id")]
    public string? ParentCategoryId { get; set; }
}
