// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>
/// 授权领域模型 AOT JsonContext（手写，模式对齐 DataModels 包 <c>Generated/</c> 生成的域上下文）：
/// 覆盖 <c>Authentication/Models/</c> 下全部标注 <c>[HttpJsonSerializable(SerializerClassName = "Authentication")]</c>
/// 的类型，供宿主在 AOT / Trim 环境下<b>安全地</b>持久化授权聚合（<see cref="WechatCorpAuthorization"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何必须存在（P0-5 / AOT006）</b>：这 8 个类型标注了 <c>[HttpJsonSerializable]</c>，
/// 而组件分析器 <c>AotDtoCoverageAnalyzer</c> 的 <c>AOT006</c>（severity = error）要求
/// 「标注了该特性的类型必须被某个 <see cref="JsonSerializerContext"/> 的 <c>[JsonSerializable(typeof(T))]</c> 覆盖」。
/// 缺失时 4 个源项目在 <c>AotStrictMode=true</c> 下各报 8 条错误，<c>verify-build.ps1</c> 步骤 2 直接失败。
/// </para>
/// <para>
/// <b>与 DataModels 域 JsonContext 的分工</b>：后者在 <c>DataModels</c> 包 <c>Generated/</c> 目录
/// （<c>scripts/GenerateJsonContext.ps1</c> 按域生成 15 个上下文）、覆盖<b>官方传输 DTO</b>；
/// 本上下文在 <c>Abstractions</c> 包、覆盖<b>授权领域模型</b>（由 <c>get_permanent_code</c> /
/// <c>get_auth_info</c> 响应映射得到）。二者经主包 <c>WechatJsonResolverExtensions</c>
/// 一并合并进组件序列化管线。
/// </para>
/// <para>
/// <b>维护方式</b>：类型清单以官方脚手架输出为准（新增/变更 <c>[HttpJsonSerializable]</c> 标注后重跑）：
/// <c>dotnet tool install -g Mud.HttpUtils.JsonContextScaffolder</c> →
/// <c>mud-jsonctx --project Mud.Wechat.Work.Abstractions\Mud.Wechat.Work.Abstractions.csproj --dry-run</c>。
/// 本文件按仓库风格手写（保留版权文件头与 XML 文档；脚手架产物为 <c>&lt;auto-generated&gt;</c> 裸文件）。
/// <b>漂移守卫</b>：若新增标注类型而未在此登记 AOT006 会立即转红，无需额外守卫。
/// </para>
/// <para>
/// <b>序列化口径</b>：<c>camelCase</c> + 大小写不敏感 + 忽略 null（与脚手架默认一致）。
/// 宿主若已有自己的持久化格式约定，可不使用本上下文（<c>IWechatCorpAuthStore</c> 为宿主实现的持久化端口）。
/// </para>
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(WechatAuthAgent))]
[JsonSerializable(typeof(WechatAuthCorpInfo))]
[JsonSerializable(typeof(WechatAuthPrivilege))]
[JsonSerializable(typeof(WechatAuthSharedFrom))]
[JsonSerializable(typeof(WechatAuthUserInfo))]
[JsonSerializable(typeof(WechatCorpAuthorization))]
[JsonSerializable(typeof(WechatCorpExName))]
[JsonSerializable(typeof(WechatDealerCorpInfo))]
public sealed partial class AuthenticationJsonContext : JsonSerializerContext
{
}
#endif
