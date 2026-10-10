// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.Abstractions.Configuration;

namespace Mud.Wechat.Channels.Abstractions;

/// <summary>
/// <see cref="ChannelsAppConfig"/> 选项校验器：把 <see cref="ChannelsAppConfig.Validate"/> 的启动期校验接入
/// Options 校验管线，支持单应用与多应用列表两种注册形态。
/// </summary>
public class ChannelsAppConfigValidator :
    IValidateOptions<ChannelsAppConfig>,
    IValidateOptions<List<ChannelsAppConfig>>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ChannelsAppConfig options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("ChannelsAppConfig 配置不能为 null。");
        }

        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail($"ChannelsAppConfig 配置验证失败: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, List<ChannelsAppConfig> options)
    {
        if (options == null || options.Count == 0)
        {
            return ValidateOptionsResult.Fail("ChannelsAppConfig 配置列表不能为 null 或空。");
        }

        var errors = new List<string>();
        for (var i = 0; i < options.Count; i++)
        {
            try
            {
                options[i].Validate();
            }
            catch (InvalidOperationException ex)
            {
                errors.Add($"小店[{i}] (AppKey: {options[i].AppKey}): {ex.Message}");
            }
        }

        var defaultApps = options.Where(c => c.IsDefault).ToList();
        if (defaultApps.Count > 1)
        {
            errors.Add(
                "存在多个 IsDefault=true 的应用（" +
                string.Join(", ", defaultApps.Select(c => c.AppKey)) +
                "），仅允许一个默认应用。");
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail("ChannelsAppConfig 配置验证失败:\n" + string.Join("\n", errors))
            : ValidateOptionsResult.Success;
    }
}