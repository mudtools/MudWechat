// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 获取事件类别列表响应体（<c>/cgi-bin/report/grid/list_cata</c>，政民沟通配置事件类别域）。
/// <para>
/// 可以获取已配置的一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。
/// </para>
/// <para>
/// 官方文档自相矛盾：响应 JSON 示例的列表字段为 <c>category_list</c>，
/// 参数说明表则写作 <c>cata_list</c>；照抄示例取 <c>category_list</c>
/// （对齐「升级服务专员部门列表以官方 JSON 示例为准」的既存处置先例）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovGetEventCategoryListResponse : WechatWorkResponse
{
    /// <summary>获取或设置分类列表（官方 JSON 示例字段 category_list；参数表误写为 cata_list，以示例为准）。</summary>
    [JsonPropertyName("category_list")]
    public List<GovEventCategoryInfo>? CategoryList { get; set; }
}
