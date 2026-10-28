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
/// 获取商家品牌列表应答（<c>POST /shop/account/get_brand_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表键名是 <c>data</c>（数组），不是 <c>data.list</c></b>：本页与
/// <c>get_category_list</c> 同形态，而 <c>account/get_info</c> 与 <c>register/check</c>、
/// <c>audit/result</c> 的 <c>data</c> 是<b>对象</b> ⇒ 同一条回包键在本域内既可以是数组也可以是对象，
/// 逐端点各自建模，守卫 SHOPA5 锁定。</para>
/// <para>官方本页「请求参数说明」为<b>无</b>（请求体 <c>{}</c>）⇒ 端点不声明请求 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopBrandListResponse : WxaResponse
{
    /// <summary>品牌列表（<c>data</c>，object array；仅含<b>审核已成功</b>的品牌）。</summary>
    [JsonPropertyName("data")]
    public List<WxaShopBrand>? Data { get; set; }
}
