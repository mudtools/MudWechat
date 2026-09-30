// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 代开发企业环境上下文（AsyncLocal，异步流隔离）。
/// </summary>
/// <remarks>
/// 企业级 access_token「一企一份」的 scope（authCorpId）动态来源（详细设计 §7.6 实施注记方案①）：
/// 业务侧经 <see cref="Authentication.IWechatAppContextSwitcher.SetCorp"/> 写入后，
/// <c>CorpTokenManager</c> 刷新时读取；跨异步边界自然隔离，多企业令牌互不串扰。
/// <para>
/// <b>归属维度（R9）</b>：<see cref="AppKey"/> 记录写入上下文的归属应用，防止多套件下
/// A 应用的 <c>permanentCode</c> 被 B 应用的令牌刷新链路误用（串号）。
/// <c>CorpTokenManager</c> 读取时校验归属：不一致即整体忽略上下文并回退仓储。
/// 为 <c>null</c> 表示未声明归属（如直接静态调用），此时不参与归属校验。
/// </para>
/// </remarks>
public static class WechatCorpContext
{
    private static readonly AsyncLocal<string?> AppKeyCurrent = new();
    private static readonly AsyncLocal<string?> AuthCorpIdCurrent = new();
    private static readonly AsyncLocal<string?> PermanentCodeCurrent = new();

    /// <summary>获取当前异步流的代操作企业归属应用键（未设置时为 null，表示未声明归属）。</summary>
    public static string? AppKey => AppKeyCurrent.Value;

    /// <summary>获取当前异步流的代操作企业 CorpId（未设置时为 null）。</summary>
    public static string? AuthCorpId => AuthCorpIdCurrent.Value;

    /// <summary>获取当前异步流的代操作企业永久授权码（未设置时为 null）。</summary>
    public static string? PermanentCode => PermanentCodeCurrent.Value;

    /// <summary>设置当前异步流的代操作企业与永久授权码。</summary>
    /// <param name="appKey">归属应用键；传 null 表示不声明归属（不参与归属校验）。</param>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="permanentCode">该企业的永久授权码（可为 null，缺省时由仓储提供）。</param>
    public static void SetCorp(string? appKey, string authCorpId, string? permanentCode)
    {
        AppKeyCurrent.Value = appKey;
        AuthCorpIdCurrent.Value = authCorpId;
        PermanentCodeCurrent.Value = permanentCode;
    }

    /// <summary>清除当前异步流的代操作企业、永久授权码与归属应用。</summary>
    public static void Clear()
    {
        AppKeyCurrent.Value = null;
        AuthCorpIdCurrent.Value = null;
        PermanentCodeCurrent.Value = null;
    }
}
