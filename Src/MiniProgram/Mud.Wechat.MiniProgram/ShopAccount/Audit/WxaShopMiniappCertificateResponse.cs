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
/// 获取小程序资质应答（<c>POST /shop/audit/get_miniapp_certificate</c>）。
/// </summary>
/// <remarks>
/// <para><b>无 <c>data</c> 包装</b>：官方两个回包示例均为顶层直给 <c>brand_info</c> / <c>category_info_list</c>，
/// 与本页其余 <c>data</c> 包装的端点形态不同 ⇒ 照官方建模（守卫 SHOPA5）。</para>
/// <para><b>官方原文三条陷阱</b>：① 「请求类目会返回<b>多次</b>的请求记录，请求品牌<b>只返回最后一次</b>的提交记录」；
/// ② 「图片经过转链，请使用高版本 chrome 浏览器打开」；③ 「如果曾经没有提交、没有储存历史文件，或是获取失败，
/// 接口会返回 <c>1050006</c>」——③ 意味着本端点「空历史」不是空数组而是错误码，宿主须按码分支。</para>
/// <para><b>注（官方原文）</b>：「该接口返回的是曾经在<b>小程序方</b>提交过的审核，非组件的入驻审核！」
/// ⇒ 与 <c>audit_brand</c> / <c>audit_category</c> 提审链路是两套记录，不可互相回读。</para>
/// <para><b>数组键可能缺省也可能出现</b>：官方示例注释「因为 <c>category_info_list</c> 为数组，因此即使为空，也可能出现在回包中」
/// ⇒ 可空集合，不得假定键必在。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopMiniappCertificateResponse : WxaResponse
{
    /// <summary>品牌资质（<c>brand_info</c>，object；<c>req_type = 2</c> 时给出，且只含最后一次提交）。</summary>
    [JsonPropertyName("brand_info")]
    public WxaShopMiniappBrandInfo? BrandInfo { get; set; }

    /// <summary>类目资质提交记录列表（<c>category_info_list</c>，array；<c>req_type = 1</c> 时给出，含多次记录）。</summary>
    [JsonPropertyName("category_info_list")]
    public List<WxaShopMiniappCategoryInfo>? CategoryInfoList { get; set; }
}
