// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「支付时间所属范围」（<c>get_order_list</c> 的 <c>pay_time_range</c>）。
/// </summary>
/// <remarks>
/// <para><b>两端默认值不对称</b>（官方原文）：<c>begin_time</c> 缺省视为 <c>0</c>，
/// <c>end_time</c> 缺省视为 <b>32 位无符号整型的最大值</b>，且<b>含</b>该时刻 ⇒ 不传即全量时间窗。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderPayTimeRange
{
    /// <summary>begin_time（官方类型 <c>number</c>）：起始时间，时间戳形式，不填则视为从0开始。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>）：结束时间（含），时间戳形式，不填则视为32位无符号整型的最大值。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
