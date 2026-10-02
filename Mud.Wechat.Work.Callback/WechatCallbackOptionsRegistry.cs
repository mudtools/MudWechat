// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调配置注册表：按「接收方 ID」登记各套件/自建应用的回调配置（P1-3 统一注册表，决策 D11）。
/// </summary>
/// <remarks>
/// <para>
/// 接收方 ID 语义：企业自建应用回调为企业 <c>CorpId</c>；第三方/服务商代开发套件回调为 <c>SuiteId</c>
/// （即外层 XML ToUserName）。同一接收方 ID 全表唯一——官方同一 ToUserName 只对应一套 Token/AESKey
/// （同企业多自建应用请共用同一套回调参数，按事件 AgentID 区分应用）。
/// </para>
/// <para>
/// SDK 基础设施：经 <c>AddWechatCallback</c>/<c>AddWechatCallbackSuite</c> 登记（注册期 fail-fast），
/// 宿主一般不直接使用。
/// </para>
/// </remarks>
internal sealed class WechatCallbackOptionsRegistry
{
    private readonly Dictionary<string, WechatCallbackOptions> _entries = new(StringComparer.Ordinal);

    /// <summary>已登记的回调配置（键 = 接收方 ID）。</summary>
    public IReadOnlyDictionary<string, WechatCallbackOptions> Entries => _entries;

    /// <summary>
    /// 登记一套回调配置（注册期 fail-fast：配置完整性立即校验，接收方 ID 为注册表键，须非空且全表唯一）。
    /// </summary>
    /// <param name="options">回调配置（登记后由本表持有，宿主不应再修改）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> 为 null。</exception>
    /// <exception cref="InvalidOperationException">配置校验失败（含接收方 ID 为空）或接收方 ID 已登记时抛出。</exception>
    public void Add(WechatCallbackOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        // 完整性 + 接收方 ID 非空（PushToken / PushEncodingAESKey / CorpId）在注册期即时暴露（F12）。
        options.Validate();

        var receiverId = options.CorpId;
        if (_entries.ContainsKey(receiverId))
        {
            throw new InvalidOperationException(
                $"回调配置的接收方 ID 重复：{receiverId}（同一接收方 ID 只允许登记一套回调配置；" +
                "同企业多自建应用请共用同一套回调参数，多套件请使用各自的 SuiteId）。");
        }

        _entries.Add(receiverId, options);
    }
}
