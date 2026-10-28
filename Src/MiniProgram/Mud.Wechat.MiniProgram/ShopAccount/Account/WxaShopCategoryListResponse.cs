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
/// 获取商家类目列表应答（<c>POST /shop/account/get_category_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>只回「已申请成功」的类目</b>（官方页面正文「获取已申请成功的类类目列表」原文，含官方自身的重复字）：
/// 全量类目须另调 <c>POST /shop/cat/get</c>，两者的列表键名也不同（<c>data</c> vs <c>third_cat_list</c>）。</para>
/// <para>官方本页「请求参数说明」为<b>无</b>（请求体 <c>{}</c>）⇒ 端点不声明请求 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopCategoryListResponse : WxaResponse
{
    /// <summary>类目列表（<c>data</c>，object array）。</summary>
    [JsonPropertyName("data")]
    public List<WxaShopAccountCategory>? Data { get; set; }
}
