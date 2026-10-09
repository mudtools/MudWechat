// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.Credential;

/// <summary>
/// 进程内 <c>ISecretProvider</c> 测试替身。
/// </summary>
/// <remarks>
/// <c>Mud.HttpUtils</c> 只定义端口、不提供实现（密钥治理权在宿主），故测试侧自带最小实现。
/// </remarks>
internal sealed class InMemorySecretProvider : ISecretProvider
{
    private readonly Dictionary<string, string> _secrets;

    /// <summary>以给定键值对创建。</summary>
    /// <param name="secrets">密钥名 → 密钥值。</param>
    public InMemorySecretProvider(IDictionary<string, string> secrets)
        => _secrets = new Dictionary<string, string>(
            secrets ?? throw new ArgumentNullException(nameof(secrets)), StringComparer.Ordinal);

    /// <summary>按名取值；未登记的名字返回 <c>null</c>。</summary>
    /// <remarks>实现签名不写 <c>string?</c>：测试工程因 <c>Tests/Directory.Build.props</c> 遮蔽根 props
    /// 而处于 nullable 关闭态，<c>?</c> 注解会报 CS8632 且无实际约束力。</remarks>
    public Task<string> GetSecretAsync(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        return Task.FromResult(_secrets.TryGetValue(name, out var value) ? value : null);
    }
}
