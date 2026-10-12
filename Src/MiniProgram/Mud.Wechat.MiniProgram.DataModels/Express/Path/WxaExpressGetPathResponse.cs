// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询物流轨迹应答（<c>path_item_list</c> 轨迹列表）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getpath"/>。<c>action_type</c> 枚举值同附录轨迹类型（揽件/运输/派送/异常等）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPathResponse : WxaResponse
{
    /// <summary>用户 OpenId（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹信息数量（<c>path_item_num</c>）。</summary>
    [JsonPropertyName("path_item_num")]
    public int PathItemCount { get; set; }

    /// <summary>轨迹信息列表（<c>path_item_list</c>），见 <see cref="WxaExpressPathItem"/>。</summary>
    [JsonPropertyName("path_item_list")]
    public List<WxaExpressPathItem>? PathItemList { get; set; }
}
