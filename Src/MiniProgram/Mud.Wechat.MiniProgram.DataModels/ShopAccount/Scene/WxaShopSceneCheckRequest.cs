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
/// 检查场景值是否在支付校验范围内请求（<c>POST /shop/scene/check</c>）。
/// </summary>
/// <remarks>
/// <para><b>本端点官方页面标题即带「(待废弃）」</b>，正文首行「接口待废弃，请使用下单前置校验基础库接口」⇒
/// 页面仍在现行目录树内、契约完整在册（200 + 请求小节），故本仓照官方落地并在接口注释里标注弃用走向；
/// 新接入应优先官方指引的基础库下单前置校验。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopSceneCheckRequest
{
    /// <summary>
    /// 场景值（<c>scene</c>，number，<b>必填</b>；官方示例 <c>1175</c>）。
    /// 官方原文：「微信后台会对符合支付校验范围内的场景值下的收银台进行支付（ticket / 订单信息）校验」，
    /// 场景值枚举与获取指引见官方本页外链两节。
    /// </summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }
}
