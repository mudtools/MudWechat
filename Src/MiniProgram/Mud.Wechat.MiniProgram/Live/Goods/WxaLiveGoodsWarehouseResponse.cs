// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品信息与审核状态应答（<c>POST /wxa/business/getgoodswarehouse</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsauditinfo.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表不含 <c>errcode</c> / <c>errmsg</c></b>（只有 <c>goods</c> 与 <c>total</c>）
/// ⇒ 继承 <see cref="WxaResponse"/> 后成功响应两字段取默认值，判成败仍走 <see cref="WxaResponse.IsSuccess"/>
/// （本域「官方成功示例不带 errcode」的又一处，与创建直播间同类）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsWarehouseResponse : WxaResponse
{
    /// <summary>商品列表（<c>goods</c>，官方类型 objarray），见 <see cref="WxaLiveWarehouseGoodsItem"/>。</summary>
    [JsonPropertyName("goods")]
    public List<WxaLiveWarehouseGoodsItem>? Goods { get; set; }

    /// <summary>商品个数（<c>total</c>，官方类型 number）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }
}
