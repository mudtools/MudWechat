// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>获取快递公司列表应答（<c>POST /cgi-bin/express/delivery/open_msg/get_delivery_list</c>，物流消息 / 查询插件共用）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_getdeliverylist"/>。</para>
/// <para>返回支持物流消息 / 查询能力的快递公司列表；调用时请求体可传空对象。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetDeliveryListResponse : WxaResponse
{
    /// <summary>快递公司数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>快递公司列表（<c>delivery_list</c>），见 <see cref="WxaExpressOpenMsgDelivery"/>。</summary>
    [JsonPropertyName("delivery_list")]
    public List<WxaExpressOpenMsgDelivery>? DeliveryList { get; set; }
}
