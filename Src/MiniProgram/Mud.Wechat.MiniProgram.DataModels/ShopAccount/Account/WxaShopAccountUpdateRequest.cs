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
/// 更新商家信息请求（<c>POST /shop/account/update_info</c>，字段平铺于请求体顶层、无 <c>data</c> 包装）。
/// </summary>
/// <remarks>
/// <para><b>覆盖式写入</b>：本页是「商家信息」这一账号级唯一值的写入口，重复提交即整份覆盖 ⇒
/// 宿主须先读 <c>account/get_info</c> 再回写，不得只带增量字段当作补丁提交。</para>
/// <para><b>必填集与读侧不同</b>（官方逐页参数表原文）：<c>service_agent_type</c> 必填，
/// <c>default_receiving_address</c> 的 <c>receiver_name</c> / <c>detailed_address</c> / <c>tel_number</c> /
/// <c>province</c> / <c>city</c> / <c>town</c> 均必填、仅 <c>country</c> 选填；
/// 而 <c>account/get_info</c> 的同一结构把 <c>province</c> / <c>city</c> / <c>town</c> 标为选填。</para>
/// <para><b>本类型与 <see cref="WxaShopAccountInfo"/> 不合并</b>：读侧有 <c>data</c> 包装、写侧没有，
/// 合并会让其中一侧的 JSON 形状出错（守卫 SHOPA5 锁定）。</para>
/// <para><b>敏感面</b>：收件人姓名 / 手机号 / 详细地址与客服联系电话，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccountUpdateRequest
{
    /// <summary>客服地址（<c>service_agent_path</c>，string，选填）：小程序页面 path。</summary>
    [JsonPropertyName("service_agent_path")]
    public string? ServiceAgentPath { get; set; }

    /// <summary>客服联系方式（<c>service_agent_phone</c>，string，选填）。禁落日志。</summary>
    [JsonPropertyName("service_agent_phone")]
    public string? ServiceAgentPhone { get; set; }

    /// <summary>
    /// 客服类型（<c>service_agent_type</c>，enum 数组，<b>必填</b>，支持多个）。
    /// 官方取值：<c>0</c> 小程序客服（官方原文注明「会校验是否开启」）、<c>1</c> 自定义客服 path、<c>2</c> 联系电话。
    /// </summary>
    [JsonPropertyName("service_agent_type")]
    public List<int>? ServiceAgentType { get; set; }

    /// <summary>
    /// 默认退货地址（<c>default_receiving_address</c>，object）：官方用途原文——退货售后超时时让用户把货物寄往此地址。
    /// 官方参数表未给本支的「是否必填」标记，但其子字段六支标「是」⇒ 实际按必填对象提交。
    /// </summary>
    [JsonPropertyName("default_receiving_address")]
    public WxaShopAddressInfo? DefaultReceivingAddress { get; set; }
}
