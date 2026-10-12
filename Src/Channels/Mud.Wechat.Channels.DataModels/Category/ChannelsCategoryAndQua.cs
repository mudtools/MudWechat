// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>类目与资质信息（官方 <c>cats</c> / <c>cats_v2</c> 数组元素，结构一致）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAndQua
{
    /// <summary>获取或设置类目详情（官方 <c>cat</c>；<c>cats_v2</c> 版本额外携带 <c>leaf</c>）。</summary>
    [JsonPropertyName("cat")]
    public ChannelsCategoryCat? Cat { get; set; }

    /// <summary>获取或设置类目资质详情（官方 <c>qua</c>；<c>cert_group_list</c> 仅 <c>cats</c> 旧树携带）。</summary>
    [JsonPropertyName("qua")]
    public ChannelsCategoryQua? Qua { get; set; }

    /// <summary>获取或设置商品资质信息（官方 <c>product_qua</c>，官方已注明「将废弃，使用 product_qua_list 代替」）。</summary>
    [JsonPropertyName("product_qua")]
    public ChannelsCategoryQuaInfo? ProductQua { get; set; }

    /// <summary>获取或设置品牌资质信息（官方 <c>brand_qua</c>）。</summary>
    [JsonPropertyName("brand_qua")]
    public ChannelsCategoryQuaInfo? BrandQua { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_list</c>，替代 <c>product_qua</c>）。</summary>
    [JsonPropertyName("product_qua_list")]
    public List<ChannelsCategoryQuaInfo>? ProductQuaList { get; set; }

    /// <summary>获取或设置是否放心买必须打开坏损包赔（官方 <c>is_confidence_require_bad_must_pay</c>）。</summary>
    [JsonPropertyName("is_confidence_require_bad_must_pay")]
    public bool? IsConfidenceRequireBadMustPay { get; set; }
}
