// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 获取订单列表响应体（<c>/cgi-bin/service/get_order_list</c>，应用版本付费族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class GetPayToolVersionOrderListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置订单列表（见 <see cref="PayToolVersionOrder"/>）。
    /// <para>官方说明：本端点<b>无分页游标</b>，按 <c>start_time</c>/<c>end_time</c> 时间窗拉取。</para>
    /// </summary>
    [JsonPropertyName("order_list")]
    public List<PayToolVersionOrder>? OrderList { get; set; }
}