// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Tests.ExtendedSDK.Finance;

/// <summary>
/// 进程内 <c>ISecretProvider</c> 测试替身（与 <c>Mud.Wechat.Pay.Tests</c> 同款最小实现）。
/// </summary>
/// <remarks>
/// <c>Mud.HttpUtils</c> 只定义端口、不提供实现（密钥治理权在宿主）⇒ 测试侧自带。
/// 会话存档的 secret / RSA 私钥 / 代理口令都经该端口取用，替身必须能区分「查无」与「空串」：
/// 两者都被封装判为失败，但「查无」的诊断文本更长，需要分别覆盖。
/// </remarks>
internal sealed class InMemorySecretProvider : ISecretProvider
{
    private readonly Dictionary<string, string?> _secrets;

    /// <summary>以给定键值对创建。</summary>
    /// <param name="secrets">密钥名 → 密钥值。</param>
    public InMemorySecretProvider(IDictionary<string, string?> secrets)
        => _secrets = new Dictionary<string, string?>(
            secrets ?? throw new ArgumentNullException(nameof(secrets)), StringComparer.Ordinal);

    /// <summary>按名取值；未登记的名字返回 <c>null</c>（查无 ≠ 空串，但封装侧同样拒绝）。</summary>
    public Task<string?> GetSecretAsync(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        return Task.FromResult(_secrets.TryGetValue(name, out var value) ? value : null);
    }
}
