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
/// 查询审核结果应答（<c>POST /shop/audit/result</c>，官方回包为 <c>data</c> 对象包装）。
/// </summary>
/// <remarks>
/// <para><b>本域两套「审核状态」取值表并存、互不等价</b>：本页是 <c>0</c> 审核中 / <c>1</c> 审核成功 /
/// <c>9</c> 审核拒绝；<c>/shop/register/check</c> 的场景状态是 <c>0</c> 审核中 / <c>1</c> 审核完成 /
/// <c>2</c> 审核失败 / <c>3</c> 未审核 ⇒ 判定器不得共用（守卫 SHOPA6 锁定「9 = 拒绝」这一非连续取值）。</para>
/// <para><b>官方回包示例即给出 <c>"brand_id": 0</c></b>：类目审核单没有品牌，该键以 <c>0</c> 出现而非缺省
/// ⇒ 宿主不得用 <c>brand_id &gt; 0</c> 之外的方式判断品牌审核是否成功，成功判据是 <c>data.status == 1</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAuditResultResponse : WxaResponse
{
    /// <summary>审核结果（<c>data</c>，object）。</summary>
    [JsonPropertyName("data")]
    public WxaShopAuditResultInfo? Data { get; set; }
}
