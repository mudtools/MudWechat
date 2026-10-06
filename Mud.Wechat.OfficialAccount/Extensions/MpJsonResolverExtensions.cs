// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.OfficialAccount.DataModels;
using Mud.Wechat.OfficialAccount.DataModels.Basic;
using Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;
using Mud.Wechat.OfficialAccount.DataModels.Menu;
using Mud.Wechat.OfficialAccount.DataModels.Tag;
using Mud.Wechat.OfficialAccount.DataModels.User;

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 公众号 AOT JsonContext 合并扩展：把 SDK 数据模型的源生成序列化上下文合并进组件序列化管线。
/// </summary>
public static class MpJsonResolverExtensions
{
    /// <summary>
    /// 将公众号的源生成序列化上下文合并进组件 <c>IOptions&lt;JsonSerializerOptions&gt;</c> 的
    /// TypeInfoResolver 管线（NET8+）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <remarks>
    /// 全部 <c>*JsonContext</c> 均位于 <c>Mud.Wechat.OfficialAccount.DataModels/Generated/</c>，
    /// 由 <c>scripts/GenerateJsonContext.ps1</c>（mud-jsonctx）按 <c>[HttpJsonSerializable]</c> 标注生成
    /// （SerializerClassName = DTO 命名空间的域段，上下文与域 DTO 同命名空间）。
    /// 各上下文键空间不重叠，合并顺序无关（<c>JsonTypeInfoResolver.Combine</c> 按序命中）；
    /// 与企微产品线的上下文<b>类型集合不相交</b>，同一宿主共存时两个 resolver 合并亦无冲突。
    /// </remarks>
    public static void ConfigureDataModelsResolver(IServiceCollection services)
    {
        var resolver = JsonTypeInfoResolver.Combine(
            CommonJsonContext.Default,
            BasicJsonContext.Default,
            TagJsonContext.Default,
            UserJsonContext.Default,
            MenuJsonContext.Default,
            CustomerMessageJsonContext.Default);
        services.AddMudHttpClientJsonContext(resolver);
    }
}
#endif
