// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 更新商品请求体（<c>POST /wxaapi/broadcast/goods/update</c>，官方只有一层包装 <c>goodsInfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_updategoodsinfo.html"/>。
/// </para>
/// <para>
/// <b>本端点无返回体业务字段</b>（官方只回 <c>errcode</c> / <c>errmsg</c>）⇒ 接口返回 <see cref="WxaResponse"/>，
/// 不造专用响应类型。
/// </para>
/// <para>
/// <b>更新后需重新提审</b>：官方错误码 <c>300010</c>「商品审核状态不对（如商品审核中）」表明审核中的商品不可更新；
/// <c>300011</c>「操作非法（API 不允许操作非 API 创建的商品）」⇒ 直播控制台添加的商品不经本接口更新。
/// </para>
/// <para>官方注意事项：本接口无特殊注意事项（不设独立调用额度，与商品管理面同域）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveUpdateGoodsRequest
{
    /// <summary>商品信息（<c>goodsInfo</c>，必填；含 <c>goodsId</c> 的 8 字段对象），见 <see cref="WxaLiveUpdateGoodsInfo"/>。</summary>
    [JsonPropertyName("goodsInfo")]
    public WxaLiveUpdateGoodsInfo? GoodsInfo { get; set; }
}
