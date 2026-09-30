// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Work.DataModels;

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信 AOT JsonContext 合并扩展（对齐 <c>FeishuJsonResolverExtensions</c>）：
/// 把 SDK 数据模型的源生成序列化上下文合并进组件序列化管线。
/// </summary>
public static class WechatJsonResolverExtensions
{
    /// <summary>
    /// 将企业微信 DataModels 的 <see cref="WechatWorkJsonContext"/> 合并进组件
    /// <c>IOptions&lt;JsonSerializerOptions&gt;</c> 的 TypeInfoResolver 管线（NET8+）。
    /// </summary>
    public static void ConfigureDataModelsResolver(IServiceCollection services)
    {
        var dataModelsResolver = JsonTypeInfoResolver.Combine(WechatWorkJsonContext.Default);
        services.AddMudHttpClientJsonContext(dataModelsResolver);
    }
}
#endif
