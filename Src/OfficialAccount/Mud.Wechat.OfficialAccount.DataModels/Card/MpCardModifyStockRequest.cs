// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 修改库存（POST /card/modifystock）

/// <summary>
/// 修改卡券库存（<c>POST /card/modifystock</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>键名为 <c>increase_stock_value</c> / <c>reduce_stock_value</c></b>（对齐基准：SKIT
/// <c>CardModifyStockRequest</c>）—— <b>不是</b>建卡 <c>sku.quantity</c> 的「quantity」词族，也<b>不是</b>
/// <c>increase_quantity</c>；SDK 不凭常识改写，守卫 CD6 锁定。
/// </para>
/// <para>
/// <b>增删同传</b>：两字段均为「增量」语义（非目标值），官方允许同时携带 ⇒ SDK <b>不做互斥校验</b>，
/// 越界由官方错误码表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardModifyStockRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置增加库存数（官方 <c>increase_stock_value</c>，选填，正整数增量）。</summary>
    [JsonPropertyName("increase_stock_value")]
    public int? IncreaseStockValue { get; set; }

    /// <summary>获取或设置减少库存数（官方 <c>reduce_stock_value</c>，选填，正整数减量）。</summary>
    [JsonPropertyName("reduce_stock_value")]
    public int? ReduceStockValue { get; set; }
}
