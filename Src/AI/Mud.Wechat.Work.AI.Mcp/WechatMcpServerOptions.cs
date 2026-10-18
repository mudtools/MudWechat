// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Mcp;

/// <summary>
/// MCP server 的自描述信息（initialize 响应里的 <c>serverInfo</c> 与使用说明）。
/// </summary>
/// <remarks>
/// <para>
/// <b>不绑定配置节</b>：这是进程内的自描述事实（叫什么、什么版本、给客户端什么使用提示），
/// 由宿主以代码设置（<c>AddWechatWorkMcpServer(configure)</c>）。不做 <c>IOptions</c> 绑定是为了
/// 不把「服务端自我介绍」变成一个需要消费点审计的配置面——它没有运行期行为，只有契约内容。
/// </para>
/// <para>
/// <b>工具集不在此处配置</b>：模型可见面由 <c>WechatToolPolicy</c> 白名单决定（工具面唯一真相源），
/// MCP 层只做投影；在这里再开一份工具清单等于造第二个真相源。
/// </para>
/// </remarks>
public sealed class WechatMcpServerOptions
{
    /// <summary>服务端名（MCP 客户端显示用；须非空）。</summary>
    public string ServerName { get; set; } = "mud-wechat-work";

    /// <summary>服务端版本（建议与承载的程序集版本一致；须非空）。</summary>
    public string ServerVersion { get; set; } = "1.0.0";

    /// <summary>给客户端模型的使用说明（可空；写入 initialize 响应的 <c>instructions</c>）。</summary>
    public string? Instructions { get; set; }
}
