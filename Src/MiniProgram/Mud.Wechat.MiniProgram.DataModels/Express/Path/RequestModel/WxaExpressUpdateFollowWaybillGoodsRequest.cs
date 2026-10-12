// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>更新运单商品信息请求体（<c>POST /cgi-bin/express/delivery/open_msg/update_follow_waybill_goods</c>，物流消息）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_updatefollowwaybillgood"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateFollowWaybillGoodsRequest
{
    /// <summary>运单 Token（<c>waybill_token</c>，必填）。</summary>
    [JsonPropertyName("waybill_token")]
    public string? WaybillToken { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }
}
