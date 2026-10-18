// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 参数校验失败的唯一抛出形态：<b>抛</b>（而不是返回错误文本），让执行链接管「分类/子类/建议/结局」。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么抛而不是返回</b>：<c>ToolResult.FromError(string)</c> 不产生 <c>ToolResult.Error</c>，
/// 返回它等于把「模型传错参数」登记成一次**成功**调用（失败率恒 0、<c>ErrorCode</c> 恒空）。
/// 抛出 <see cref="ToolArgumentException"/> 后由执行链统一回填
/// <c>Validation/invalid_args</c>（含 <c>suggestions</c>）并结算为 <c>Failed</c>。
/// </para>
/// <para>
/// <b>为什么需要这一行适配</b>：上游构造签名为 <c>(string, IReadOnlyList&lt;string&gt;)</c> —— 建议列表必填。
/// 本仓 97 处站点绝大多数只有一句可执行提示（"必须给 mobile/email/name 之一"），逐处写 <c>[]</c> 只是噪声；
/// 需要枚举合法取值（如 <c>msg_type</c> 闭集）的站点直接传第二个参数。
/// </para>
/// </remarks>
internal static class WechatToolArgumentErrors
{
    /// <summary>构造一个参数校验异常。</summary>
    /// <param name="message">可执行的中文原因（指出哪个参数、为什么、怎么改）。</param>
    /// <param name="suggestions">合法取值或修正建议（闭集参数必须给）。</param>
    /// <returns>待抛出的异常（本方法不抛出，便于 <c>throw</c> 表达式保留堆栈语义）。</returns>
    public static ToolArgumentException Invalid(string message, params string[] suggestions)
        => new(message, suggestions);
}
