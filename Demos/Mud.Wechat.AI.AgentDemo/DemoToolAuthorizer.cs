// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Security;

namespace Mud.Wechat.AI.AgentDemo;

/// <summary>
/// Demo 授权器：只放行只读工具，写类一律拒绝（并说明拒绝理由）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须注册它</b>：<see cref="WechatToolPolicy.EnforceAuthorization"/> 默认为 <see langword="true"/>，
/// 而分发器在<b>容器里没有授权器</b>时是 <b>fail-closed 直接拒绝</b>（不是放行）。
/// 也就是说「忘了注册授权器」的表现是<b>所有工具都调不动</b>，而不是「工具全开」
/// —— 这是本 Demo 最容易踩空的一步，README 里单独列了症状与定位方法。
/// </para>
/// <para>
/// <b>判定必须是纯查询</b>：授权器处在每次工具调用的关键路径上，官方接口调用会翻倍延迟并新增失败面
/// （<see cref="IWechatToolAuthorizer"/> 的实现纪律）。真实形态是「回合前预取权限点 → 这里查表」。
/// </para>
/// </remarks>
internal sealed class DemoToolAuthorizer : IWechatToolAuthorizer
{
    /// <inheritdoc />
    public ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        // 风险档由生成物按工具契约给出（Read / Write / HighRiskWrite），不是模型可影响的入参。
        if (context.Risk != WechatToolRisk.Read)
        {
            return ValueTask.FromResult(WechatToolAuthorizationResult.Deny(
                $"Demo 只放行只读工具，本工具风险档为 {context.Risk}"));
        }

        // 白名单 + 只读档双双通过即放行；真实宿主在这里还应查「该应用对该部门是否有可见范围」。
        return ValueTask.FromResult(WechatToolAuthorizationResult.Allow());
    }
}
