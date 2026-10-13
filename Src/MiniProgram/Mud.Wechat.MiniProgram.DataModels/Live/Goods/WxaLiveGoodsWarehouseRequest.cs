// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取商品信息与审核状态请求体（<c>POST /wxa/business/getgoodswarehouse</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsauditinfo.html"/>。
/// </para>
/// <para>
/// <b>路由族与前缀不同源（本域最易写错的一处）</b>：本页接口英文名 <c>getGoodsAuditInfo</c>、
/// 云调用 <c>liveBroadcast.goodsInfo</c>，而路由是 <c>/wxa/business/getgoodswarehouse</c>
/// —— <b>不在</b> <c>/wxaapi/broadcast/</c> 前缀下。历史上有第三方清单把它写作
/// <c>/wxaapi/broadcast/getgoodswarehouse</c>，该路由官方不存在，已由本线零声明守卫（NC 系列）在册禁止。
/// </para>
/// <para>官方注意事项：本接口无特殊注意事项（不设独立调用额度）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsWarehouseRequest
{
    /// <summary>商品 ID 数组（<c>goods_ids</c>，必填；官方类型 numarray）。</summary>
    [JsonPropertyName("goods_ids")]
    public List<long>? GoodsIds { get; set; }
}
