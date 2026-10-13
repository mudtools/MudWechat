// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品排序项（<c>goods[].goodsId</c>，<c>POST /wxaapi/broadcast/goods/sort</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_sortgoods.html"/>。
/// </para>
/// <para>
/// <b>参数表 number 而示例字符串（照录参数表）</b>：本页 <c>goods[].goodsId</c> 的类型列为 number，
/// 官方「请求示例」却写 <c>{ "goodsId": "123" }</c> ⇒ 建为 <see cref="long"/>?（参数表为权威，
/// 同域「上下架 / 推送 / 删除」的 <c>goodsId</c> 亦为 number）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveSortGoodsItem
{
    /// <summary>商品 ID（<c>goodsId</c>，必填；官方类型列 number）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }
}
