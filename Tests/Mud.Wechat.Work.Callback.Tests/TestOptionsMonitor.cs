// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// <see cref="IOptionsMonitor{TOptions}"/> 测试替身（固定值；支持测试中改值模拟热更新）。
/// </summary>
internal sealed class TestOptionsMonitor<T> : IOptionsMonitor<T>
{
    private T _currentValue;

    public TestOptionsMonitor(T initialValue)
    {
        _currentValue = initialValue;
    }

    /// <inheritdoc />
    public T CurrentValue => _currentValue;

    /// <inheritdoc />
    public T Get(string? name) => _currentValue;

    /// <inheritdoc />
    public IDisposable? OnChange(Action<T, string?> listener) => null;

    /// <summary>测试中替换当前值（模拟配置热更新）。</summary>
    public void Set(T value) => _currentValue = value;
}
