// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 获取居民上报事件分类统计请求体（<c>/cgi-bin/report/resident/category_statistic</c>，政民沟通居民上报族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovResidentGetCategoryStatisticRequest
{
    /// <summary>
    /// 获取或设置分类 ID。不传此字段，能拉取到所有一级分类的数据；
    /// 传一级分类的 category_id 能拉取到该一级分类下的所有二级分类的数据。
    /// </summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }
}
