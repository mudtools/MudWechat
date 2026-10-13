// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间商品排序请求体（<c>POST /wxaapi/broadcast/goods/sort</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_sortgoods.html"/>。
/// </para>
/// <para>
/// <b><c>goods</c> 为对象数组而非 ID 数组</b>：官方类型列 objarray、元素只含 <c>goodsId</c>（见
/// <see cref="WxaLiveSortGoodsItem"/>）⇒ 与「导入商品」的 <c>ids</c>（numarray 纯 ID 数组）形态不同，两者不共用。
/// 排序为<b>全量覆盖</b>语义：提交的数组即新的完整顺序。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>300048</c>（拉取商品列表失败）/ <c>300023</c>（房间状态拦截）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveSortGoodsRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>商品 ID 列表（<c>goods</c>，必填；官方类型 objarray，元素见 <see cref="WxaLiveSortGoodsItem"/>）。</summary>
    [JsonPropertyName("goods")]
    public List<WxaLiveSortGoodsItem>? Goods { get; set; }
}
