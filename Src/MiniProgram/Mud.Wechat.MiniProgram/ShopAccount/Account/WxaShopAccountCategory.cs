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
/// 商家已申请成功的三级类目条目（官方 <c>get_category_list</c> 回包 <c>data[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>键名口径为 <c>cat</c> 缩写</b>（<c>first_cat_id</c> / <c>second_cat_id</c> / <c>third_cat_id</c> 与
/// <c>*_cat_name</c>），与 <c>cat/get</c>（<c>first_cat_id</c> 同）、<c>audit_category</c>
/// （<c>level1</c> / <c>level2</c> / <c>level3</c>）、<c>get_miniapp_certificate</c>
/// （<c>first_category_id</c> / <c>second_category_id</c>）<b>三套拼写并存</b> ⇒ 不得互相抄写，守卫 SHOPA5 逐条锁定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccountCategory
{
    /// <summary>一级类目 ID（<c>first_cat_id</c>，number；官方示例 6870）。</summary>
    [JsonPropertyName("first_cat_id")]
    public long? FirstCatId { get; set; }

    /// <summary>一级类目名称（<c>first_cat_name</c>，string）。</summary>
    [JsonPropertyName("first_cat_name")]
    public string? FirstCatName { get; set; }

    /// <summary>二级类目 ID（<c>second_cat_id</c>，number）。</summary>
    [JsonPropertyName("second_cat_id")]
    public long? SecondCatId { get; set; }

    /// <summary>二级类目名称（<c>second_cat_name</c>，string）。</summary>
    [JsonPropertyName("second_cat_name")]
    public string? SecondCatName { get; set; }

    /// <summary>三级类目 ID（<c>third_cat_id</c>，number；提审类目资质与新增商品用的就是本值）。</summary>
    [JsonPropertyName("third_cat_id")]
    public long? ThirdCatId { get; set; }

    /// <summary>三级类目名称（<c>third_cat_name</c>，string；官方示例「隔离霜/妆前乳」）。</summary>
    [JsonPropertyName("third_cat_name")]
    public string? ThirdCatName { get; set; }
}
