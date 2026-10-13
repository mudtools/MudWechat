// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品列表应答（<c>GET /wxaapi/broadcast/goods/getapproved</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsinfo.html"/>。
/// </para>
/// <para>
/// <b>与「获取商品的信息与审核状态」应答同形不同源</b>：官方两页返回体都是 <c>{ goods: objarray, total: number }</c>，
/// 但元素字段集与拼写不同（本页 camelCase 为主、那页 snake_case 为主）⇒ 列表条目类型分立
/// （<see cref="WxaLiveApprovedGoodsItem"/> vs <see cref="WxaLiveWarehouseGoodsItem"/>），响应类型亦分立，不做共用抽象。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>-1</c> / <c>40001</c> / <c>200002</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsListResponse : WxaResponse
{
    /// <summary>商品列表（<c>goods</c>，官方类型 objarray），见 <see cref="WxaLiveApprovedGoodsItem"/>。</summary>
    [JsonPropertyName("goods")]
    public List<WxaLiveApprovedGoodsItem>? Goods { get; set; }

    /// <summary>商品个数（<c>total</c>，官方类型 number）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }
}
