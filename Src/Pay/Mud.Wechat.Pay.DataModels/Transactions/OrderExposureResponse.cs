// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 订单曝光应答（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014326314"/>；
/// <b>POST /v3/pay/transactions/exposure</b>。</para>
/// <para><b>应答特征</b>：成功时返回空 JSON 对象 <c>{}</c>，失败时返回 4xx + <c>{"code","message"}</c>。
/// 本接口仅用于上报曝光事件，无业务数据返回。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class OrderExposureResponse : WechatPayResponse
{
}
