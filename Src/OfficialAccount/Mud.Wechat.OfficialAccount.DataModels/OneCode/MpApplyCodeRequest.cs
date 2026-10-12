// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 申请二维码（POST /intp/marketcode/applycode）

/// <summary>
/// 申请二维码（<c>POST /intp/marketcode/applycode</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方注意事项原文：① 「<c>code_count</c> 参数必须是 <b>10000 的整数倍</b>，范围 <b>10000-20000000</b>」；
/// ② 「相同 <c>isv_application_id</c> 视为同一申请单」（⇒ 幂等键，重试不会重复生成）。
/// SDK <b>不做本地范围/倍数拦截</b>（沿用「越界由官方表达」的既存口径）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：本页错误码表把 <c>40002 invalid grant_type</c> 一并列出——该参数属
/// <c>getAccessToken</c> 接口，本接口请求体<b>无</b> <c>grant_type</c> 字段（误挂，照录不建模）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeRequest
{
    /// <summary>获取或设置申请码的数量（官方 <c>code_count</c>，必填；10000 的整数倍，范围 10000~20000000）。</summary>
    [JsonPropertyName("code_count")]
    public int CodeCount { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>，必填；相同值视为同一申请单 ⇒ 幂等键）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string IsvApplicationId { get; set; } = string.Empty;
}
