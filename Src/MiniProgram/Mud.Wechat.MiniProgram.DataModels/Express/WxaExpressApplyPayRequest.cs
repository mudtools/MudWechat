// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>申请支付请求体（<c>POST /wxa/business/insurance_freight/applypay</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_applypay"/>（接口英文名 <c>insurance_freight_applypay</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressApplyPayRequest
{
    /// <summary>充值订单 ID（<c>order_id</c>，必填；与 <see cref="WxaExpressCreateChargeIdResponse.OrderId"/> 保持一致）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }
}
