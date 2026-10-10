// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 会话内容存档 C SDK <b>原生入口</b>返回码非 0（或取数阶段形态异常）时抛出的异常。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不复用 <see cref="Mud.Wechat.Work.Abstractions.Exceptions.WechatWorkException"/></b>：那是
/// <c>/cgi-bin/*</c> 的 <c>errcode</c> 通道。原生返回码与 <c>errcode</c> 是<b>两层不同事实</b>
/// —— 原生返回非 0 时 <c>Slice</c> 内容不保证是合法 JSON，原生返回 0 而信封 <c>errcode != 0</c>
/// 也可能出现（密钥/机器人权限类问题走信封侧）。合用一种异常会让调用方无法判断「该重试、还是该换密钥」，
/// 故两条通道各持一个类型（守卫 FIN-B1/FIN-B5 双向锁定「不得混用」）。
/// 信封层的判错仍走 <c>WechatWorkException.ThrowIfFailed</c>。
/// </para>
/// <para>
/// <b>消息里可以出现什么</b>：入口名、返回码、机器人键、版本号。<b>绝不出现</b>密文、明文正文、
/// <c>corpid</c>、机器人 secret、RSA 私钥或解密后的 AES 密钥（守卫 FIN-B5）。
/// </para>
/// </remarks>
/// <param name="EntryName">发生异常的原生入口名（如 <c>GetChatData</c>）。</param>
/// <param name="ReturnCode">原生返回码；取数阶段形态异常时为 <c>0</c>。</param>
/// <param name="message">诊断消息（不含任何凭据与报文内容）。</param>
public sealed class WechatFinanceNativeException(
    string EntryName, int ReturnCode, string message) : InvalidOperationException(message)
{
    /// <summary>发生异常的原生入口名。</summary>
    public string EntryName { get; } = EntryName;

    /// <summary>原生返回码（0 = 非返回码成因，而是取数阶段的形态异常）。</summary>
    public int ReturnCode { get; } = ReturnCode;
}

/// <summary>
/// 原生返回码常量与重试判据。
/// </summary>
/// <remarks>
/// <para>
/// <b>只有 0 是被断言的契约</b>（<c>0 = 成功</c>，SKIT 与官方示例一致，本仓守卫亦只锁这一条）。
/// 其余取值的<b>中文释义</b>来自本地 SKIT 源码与其注释转述，官方页面 <c>path/91774</c> 的逐条原文
/// **待逐页核验** ⇒ 本类<b>不做</b>逐码枚举（把未核验的语义写进常量名 = 让未核验事实成为契约面），
/// 只在 <see cref="IsMediaShardRetryable"/> 里承载「SKIT 实测对 10001/10002/10003 做退避重试」这一
/// <b>行为</b>事实，并允许宿主用 <c>WechatFinanceOptions.MediaShardRetryCount = 0</c> 关闭。
/// </para>
/// </remarks>
public static class WechatFinanceNativeCodes
{
    /// <summary>成功（唯一被断言的返回码语义）。</summary>
    public const int Success = 0;

    /// <summary>
    /// 媒体分片拉取时是否值得按同一游标退避重试。
    /// </summary>
    /// <param name="returnCode">原生返回码。</param>
    /// <remarks>
    /// 判据 <c>10001 / 10002 / 10003</c> 取自 SKIT <c>WechatWorkFinanceClient.ExecuteGetMediaFileAsync</c>
    /// 的重试分支（其行为事实，非官方文案）。<b>只对媒体分片生效</b>：会话记录拉取（<c>GetChatData</c>）
    /// 不重试 —— 它的游标语义下重试同一次调用与「下一次调用」等价，交给上层轮询更清楚。
    /// </remarks>
    public static bool IsMediaShardRetryable(int returnCode)
        => returnCode == 10001 || returnCode == 10002 || returnCode == 10003;
}
