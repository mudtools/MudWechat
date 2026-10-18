// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 小程序方曾提交过的类目资质记录（官方 <c>get_miniapp_certificate</c> 回包 <c>category_info_list()</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>键名为 <c>*_category_id</c> / <c>*_category_name</c> 全称形态</b>，且官方本页只回两级
/// （<c>first</c> / <c>second</c>，无 third）⇒ 与 <c>account/get_category_list</c> 的 <c>*_cat_*</c> 六支、
/// <c>cat/get</c> 的三级 <c>third_cat_*</c> 都不同，三套拼写各自照官方原文（守卫 SHOPA5）。</para>
/// <para>官方说明明写「小程序的一级类目 / 二级类目」⇒ 这里回显的是<b>小程序账号自身的类目</b>，
/// 不是交易组件申请的三级类目。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopMiniappCategoryInfo
{
    /// <summary>小程序一级类目 ID（<c>first_category_id</c>，官方类型列 uint32；示例 304「商家自营」）。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>小程序一级类目名（<c>first_category_name</c>，string）。</summary>
    [JsonPropertyName("first_category_name")]
    public string? FirstCategoryName { get; set; }

    /// <summary>小程序二级类目 ID（<c>second_category_id</c>，官方类型列 uint32；示例 321「食品」）。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>小程序二级类目名（<c>second_category_name</c>，string）。</summary>
    [JsonPropertyName("second_category_name")]
    public string? SecondCategoryName { get; set; }

    /// <summary>资质相关图片（<c>certificate_url</c>，官方类型列 stringArray，经微信侧转链）。</summary>
    [JsonPropertyName("certificate_url")]
    public List<string>? CertificateUrl { get; set; }
}
