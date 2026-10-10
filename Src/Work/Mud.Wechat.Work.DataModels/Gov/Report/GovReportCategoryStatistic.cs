// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 上报事件分类统计项（巡查上报 / 居民上报「获取上报事件分类统计」响应
/// <c>dashboard_list</c> 元素，政民沟通模块两族复用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovReportCategoryStatistic
{
    /// <summary>获取或设置分类 ID。</summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>获取或设置分类名称。</summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置分类等级（1 一级分组 / 2 二级分类）。</summary>
    [JsonPropertyName("category_level")]
    public int? CategoryLevel { get; set; }

    /// <summary>获取或设置累计上报数。</summary>
    [JsonPropertyName("total_case")]
    public int? TotalCase { get; set; }

    /// <summary>获取或设置累计办结数。</summary>
    [JsonPropertyName("total_solved")]
    public int? TotalSolved { get; set; }

    /// <summary>获取或设置分类类型（0 其他 / 1 有效分类）。</summary>
    [JsonPropertyName("category_type")]
    public int? CategoryType { get; set; }
}
