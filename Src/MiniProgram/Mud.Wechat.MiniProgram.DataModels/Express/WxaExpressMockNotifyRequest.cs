// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>模拟配送状态变更回调请求体（<c>POST /cgi-bin/express/intracity/mocknotify</c>，仅用于<b>测试环境</b>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_mocknotify"/>。</para>
/// <para>仅测试环境可用：模拟配送单状态变化，触发配置的 <c>callback_url</c> 回调，用于商家侧联调。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressMockNotifyRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，可选）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>，可选；与 <c>wx_order_id</c> 二选一）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>，可选；与 <c>store_order_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>模拟变更后的配送单状态（<c>order_status</c>，必填；1 待接单，2 待取货，3 配送中，4 已完成，5 已取消）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }
}
