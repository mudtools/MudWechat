// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// <see cref="WechatAuthorizationOptions"/> 选项校验器：把编排策略校验接入 Options 管线。
/// </summary>
/// <remarks>
/// <b>P2-10</b>：与 <c>AddOptions&lt;T&gt;().Validate(lambda, "泛化文案")</c> 的差异在于——
/// 本实现把 <see cref="WechatAuthorizationOptions.Validate"/> 抛出的**真实原因**原样写入
/// <see cref="ValidateOptionsResult.Fail(string)"/>，使 <c>OptionsValidationException.Failures</c>
/// 能直接定位到具体字段（原实现的泛化文案让排障失去线索）。
/// </remarks>
public sealed class WechatAuthorizationOptionsValidator : IValidateOptions<WechatAuthorizationOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, WechatAuthorizationOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("WechatAuthorizationOptions 不能为 null。");
        }

        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail($"WechatAuthorizationOptions 配置验证失败：{ex.Message}");
        }
    }
}
