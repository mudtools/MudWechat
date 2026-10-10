// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;

namespace Mud.Wechat.Redis.Configuration;

/// <summary>
/// <see cref="WechatRedisOptions"/> 的启动期校验器（IValidateOptions 管线）。
/// </summary>
/// <remarks>
/// net6+ 宿主经 <c>ValidateOnStart()</c> 在启动期触发；ns2.0 无该入口，
/// 由 <c>IOptions&lt;WechatRedisOptions&gt;.Value</c> 首次解析时触发（连接装配即消费）。
/// </remarks>
public class WechatRedisOptionsValidator : IValidateOptions<WechatRedisOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, WechatRedisOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("WechatRedisOptions 配置不能为 null。");
        }

        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail($"WechatRedisOptions 配置验证失败：{ex.Message}");
        }
    }
}
