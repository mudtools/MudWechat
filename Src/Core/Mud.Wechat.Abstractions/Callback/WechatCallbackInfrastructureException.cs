// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 回调管线基础设施故障（分布式指纹守卫存储不可用等），区别于报文验证失败。
/// </summary>
/// <remarks>
/// <para>继承 <see cref="InvalidOperationException"/>：宿主既有 catch 块零破坏（对齐
/// <see cref="WechatCallbackException"/> 的决策 D7 模式）；且回调管线宿主的专用 catch
/// 必须置于 <c>catch (InvalidOperationException)</c> 之前（C# 更具体 catch 置后即 CS0160 编译错误）。</para>
/// <para>应答语义：回调 SPI（如 <see cref="IWechatCallbackReplayGuard"/>）实现方在存储故障时
/// 抛出本类型，管线宿主映射 503 触发企业微信 96238 重推；验证类失败
/// （<see cref="WechatCallbackException"/>）映射 403、不重推。本类型<b>仅由回调 SPI 实现方抛出</b>
/// （处理器异常由分发器隔离，不达管线宿主）。异常消息不得含密钥/密文材料。</para>
/// </remarks>
public class WechatCallbackInfrastructureException : InvalidOperationException
{
    /// <summary>创建回调基础设施故障异常。</summary>
    /// <param name="message">异常消息（只允许键名/操作描述等非敏感材料，不得含密钥/密文）。</param>
    /// <param name="innerException">内部异常（如存储层的分类异常）。</param>
    public WechatCallbackInfrastructureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
