// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 逐月账单条目（官方 <c>Res.bill_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方对失效的表述是「及时使用，一定时间后失效」</b>（未给具体时长）⇒ 与 <c>cos_url</c> 的 30 分钟、CDN 日志的 24 小时不同，本域这三类临时链接各有口径，<b>不得</b>互相套用有效期。</para>
/// <para><b>敏感值</b>：<c>bill_url</c> 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_ios_settlement_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayIosBillInfo
{
    /// <summary>month（官方类型 <c>string</c>）：月份，格式 YYYYMM</summary>
    [JsonPropertyName("month")]
    public string? Month { get; set; }

    /// <summary>bill_url（官方类型 <c>string</c>）：账单下载链接，及时使用，一定时间后失效</summary>
    [JsonPropertyName("bill_url")]
    public string? BillUrl { get; set; }
}
