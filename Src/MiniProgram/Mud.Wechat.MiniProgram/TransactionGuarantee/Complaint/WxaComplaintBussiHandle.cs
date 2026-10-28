// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 商家回应投诉的处理态度官方枚举值（<c>bussiHandle</c>）。
/// </summary>
/// <remarks>
/// 官方原文（「商家回应投诉」请求体参数表）：「操作：1 是同意和解，2 是拒绝和解」⇒ 官方仅公布这两个取值，
/// 本类不得扩充。相关错误码：<c>309</c> 状态不对（可能已转到下一个状态）、<c>1</c> 单号不存在、
/// <c>10001</c> 参数异常、<c>40001</c> access_token 无效。
/// </remarks>
public static class WxaComplaintBussiHandle
{
    /// <summary>同意和解（<c>1</c>）。</summary>
    public const int AgreeCompromise = 1;

    /// <summary>拒绝和解（<c>2</c>）。</summary>
    public const int RefuseCompromise = 2;
}
