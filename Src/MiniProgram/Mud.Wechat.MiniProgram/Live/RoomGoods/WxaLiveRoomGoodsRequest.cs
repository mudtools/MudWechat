// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 「房间 + 商品」请求体（<c>{ roomId, goodsId }</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：三端点的官方请求体字段集<b>完全一致</b>（<c>roomId</c> + <c>goodsId</c>，均必填、均 number）⇒
/// 共用本类型：
/// <c>POST /wxaapi/broadcast/goods/deleteInRoom</c>（删除直播间商品）、
/// <c>POST /wxaapi/broadcast/goods/push</c>（推送商品）、
/// <c>POST /wxaapi/broadcast/goods/getVideo</c>（下载商品讲解视频）。
/// </para>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletedoods.html"/>。
/// </para>
/// <para>
/// <b>状态互斥（官方错误码明示）</b>：<c>300047</c>「已有商品正在推送，请稍后再试」与
/// <c>300049</c>「商品推送过程中不允许上下架」⇒ 推送为<b>串行</b>操作，宿主须避免并发推送同房间；
/// <c>300052</c>（已下架的商品无法推送）须先 <c>onsale</c> 上架。
/// </para>
/// <para>各端点调用额度均为 10000 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoomGoodsRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>商品 ID（<c>goodsId</c>，必填）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }
}
