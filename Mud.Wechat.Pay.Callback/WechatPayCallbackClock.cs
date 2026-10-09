// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 回调链路时钟端口（<b>仅供测试注入</b>：让时效闸的边界用例可确定性地验证）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不用 <c>TimeProvider</c></b>：本产品线最低 TFM 为 <c>net6.0</c>，该类型自 <c>net8.0</c> 才存在；
/// 为其加条件编译只会让生产代码与测试路径分叉。
/// </para>
/// <para>刻意保持 <c>internal</c>：它不是公开扩展点（宿主无需替换时钟），公开化会制造无消费点的公开面。</para>
/// </remarks>
internal interface IWechatPayCallbackClock
{
    /// <summary>当前秒级 Unix 时间戳（UTC）。</summary>
    long UtcNowEpochSeconds { get; }
}

/// <summary>系统时钟实现（生产默认）。</summary>
internal sealed class SystemWechatPayCallbackClock : IWechatPayCallbackClock
{
    /// <inheritdoc />
    public long UtcNowEpochSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
