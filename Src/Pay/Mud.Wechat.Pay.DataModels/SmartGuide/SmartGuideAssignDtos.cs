// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.SmartGuide;

/// <summary>服务人员分配请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535161"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474145"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideAssignRequest
{
    /// <summary>【商户订单号】 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-&amp;#124;* ，且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【子商户ID】 导购所属商户的商户ID（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}
