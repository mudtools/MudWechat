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
/// 场景准入事项审核结果（官方 <c>/shop/register/check</c> 回包 <c>scene_group_ext_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>审核粒度是「事项」而非「场景」</b>：官方 <c>ext_id</c> 取值 <c>1</c> 客服售后、<c>2</c> 电商平台，
/// 场景总状态 <c>status</c> 由这些事项汇总而来 ⇒ 驳回原因要去外层 <c>reason</c> 取（本层官方无 reason 字段）。</para>
/// <para>官方回包示例中本层元素只写 <c>ext_id</c> 与 <c>status</c> 两支、并带尾逗号，
/// 说明该数组可只出现部分事项 ⇒ 集合与元素均可空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopSceneGroupExt
{
    /// <summary>审核事项 id（<c>ext_id</c>，number）：<c>1</c> 客服售后 / <c>2</c> 电商平台。</summary>
    [JsonPropertyName("ext_id")]
    public long? ExtId { get; set; }

    /// <summary>
    /// 该事项的审核结果（<c>status</c>，number）：<c>0</c> 审核中 / <c>1</c> 审核成功 / <c>2</c> 审核失败 / <c>3</c> 未审核。
    /// 与外层场景 <c>status</c> 同一张取值表，与 <c>/shop/audit/result</c> 的 <c>0/1/9</c> 表不同。
    /// </summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
