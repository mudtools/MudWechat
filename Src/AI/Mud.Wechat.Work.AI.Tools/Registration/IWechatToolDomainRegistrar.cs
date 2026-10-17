// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Registration;

/// <summary>
/// 域注册器（生成物实现本接口）：把某个执行器类承载的工具批量装入 <see cref="WechatToolRegistry"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么由生成物实现而不是手写</b>：工具名 → 执行器方法的对应关系不可派生（工具名可缀化、方法名可任意），
/// 若手写就会出现「新增工具忘了登记」的静默漏面；生成器按 <c>[ToolHandler]</c> 聚合，漏登记即编译期错误。
/// </para>
/// <para>
/// <b>实现类为 internal</b>：装配入口按工厂委托持有它们，宿主只消费 <see cref="WechatToolRegistry"/>。
/// </para>
/// </remarks>
public interface IWechatToolDomainRegistrar
{
    /// <summary>把本域的工具装入注册表。</summary>
    /// <param name="registry">工具注册表（不得为 <see langword="null"/>）。</param>
    void Register(WechatToolRegistry registry);
}
