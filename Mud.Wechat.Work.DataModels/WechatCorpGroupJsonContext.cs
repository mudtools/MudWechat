// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using Mud.Wechat.Work.DataModels.CorpGroup;
using System.Text.Json.Serialization;

namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 上下游域（CorpGroup）独立 AOT JsonContext。
/// </summary>
/// <remarks>
/// <para>
/// 为什么单独拆出：上下游域与客户联系「客户管理」域各有一个官方端点
/// （<c>corpgroup/batch/external_userid_to_pending_id</c> 与
/// <c>idconvert/batch/external_userid_to_pending_id</c>），DTO 简单类型名相同
/// （<c>ExternalUserIdToPendingIdRequest/Response</c>）。System.Text.Json 源生成器
/// 按简单类型名去重，同名类型登记进同一 <see cref="JsonSerializerContext"/> 时后者会被
/// <b>静默丢弃</b>（无诊断、运行期 <c>GetTypeInfo</c> 返回 null）。
/// 故将上下游域的同名类型拆入本上下文，由主包 <c>WechatJsonResolverExtensions</c>
/// 一并合并进组件序列化管线（<see cref="WechatWorkJsonContext"/> 与本上下文键空间不重叠）。
/// </para>
/// <para>新增与既有域同名的 DTO 时，必须先核对官方端点是否同源；确属不同端点则登记进本上下文。</para>
/// </remarks>
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ExternalUserIdToPendingIdRequest))]
[JsonSerializable(typeof(ExternalUserIdToPendingIdResponse))]
public sealed partial class WechatCorpGroupJsonContext : JsonSerializerContext
{
}
#endif
