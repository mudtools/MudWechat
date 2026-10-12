// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>商品编辑要求（官方 <c>product_requirement</c> 对象，<c>getcategorydetail</c> 与 <c>getcategoryproductrule</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryProductRequirement
{
    /// <summary>获取或设置商品标题的编辑要求（官方 <c>product_title_requirement</c>）。</summary>
    [JsonPropertyName("product_title_requirement")]
    public string? ProductTitleRequirement { get; set; }

    /// <summary>获取或设置商品主图的编辑要求（官方 <c>product_img_requirement</c>）。</summary>
    [JsonPropertyName("product_img_requirement")]
    public string? ProductImgRequirement { get; set; }

    /// <summary>获取或设置商品描述的编辑要求（官方 <c>product_desc_requirement</c>）。</summary>
    [JsonPropertyName("product_desc_requirement")]
    public string? ProductDescRequirement { get; set; }
}
