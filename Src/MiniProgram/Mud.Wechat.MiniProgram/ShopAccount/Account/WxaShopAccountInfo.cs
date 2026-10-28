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
/// 商家信息对象（官方 <c>account/get_info</c> 回包 <c>data</c> 的内容体）。
/// </summary>
/// <remarks>
/// <para><b>与请求侧 <see cref="WxaShopAccountUpdateRequest"/> 的关系是「同字段、不同包装」</b>：
/// 写侧四支键平铺在请求体顶层，读侧同四支键包在 <c>data</c> 里 ⇒ 两个类型不合并（守卫 SHOPA5 锁定包装差异）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccountInfo
{
    /// <summary>
    /// 客服地址（<c>service_agent_path</c>，string）：小程序页面 path。
    /// 仅当 <see cref="ServiceAgentType"/> 含 <c>1</c> 时有意义。
    /// </summary>
    [JsonPropertyName("service_agent_path")]
    public string? ServiceAgentPath { get; set; }

    /// <summary>
    /// 客服联系方式（<c>service_agent_phone</c>，string；官方示例 <c>020-888888</c>）。
    /// 仅当 <see cref="ServiceAgentType"/> 含 <c>2</c> 时有意义。属可识别联系方式，禁落日志。
    /// </summary>
    [JsonPropertyName("service_agent_phone")]
    public string? ServiceAgentPhone { get; set; }

    /// <summary>
    /// 客服类型（<c>service_agent_type</c>，number 数组，支持多个）。
    /// 官方取值：<c>0</c> 小程序官方客服（写侧原文注明「会校验是否开启」，读侧写作「小程序官方客服」）、
    /// <c>1</c> 自定义客服 path、<c>2</c> 联系电话。
    /// </summary>
    [JsonPropertyName("service_agent_type")]
    public List<int>? ServiceAgentType { get; set; }

    /// <summary>
    /// 默认退货地址（<c>default_receiving_address</c>，object）：官方用途原文——售后超时时让用户把商品退往此地址。
    /// </summary>
    [JsonPropertyName("default_receiving_address")]
    public WxaShopAddressInfo? DefaultReceivingAddress { get; set; }
}
