// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>拉取充值订单信息请求体（<c>POST /wxa/business/insurance_freight/getpayorderlist</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getpayorderlist"/>（接口英文名 <c>insurance_freight_getpayorderlist</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayOrderListRequest
{
    /// <summary>订单状态过滤（<c>status_list</c>，必填；枚举见 <see cref="WxaExpressPayOrderItem"/>）。</summary>
    [JsonPropertyName("status_list")]
    public int[]? StatusList { get; set; }

    /// <summary>分页偏移（<c>offset</c>，可选）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>分页大小（<c>limit</c>，可选）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
