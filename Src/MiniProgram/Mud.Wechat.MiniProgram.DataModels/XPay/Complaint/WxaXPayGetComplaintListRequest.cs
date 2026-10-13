// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「获取投诉列表」请求体（<c>POST /xpay/get_complaint_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>日期格式是 <c>YYYY-MM-DD</c> 字符串</b>（与本域账单侧 <c>YYYYMMDD</c> 数字不同形）⇒ 两支不共用类型。</para>
/// <para><b>官方注意事项列「使用用户态签名与支付签名」，参数表只列 <c>pay_sig</c></b> ⇒ 官方自相矛盾，处置见接口注释与守卫 XP3。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayGetComplaintListRequest
{
    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>begin_date（官方类型 <c>string</c>，<b>必填</b>）：筛选开始时间，格式为yyyy-mm-dd,如2023-01-01</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>end_date（官方类型 <c>string</c>，<b>必填</b>）：筛选结束时间，格式为yyyy-mm-dd,如2023-01-01</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>offset（官方类型 <c>number</c>，<b>必填</b>）：筛选偏移，从0开始</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>，<b>必填</b>）：筛选最多返回条数</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
