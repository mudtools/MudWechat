// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationSchoolPay;

/// <summary>查询用户欠费状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012469161"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayUserDebtStateResponse : WechatPayResponse
{
    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与商户号存在绑定关系。如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID。本AppID为商户在申请接入校园轻松付时所提供的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户AppID下的用户标识】 使用字段AppID生成的用户唯一标识，与返回字段AppID有对应关系（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户状态】 根据校园轻松付风控规则，当用户在校园轻松付产品中欠款超过一定限制后，会处于无法扣款的状态，直到用户归还欠款。可选状态：。可选取值：`NORMAL` 根据校园轻松付风控规则，现在可扣款 / `FORBIDDEN` 根据校园轻松付风控规则，拒绝该用户扣款（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【当前用户欠款笔数】 用户在校园轻松付中尚未归还的欠款笔数。 用户使用校园轻松付进行消费时，如果微信账户上可用的付款方式均没有足够的余额完成支付，会由微信支付出资对该笔订单进行垫付。这次垫付即会产生一笔用户向微信支付的欠款，直到用户对该笔欠款进行归还（官方字段路径 <c>debt_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("debt_count")]
    public long? DebtCount { get; set; }

    /// <summary>【用户欠款状态更新时间】 使用 rfc3339 所定义的格式，即为YYYY-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}
