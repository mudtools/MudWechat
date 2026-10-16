// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BusinessCircle;

/// <summary>商圈会员积分同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534698"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474133"/></summary>
[HttpJsonSerializable(SerializerClassName = "BusinessCircle")]
public class BusinessCirclePointsSyncRequest
{
    /// <summary>【商圈商户ID】 当以服务商模式管理商圈积分能力时，则要带上商圈商户ID，否则留空（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信订单号】 微信支付推送的商圈内交易通知里携带的微信订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【小程序AppID】 顾客授权积分时使用的小程序的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【顾客OpenID】 顾客授权时使用的小程序上的OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【是否获得积分】 用于标明此单是否获得积分，true为获得积分，false为未获得（官方字段路径 <c>earn_points</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("earn_points")]
    public bool? EarnPoints { get; set; }

    /// <summary>【订单新增积分值】 顾客此笔交易新增的积分值（官方字段路径 <c>increased_points</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("increased_points")]
    public long? IncreasedPoints { get; set; }

    /// <summary>【积分更新时间】 为顾客此笔交易成功积分的时间（官方字段路径 <c>points_update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("points_update_time")]
    public string? PointsUpdateTime { get; set; }

    /// <summary>【未获得积分的备注信息】 当未获得积分时，提供未获得积分的原因等备注信息（官方字段路径 <c>no_points_remarks</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("no_points_remarks")]
    public string? NoPointsRemarks { get; set; }

    /// <summary>【顾客积分总额】 当前顾客积分总额（官方字段路径 <c>total_points</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_points")]
    public long? TotalPoints { get; set; }

}
