// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI;
using Mud.AI.Attributes;

namespace Mud.Wechat.Work.AI.Tools;

/// <summary>
/// 企微工具面剖面（<c>Mud.AI.Generator</c> 的输入契约）：33 槽逐一取值，槽的语义由
/// <see cref="SdkToolProfileAttribute"/> 定义，值换微信、结构不换。
/// </summary>
/// <remarks>
/// <para>
/// <b>命名空间 = 程序集归属</b>：<see cref="SdkToolProfileAttribute.OwnerAssembly"/> /
/// <see cref="SdkToolProfileAttribute.GeneratedNamespace"/> 等槽的字面量与工程一一对应，
/// 产物因此不会越过本程序集边界（不重演飞书「命名空间树 ⊃ 程序集边界」的张力）。
/// </para>
/// <para>
/// <b>令牌身份（决策③）</b>：企微令牌归属域的唯一真相源是接口上的 <c>[Token(TokenManagerKey)]</c>
/// （仅 <c>Internal</c> / <c>Corp</c> 两类），<b>不</b>用接口名后缀另推第二真相源；
/// 本剖面的 <see cref="SdkToolProfileAttribute.TokenKindMarkers"/> 仅在引擎需要「承载接口名 ⇒ 身份」
/// 兜底时生效，且父接口工具（未声明 <c>[Token]</c>）的身份由运行期 <c>UseCorpScope</c> 决定。
/// </para>
/// <para>
/// <b>诊断段隔离</b>：<see cref="SdkToolProfileAttribute.DiagnosticPrefix"/> 取 <c>MUDWT</c>，
/// 与飞书的 <c>MUDFT</c> 不共用段位（两生态的诊断口径不得分叉）。
/// </para>
/// </remarks>
[SdkToolProfile(
    name: "Wechat",
    ToolAttributeName = "AgentTool",
    ToolAttributeNamespace = "Mud.AI.Abstractions.Tools",
    ToolHandlerAttributeName = "ToolHandler",
    ToolHandlerAttributeNamespace = "Mud.AI.Abstractions.Tools.Handlers",
    ParameterAttributeName = "ToolParameter",
    SdkNamespaceRoot = "Mud.Wechat.Work",
    SoftAbsentNamespaceRoots = "Mud.AI",
    InterfaceNameRegex = "^IWechatWork(?<domain>[A-Z][a-zA-Z0-9]*?)(?<resource>[A-Z][a-zA-Z0-9]*)?Service(?<appType>_Internal|_Provider|_ThirdParty)?$",
    TokenKindStrategy = TokenKindDerivationStrategy.NamePrefix,
    TokenKindMarkers = "IWechatWork=Tenant",
    ProductPrefix = "WechatTool",
    ProductPluralPrefix = "WechatTools",
    DiagnosticPrefix = "MUDWT",
    DiagnosticCategory = "MudWechat.AI",
    ToolingDiagnosticCategory = "MudWechat.Tooling",
    // 危险词表：只收「破坏性动作」的动词。两个刻意不收录的量词/补救词——
    //   · batch：量词，BatchGetApprovalNumbersAsync 是只读查询，收录它会让审批读面永久被判 high-risk-write；
    //   · recall：补救动作（撤回自己刚发的消息），破坏性为 0，收录它会让每次纠错都要求人工审批。
    // 破坏性由 delete/remove/clear/destroy/dismiss/revoke 覆盖：BatchDelete*/BatchUpdate* 仍命中真实动词。
    WriteVerbKeywords = "delete|remove|kick|transfer|export|import|clear|revoke|disable|destroy|dismiss",
    SchemaExtensionKey = "x-wechat",
    OwnerAssembly = "Mud.Wechat.Work.AI.Tools",
    GeneratedNamespace = "Mud.Wechat.Work.AI.Tools.Generated",
    ContractNamespace = "Mud.Wechat.Work.AI.Tools",
    RegistrationNamespace = "Mud.Wechat.Work.AI.Tools.Registration",
    RiskEnumFullName = "Mud.Wechat.Work.AI.AgentTools.WechatToolRisk",
    ResultTypeName = "ToolResult",
    BindingTypeName = "WechatToolBinding",
    OutputWrapperNamespace = "Mud.Wechat.Work.DataModels",
    OutputWrapperTypeNames = "WechatWorkResponse",
    SdkInterfacePrefix = "IWechatWork",
    CapabilityCatalogPropertyName = "WechatToolCatalog",
    GoldenFileName = "WechatToolSchemas.golden.txt",
    GoldenUpdatePropertyName = "WechatToolGoldenUpdate",
    GuidanceDirectory = "/Guidance/",
    AffordanceDirectory = "")]
internal sealed class WechatToolProfile : ISdkToolProfile
{
}
