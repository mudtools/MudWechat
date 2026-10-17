// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;

namespace Mud.Wechat.Work.AI.Configuration;

/// <summary>
/// <see cref="WechatAgentOptions"/> 的选项校验器：把 <see cref="WechatAgentOptions.Validate"/> 接入
/// <c>IOptions&lt;T&gt;</c> 解析链（首次解析即校验，非法配置在 Agent 激活时 fail-fast）。
/// </summary>
/// <remarks>
/// 校验逻辑单一来源在 <see cref="WechatAgentOptions.Validate"/>（本类型只做「异常 → 校验结果」的适配），
/// 避免出现第二份可漂移的规则表。
/// </remarks>
public sealed class WechatAgentOptionsValidator : IValidateOptions<WechatAgentOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, WechatAgentOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("WechatAi 配置为空。");
        }

        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail(ex.Message);
        }
    }
}
