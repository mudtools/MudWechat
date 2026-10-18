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
/// 获取商家信息应答（<c>POST /shop/account/get_info</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>data</c> 是对象</b>（与 <c>get_brand_list</c> / <c>get_category_list</c> 的 <c>data</c> 数组形态不同）。</para>
/// <para>官方本页「请求参数说明」为<b>无</b>（请求体 <c>{}</c>）⇒ 端点不声明请求 DTO。</para>
/// <para><b>敏感面</b>：回包含客服联系电话与默认退货地址的收件人姓名 / 手机号 / 详细地址，
/// <b>不得</b>写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccountInfoResponse : WxaResponse
{
    /// <summary>商家信息（<c>data</c>，object）。</summary>
    [JsonPropertyName("data")]
    public WxaShopAccountInfo? Data { get; set; }
}
