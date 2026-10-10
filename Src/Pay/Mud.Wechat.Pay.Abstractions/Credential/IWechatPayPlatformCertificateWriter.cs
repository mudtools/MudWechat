// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 平台证书<b>写入</b>端口（刷新侧专用；验签侧的 <see cref="IWechatPayPlatformCertificateStore"/> 保持只读）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何与读端口分开</b>：验签是「每笔回调都要走」的热路径，其端口必须是<b>最小面</b>
/// （只有 <c>TryGetCertificate</c>）——宿主若只想提供一份只读证书源（如从 KMS / 配置中心读取），
/// 不该被迫实现写入。而写入只被<b>证书刷新</b>一条链路使用，故独立成端口。
/// </para>
/// <para>
/// <b>默认实现</b>：<see cref="WechatPayPlatformCertificateCache"/> 同时实现读写两端口
/// （其 <c>Set</c> 已存在），由 <c>AddPayApp</c> 一并登记。
/// </para>
/// <para>
/// <b>所有权</b>：登记后证书实例<b>归实现方所有</b>（由其在覆盖 / 释放时负责 <c>Dispose</c>），
/// 调用方<b>不得</b>在登记后继续使用该实例。
/// </para>
/// </remarks>
public interface IWechatPayPlatformCertificateWriter
{
    /// <summary>登记（或覆盖）一本平台证书。</summary>
    /// <param name="serialNumber">平台证书序列号（<c>Wechatpay-Serial</c> 取值来源）。</param>
    /// <param name="certificate">证书实例（<b>所有权随之转移</b>）。</param>
    /// <exception cref="ArgumentException"><paramref name="serialNumber"/> 为 <c>null</c>/空白。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="certificate"/> 为 <c>null</c>。</exception>
    /// <remarks>同序列号重复登记为<b>覆盖</b>语义（旧实例由实现方释放）。</remarks>
    void Set(string serialNumber, X509Certificate2 certificate);
}
