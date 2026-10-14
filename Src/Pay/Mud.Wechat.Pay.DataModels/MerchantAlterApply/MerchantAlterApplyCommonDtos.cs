// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantAlterApply;

/// <summary>
/// 三族「提交申请单」的共用应答（官方 <c>apply_id</c> 单字段）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：主体 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>、
/// 超级管理员 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/>、
/// 商家简称 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166071"/>，2026-10-14 逐字段核验。</para>
/// <para><b>三页应答表完全相同</b>（<c>apply_id</c> 选填 string(64)，语义「微信支付分配的申请单号」）
/// ⇒ 按「表相同则共用」归并为一型，三族提交支各自返回本型（守卫 MAA-B6）。
/// 这也是本域唯一的「三族共用」类型：<c>state</c> 值集在查询/撤销支分族，唯有受理应答同形。</para>
/// <para><b>拿到 <c>apply_id</c> 只表示受理成功</b>：官方是审核流，须轮询查询支到
/// <c>APPLYMENT_STATE_FINISHED</c> 才算资料变更完成。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplySubmitResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>apply_id</c>，选填 string(64)）：微信支付分配，后续查询与撤销以它定位。</summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }
}

/// <summary>
/// 超级管理员族与商家简称族「撤销申请单」的共用应答（<c>apply_id</c> + <c>state</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：超管 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617919"/>、
/// 简称 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166090"/>，2026-10-14 逐字段核验。</para>
/// <para><b>两页字段表相同（<c>apply_id</c> 选填 string、<c>state</c> 选填 string）⇒ 共用本型</b>，
/// 但 <c>state</c> 的值清单两页不同（超管页印 5 值、简称页只印 <c>APPLYMENT_STATE_CANCELED</c> 1 值）——
/// 差异只在文档完整性，不构成两个契约，故不因此分建（守卫 MAA-B6 锁「共用」、MAA-B4 锁「值集分族」）。</para>
/// <para><b>与主体族的撤销应答分建</b>：主体族回 <c>{result}</c>（<c>SUCCESS</c> / <c>FAIL</c>），
/// 本型回 <c>{apply_id, state}</c> ⇒ 同域两形态，跨族复制即丢字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyRevokeResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>apply_id</c>，选填 string）。</summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>申请单状态（<c>state</c>，选填 string）：撤销后为 <c>APPLYMENT_STATE_CANCELED</c>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
