// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 微信支付**平台证书**存取端口（按序列号分槽）—— 轮换与热更的隔离缝（方案 v2 §2.2 第 3 条）。
/// </summary>
/// <remarks>
/// <para>
/// 为什么按序列号而不是「当前证书」单槽：官方在证书到期前会**并行**下发新旧两本平台证书，
/// 应答/回调头里的 <c>Wechatpay-Serial</c> 可能指向任一本；单槽会让切换瞬间的验签全量失败。
/// </para>
/// <para>
/// <b>未知序列号 ⇒ 查不到 ⇒ 上层验签返回 <c>false</c>（fail-closed）</b>，
/// 这正是守卫 <b>PAY-B3</b> 第 4 条的落点。上层可在刷新证书后**重试一次**，仍未知则拒绝——不得放宽。
/// </para>
/// <para>本接口属端口，实现可替换（进程内缓存 / Redis / 宿主自管），与 <c>IWechatCallbackReplayGuard</c> 同款做法。</para>
/// </remarks>
public interface IWechatPayPlatformCertificateStore
{
    /// <summary>按序列号取平台证书。</summary>
    /// <param name="serialNumber">平台证书序列号（<c>Wechatpay-Serial</c>）。</param>
    /// <param name="certificate">命中时为证书实例；否则 <c>null</c>。</param>
    /// <returns>是否命中。</returns>
    /// <remarks>序列号为 <c>null</c>/<c>空白</c> 时**必须**返回 <c>false</c>，不得回落到「任意证书」。</remarks>
    bool TryGetCertificate(string? serialNumber, out X509Certificate2? certificate);
}
