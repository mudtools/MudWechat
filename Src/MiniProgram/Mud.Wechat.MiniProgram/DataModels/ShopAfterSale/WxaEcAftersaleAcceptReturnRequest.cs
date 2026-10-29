// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 同意退货请求体（<c>POST /shop/ecaftersale/acceptreturn</c>）。
/// </summary>
/// <remarks>
/// <para><b>退货地址键名是 <c>town</c></b>：本端点用 <c>town</c> 表区县，官方公共结构 <see cref="WxaEcAftersaleReturnAddressInfo"/></para>
/// <para>用 <c>county</c> ⇒ 两个类型分开建模，勿合并。</para>
/// <para><b>敏感值</b>：<c>address_info</c> 的姓名 / 电话 / 地址属个人信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleAcceptReturnRequest
{
    /// <summary>微信侧售后单号（与 <c>out_aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>外部售后单号（与 <c>aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>退货收货地址（必填）。</summary>
    [JsonPropertyName("address_info")]
    public WxaEcAftersaleAcceptReturnAddressInfo? AddressInfo { get; set; }
}
