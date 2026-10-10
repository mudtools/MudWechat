// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「插件管理」域 SDK（2 端点：插件申请管理 + 插件管理）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 插件管理，2026-10-10 依据官方清单核验）：
/// <c>plugin-management/api_managepluginapplication.html</c>（<c>/wxa/devplugin</c>，插件<b>开发者</b>视角）、
/// <c>api_manageplugin.html</c>（<c>/wxa/plugin</c>，插件<b>使用方</b>视角）。
/// </para>
/// <para>
/// <b>action 驱动</b>：两端点均为单路由多操作（<c>action</c> 字段区分语义），DTO 按官方参数表
/// 建模并标注各 action 的配套字段。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。两端点官方均为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Plugin", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaPluginService
{
    /// <summary>
    /// 插件申请管理（插件开发者视角）。官方文档：<c>plugin-management/api_managepluginapplication.html</c>。
    /// </summary>
    /// <param name="request"><c>action</c> 驱动的申请列表 / 同意 / 拒绝操作，见 <see cref="DataModels.Plugin.WxaDevPluginRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>插件使用方申请列表（<c>dev_apply_list</c> 时），见 <see cref="DataModels.Plugin.WxaDevPluginResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/devplugin</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>操作语义（官方原文）</b>：获取当前所有插件使用方信息（<c>dev_apply_list</c>）以及修改插件使用申请的状态（<c>dev_agree</c> / <c>dev_refuse</c>）。</para>
    /// </remarks>
    [Post("/wxa/devplugin")]
    Task<DataModels.Plugin.WxaDevPluginResponse> ManagePluginApplicationAsync(
        [Body] DataModels.Plugin.WxaDevPluginRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 插件管理（插件使用方视角）。官方文档：<c>plugin-management/api_manageplugin.html</c>。
    /// </summary>
    /// <param name="request"><c>action</c> 驱动的申请 / 查看 / 更新 / 删除操作，见 <see cref="DataModels.Plugin.WxaPluginRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已添加的插件列表（<c>list</c> 时），见 <see cref="DataModels.Plugin.WxaPluginResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/plugin</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>操作语义（官方原文）</b>：支持申请（<c>apply</c>）、查看（<c>list</c>）、更新（<c>update</c>）、删除（<c>delete</c>）插件等操作。</para>
    /// </remarks>
    [Post("/wxa/plugin")]
    Task<DataModels.Plugin.WxaPluginResponse> ManagePluginAsync(
        [Body] DataModels.Plugin.WxaPluginRequest request,
        CancellationToken cancellationToken = default);
}