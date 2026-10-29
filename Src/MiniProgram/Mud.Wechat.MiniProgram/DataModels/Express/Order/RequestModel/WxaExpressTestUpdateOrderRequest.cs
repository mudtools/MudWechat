// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>模拟快递公司更新订单状态请求体（<c>POST /cgi-bin/express/business/test_update_order</c>，仅用于<b>测试环境</b>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_testupdateorder"/>。</para>
/// <para>日常测试时调用本接口模拟快递公司推送轨迹，观察商户侧回调联调；生产环境不可调用（官方原文：仅供测试使用）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTestUpdateOrderRequest
{
    /// <summary>快递公司客户编码（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>商户订单号（<c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹发生时间（<c>action_time</c>，必填；Unix 时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；枚举值同附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }
}
