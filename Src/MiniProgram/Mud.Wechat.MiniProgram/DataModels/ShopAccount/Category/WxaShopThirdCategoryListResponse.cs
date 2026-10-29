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
/// 获取全量三级类目应答（<c>POST /shop/cat/get</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表键是 <c>third_cat_list</c>（顶层），不是 <c>data</c></b>：与本域另两支列表端点
/// （<c>get_brand_list</c> / <c>get_category_list</c> 用 <c>data</c>）不同形 ⇒ 守卫 SHOPA5 锁定。</para>
/// <para><b>体量与频率陷阱（官方原文，本域唯一的频率性表述）</b>：「该接口拉到的是【全量】三级类目数据，
/// 数据回包大小约为 <b>2MB</b>。所以请商家自己做好缓存，不要频繁调用（<b>有严格的频率限制</b>），
/// 该类目数据不会频率变动，推荐商家<b>每天调用一次</b>更新商家自身缓存」
/// ——官方给的是「严格限频 + 每日一刷」的建议，<b>没有</b>「次/秒」「次/天」这类数字额度 ⇒ 不得凭空补数。</para>
/// <para>官方本页「请求参数说明」为<b>无</b>（请求体 <c>{}</c>）⇒ 端点不声明请求 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopThirdCategoryListResponse : WxaResponse
{
    /// <summary>类目列表（<c>third_cat_list</c>，object array；全量三级类目及其资质信息）。</summary>
    [JsonPropertyName("third_cat_list")]
    public List<WxaShopThirdCategory>? ThirdCatList { get; set; }
}
