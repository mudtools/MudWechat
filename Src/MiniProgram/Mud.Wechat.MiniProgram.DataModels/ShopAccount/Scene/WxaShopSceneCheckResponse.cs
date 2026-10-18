// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 检查场景值是否在支付校验范围内应答（<c>POST /shop/scene/check</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方本页有两处「表内有、示例中无」的字段</b>（逐字照官方回包表建模，不做归一）：
/// ① 回包示例只有 <c>is_matched</c>，回包表另有 <c>requiredFundType</c>；
/// ② <c>requiredFundType</c> 的说明写作「<c>require_order</c> = 1 时生效」，而本页请求表只有
/// <c>scene</c> 一支、<c>require_order</c> 全文未定义 ⇒ 该键只能视为条件性回填，宿主须容忍缺省。</para>
/// <para><b><c>requiredFundType</c> 是本域唯一的驼峰键</b>（本域其余 38 支 DTO 全为 snake_case）：
/// 键名照官方原文钉死，不得「顺手」改成 <c>required_fund_type</c>，守卫 SHOPA5 锁定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopSceneCheckResponse : WxaResponse
{
    /// <summary>
    /// 是否需要支付校验（<c>is_matched</c>，官方类型列 enum）：<c>1</c> 需要 / <c>0</c> 不需要。
    /// 官方回包示例为数值 <c>1</c> ⇒ 建为可空数值而非布尔（不改变线格式）。
    /// </summary>
    [JsonPropertyName("is_matched")]
    public long? IsMatched { get; set; }

    /// <summary>
    /// 资金主体类型（<c>requiredFundType</c>，官方类型列 enum，官方原文「require_order = 1 时生效」）：
    /// <c>0</c> 非二级商户号订单 / <c>1</c> 二级商户号订单 / <c>2</c> 两种方式皆可（官方注「后续只会存在 1」）。
    /// </summary>
    [JsonPropertyName("requiredFundType")]
    public long? RequiredFundType { get; set; }
}
