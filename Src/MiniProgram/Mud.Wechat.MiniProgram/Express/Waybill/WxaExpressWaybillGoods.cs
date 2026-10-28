// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>物流消息商品信息（<c>open_msg</c> 族的 <c>goods_info</c> 内嵌对象）。</summary>
/// <remarks>字段以官方「物流消息」相关文档核验（2026-10-11）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillGoods
{
    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品图片 URL（<c>goods_img_url</c>）。</summary>
    [JsonPropertyName("goods_img_url")]
    public string? GoodsImageUrl { get; set; }

    /// <summary>商品详情描述（<c>goods_desc</c>）。</summary>
    [JsonPropertyName("goods_desc")]
    public string? GoodsDescription { get; set; }

    /// <summary>商品详情列表（<c>detail_list</c>），见 <see cref="WxaExpressWaybillGoodsDetail"/>。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressWaybillGoodsDetail>? DetailList { get; set; }
}
