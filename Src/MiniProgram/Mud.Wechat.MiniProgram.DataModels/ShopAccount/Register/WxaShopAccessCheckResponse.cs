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
/// 获取接入状态应答（<c>POST /shop/register/check</c>）。
/// </summary>
/// <remarks>
/// <para><b>未接入不是「空对象」而是错误码</b>：官方页面开头原文「如果账户未接入，将返回错误码 <c>1040003</c>」
/// （回包表再次注明「未开通将返回 1040003（该小程序还没接入）」）⇒ 宿主须先按 <c>errcode</c> 分支，
/// 再读 <c>data</c>；把 1040003 当「已接入但任务未完成」是会误判的。</para>
/// <para>官方本页「请求参数说明」为<b>无</b>（请求体 <c>{}</c>）⇒ 端点不声明请求 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccessCheckResponse : WxaResponse
{
    /// <summary>接入状态（<c>data</c>，object，含接入任务面与场景接入面两支）。</summary>
    [JsonPropertyName("data")]
    public WxaShopAccessInfo? Data { get; set; }
}
