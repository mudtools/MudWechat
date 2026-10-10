// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Contracts;

/// <summary>
/// 微信系 API 统一响应判错契约（**非序列化**契约：只描述判错面，不约束 DTO 的 JSON 形状）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何下沉契约而非 DTO 基底</b>：各产品线的响应基底 DTO 形态并不一致——
/// 企业微信恒带 <c>errcode</c>；公众号 <c>token</c> / <c>stable_token</c> **成功响应不带 <c>errcode</c>**；
/// 微信支付 APIv3 更是无 <c>errcode</c> 字段。故响应**基底 DTO 留在各产品线 DataModels**，
/// 本层只抽「判错面」这一最小公共语义，由 <see cref="WechatApiException"/> 的
/// <c>ThrowIfFailed</c> 统一消费。
/// </para>
/// <para>
/// 实现方约定：<c>ErrorCode</c> 缺省值必须为 <c>0</c>（成功），以天然覆盖「响应体未携带错误码」的形态。
/// </para>
/// </remarks>
public interface IWechatApiResponse
{
    /// <summary>API 业务错误码（<c>0</c> 表示成功；响应体未携带时保持缺省 0）。</summary>
    int ErrorCode { get; }

    /// <summary>API 业务错误信息（可空）。</summary>
    string? ErrorMessage { get; }

    /// <summary>本次调用是否成功（<see cref="ErrorCode"/> == 0）。</summary>
    bool IsSuccess { get; }
}
