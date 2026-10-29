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
/// 商家已申请成功的品牌条目（官方 <c>get_brand_list</c> 回包 <c>data[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para>字段名照官方原文：<c>brand_id</c> / <c>brand_wording</c>（<b>不是</b> <c>brand_name</c>，
/// 官方用「品牌词」一词，守卫 SHOPA5 锁定）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopBrand
{
    /// <summary>品牌 ID（<c>brand_id</c>，number；官方示例值 2101，品牌审核成功后由回调给出）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>品牌名称（<c>brand_wording</c>，string；官方示例「悦诗风吟」）。</summary>
    [JsonPropertyName("brand_wording")]
    public string? BrandWording { get; set; }
}
