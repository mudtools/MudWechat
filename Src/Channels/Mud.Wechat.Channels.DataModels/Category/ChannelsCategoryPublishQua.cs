// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>发品资质项（官方 <c>product_qua_list</c> 数组元素，<c>getcategoryproductrule</c> 形态——
/// 与 <c>getallcategory</c>/<c>getcategorydetail</c> 的资质形态不同（本形态无 <c>qua_id</c>/<c>need_to_apply</c>/<c>mandatory</c>、
/// 以 <c>id</c> 为标识且多 <c>required_rule</c>）⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPublishQua
{
    /// <summary>获取或设置资质 ID（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置资质描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置资质名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置填写规则（官方 <c>required_rule</c>）。</summary>
    [JsonPropertyName("required_rule")]
    public ChannelsCategoryRequiredRule? RequiredRule { get; set; }
}
