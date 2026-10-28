// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 上下架商品请求体（<c>POST /wxaapi/broadcast/goods/onsale</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_salegoods.html"/>。
/// </para>
/// <para>
/// <b><c>onSale</c> 为 0/1 开关而非布尔</b>（0 下架 / 1 上架）⇒ 建为 <see cref="long"/>?，不建常量类
/// —— 本域内 <c>close*</c>、<c>isFeedsPublic</c>、<c>banComment</c>、<c>onSale</c> 全是同族 0/1 开关，
/// 统一不建常量类（照录原则：官方未给独立枚举表，只在说明里写两值）。
/// </para>
/// <para>
/// <b>官方接口英文名大小写异常</b>：本页接口英文名为 <c>SaleGoods</c>（首字母大写，本域唯一），
/// 路由为 <c>goods/onsale</c>、云调用为 <c>liveBroadcast.goodsSale</c> ⇒ 仅留档，不影响类型名。
/// </para>
/// <para>官方错误码：<c>300049</c>「商品推送过程中不允许上下架」（须等推送结束）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveOnSaleRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>商品 ID（<c>goodsId</c>，必填）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }

    /// <summary>上下架（<c>onSale</c>，必填）：0 下架、1 上架。</summary>
    [JsonPropertyName("onSale")]
    public long? OnSale { get; set; }
}
