// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 添加并提审商品应答（<c>POST /wxaapi/broadcast/goods/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html"/>。
/// </para>
/// <para>
/// <b>两个 ID 必须持久化</b>：官方注意事项明示丢失即无法调用本域其他接口（重提审需 <c>goodsId</c>、
/// 撤回审核需 <c>goodsId</c> + <c>auditId</c>）。
/// </para>
/// <para>
/// <b>成功示例里 <c>errcode</c> 后置且 <c>errmsg</c> 缺省</b>（官方示例 <c>{ "goodsId": 51, "auditId": 525022786, "errcode": 0 }</c>）
/// ⇒ 判成败只看 <c>errcode</c>；<c>300021</c>「商品添加成功，审核失败」时仍可能带回 <c>goodsId</c>，宿主须按业务态处理。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAddGoodsResponse : WxaResponse
{
    /// <summary>商品 ID（<c>goodsId</c>，官方类型 number）。注意「获取商品列表」回的是 string，见 <see cref="WxaLiveApprovedGoodsItem"/>。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }

    /// <summary>审核单 ID（<c>auditId</c>，官方类型 number），撤回审核时必填。</summary>
    [JsonPropertyName("auditId")]
    public long? AuditId { get; set; }
}
