// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 获取订单详情响应体（<c>/cgi-bin/service/get_order</c>，应用版本付费族）。
/// </summary>
/// <remarks>
/// <para>
/// 官方本端点把订单字段<b>平铺在响应根级</b>（无 <c>pay_order</c>/<c>order</c> 包装节点），
/// 与「获取订单列表」的 <c>order_list</c> 数组元素同构，故本类型继承
/// <see cref="PayToolVersionOrder"/> 承载同一组官方字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class GetPayToolVersionOrderDetailResponse : PayToolVersionOrder
{
}