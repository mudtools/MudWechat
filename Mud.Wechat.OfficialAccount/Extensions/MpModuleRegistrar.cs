// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>公众号模块注册器接口（对齐企微 <c>IWechatModuleRegistrar</c>）。</summary>
public interface IMpModuleRegistrar
{
    /// <summary>注册器对应的模块。</summary>
    MpModule Module { get; }

    /// <summary>向服务集合注册该模块的 API 客户端。</summary>
    void Register(IServiceCollection services);
}

/// <summary>
/// 公众号模块注册器：包装源生成器产出的 <c>Add{Module}WebApiHttpClient()</c> 注册委托。
/// </summary>
internal sealed class MpModuleRegistrar : IMpModuleRegistrar
{
    private readonly Action<IServiceCollection> _register;

    /// <summary>创建模块注册器。</summary>
    /// <param name="module">模块枚举。</param>
    /// <param name="register">注册委托。</param>
    public MpModuleRegistrar(MpModule module, Action<IServiceCollection> register)
    {
        Module = module;
        _register = register ?? throw new ArgumentNullException(nameof(register));
    }

    /// <inheritdoc />
    public MpModule Module { get; }

    /// <inheritdoc />
    public void Register(IServiceCollection services) => _register(services);
}
