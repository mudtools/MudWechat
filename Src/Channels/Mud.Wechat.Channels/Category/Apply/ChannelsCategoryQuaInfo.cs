// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>资质条目（官方 <c>product_qua</c> / <c>brand_qua</c> 对象与 <c>product_qua_list</c> 数组元素共用形态——
/// 官方 <c>product_qua</c>/<c>brand_qua</c> 未携带 <c>name</c> 字段，可空超集覆盖）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryQuaInfo
{
    /// <summary>获取或设置资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置该类目是否需要申请才能用（官方 <c>need_to_apply</c>）。</summary>
    [JsonPropertyName("need_to_apply")]
    public bool? NeedToApply { get; set; }

    /// <summary>获取或设置资质的描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置申请此类目时是否必须提交资质证明（官方 <c>mandatory</c>）。</summary>
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }

    /// <summary>获取或设置资质名称（官方 <c>name</c>，仅 <c>product_qua_list</c> 元素携带）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
