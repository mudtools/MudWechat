// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询广告金充值记录」请求体（<c>POST /xpay/query_funds_bill</c>）。
/// </summary>
/// <remarks>
/// <para><b>分页与 filter 官方全部标注必填</b>（与本域多数查询端点不同），且 <c>filter</c> 内<c>oper_time_begin</c>/<c>oper_time_end</c> 亦必填 ⇒ 不传时间窗无法查询。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_funds_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryFundsBillRequest
{
    /// <summary>page（官方类型 <c>number</c>，<b>必填</b>）：查询页码，不小于 1</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>page_size（官方类型 <c>number</c>，<b>必填</b>）：每页记录数量</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>filter（官方类型 <c>object</c>，<b>必填</b>）：查询过滤条件</summary>
    [JsonPropertyName("filter")]
    public WxaXPayFundsBillFilter? Filter { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
